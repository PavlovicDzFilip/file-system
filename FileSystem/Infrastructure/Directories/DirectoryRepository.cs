using System;
using FileSystem.Domain;
using FileSystem.Domain.Directories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FileSystem.Infrastructure.Directories
{
    public class DirectoryRepository : IDirectoryRepository
    {
        private readonly Database _database;

        public DirectoryRepository(Database database)
            => _database = database;

        public Task<Directory> GetById(DirectoryId directoryId)
            => _database
                .Directories
                .AsNoTracking()
                .FirstOrDefaultAsync(dir => dir.Id == directoryId);

        public Task<Directory[]> GetByIdWithAllChildren(DirectoryId directoryId)
            => GetDirectoryWithChildren(directoryId)
                .ToArrayAsync();

        public Task<Directory> GetByNameForParent(DirectoryId parentId, DirectoryName directoryName)
            => WithParent(parentId)
                .AsNoTracking()
                .FirstOrDefaultAsync(dir => dir.Name.Equals(directoryName));

        public void Add(Directory directory)
            => _database
                .Directories
                .Add(directory);

        public void AddRange(Directory[] directories)
            => directories
                .ForEach(Add);

        public Task<Directory[]> GetChildren(DirectoryId parentId)
            => WithParent(parentId)
                .AsNoTracking()
                .ToArrayAsync();

        public async Task<DirectoryPath> GetDirectoriesInPath(Path path)
        {
            var names = GetDirectoryNamesParameters(path);
            var query = PrepareDirectoryPathCteQuery(names);

            var parameters = names.Cast<object>().ToArray();
            var directories = await _database.Directories
                .FromSqlRaw(query, parameters)
                .ToArrayAsync();

            return new DirectoryPath(directories);
        }

        public async Task<Directory> GetLastDirectoryInPath(Path path)
        {
            var directoryPath = await GetDirectoriesInPath(path);
            return directoryPath.Next.MatchFull(path);
        }

        public void Update(Directory directory)
            => _database.Entry(directory).State = EntityState.Modified;

        public void Remove(Directory directory)
        {
            _database
                .Directories
                .Remove(directory);
        }

        public Task RemoveFast(DirectoryId directoryId)
        {
            var directoryIdParameter = GetStartDirectorySqlParameter(directoryId);
            var parameters = directoryIdParameter == default ? 
                Array.Empty<object>() : 
                new object[] { directoryIdParameter };

            var cteQuery = PrepareDirectoryWithChildrenCteQuery(directoryIdParameter);
            var directoryIdsQuery = $"Select Id from CTE_FileSystem";
            var deleteFilesQuery = $@"DELETE from Files where ParentId in ({directoryIdsQuery});";
            var deleteDirectoriesQuery = $@"DELETE from Directories where ParentId in ({directoryIdsQuery});";
            var query = $@"

{cteQuery}

{deleteFilesQuery}

{cteQuery}

{deleteDirectoriesQuery}";

            return _database.Database.ExecuteSqlRawAsync(query, parameters);
        }

        private SqlParameter GetStartDirectorySqlParameter(DirectoryId directoryId)
        {
            if (directoryId == DirectoryId.NoParent)
            {
                return default;
            }

            var directoryIdParameter = new SqlParameter("@directoryId", SqlDbType.BigInt)
            {
                Value = directoryId.Value
            };

            return directoryIdParameter;
        }

        private IQueryable<Directory> GetDirectoryWithChildren(DirectoryId directoryId)
        {
            var directoryIdParameter = GetStartDirectorySqlParameter(directoryId);
            var parameters = directoryIdParameter == default ?
                Array.Empty<object>() :
                new object[] { directoryIdParameter };

            var query = PrepareDirectoryWithChildrenCteQuery(directoryIdParameter) +
                        "SELECT Id, Name, ParentId, CreatedAt FROM CTE_FileSystem";

            IQueryable<Directory> childrenDirectories = _database
                .Directories
                .FromSqlRaw(query, parameters);

            return childrenDirectories;
        }

        private static string PrepareDirectoryWithChildrenCteQuery(SqlParameter directoryIdParameter)
        {
            var idCriteria = directoryIdParameter == default ? 
                "IS NULL" : 
                $"= {directoryIdParameter.ParameterName}";

            var query = @$";
WITH CTE_FileSystem AS (
SELECT Id, Name, ParentId, CreatedAt
FROM       
    [dbo].[Directories]
WHERE Id {idCriteria}
UNION ALL
SELECT d.Id, d.Name, d.ParentId, d.CreatedAt
FROM       
    [dbo].[Directories] d
    INNER JOIN CTE_FileSystem fs 
        ON d.ParentId = fs.Id
)

";

            return query;
        }

        private static string PrepareDirectoryPathCteQuery(IReadOnlyCollection<SqlParameter> names)
        {
            var directoryIdNamePairs = names.Select(((parameter, i) => $"({i}, {parameter.ParameterName})"));
            var directoryNamesValues = string.Join(", ", directoryIdNamePairs);

            var query = @$"
DECLARE @directoryNames TABLE (DirectoryDepth int primary key, Name nvarchar({DirectoryName.MaxLength}));
INSERT INTO @directoryNames VALUES {directoryNamesValues}

;WITH CTE_FileSystem AS (
SELECT Id, Name, ParentId, CreatedAt, 0 as Depth
FROM       
    [dbo].[Directories]
WHERE ParentId IS NULL AND Name = (SELECT Name FROM @directoryNames AS dn WHERE dn.DirectoryDepth = 0)
UNION ALL
SELECT d.Id, d.Name, d.ParentId, d.CreatedAt, fs.Depth + 1 as Depth
FROM       
    [dbo].[Directories] d
    INNER JOIN CTE_FileSystem fs 
        ON d.ParentId = fs.Id
    WHERE d.Name IN (SELECT Name FROM @directoryNames AS dn WHERE dn.DirectoryDepth = Depth + 1)
)

SELECT Id, Name, ParentId, CreatedAt FROM CTE_FileSystem
";

            return query;
        }

        private static List<SqlParameter> GetDirectoryNamesParameters(Path path)
        {
            var names = new List<SqlParameter>();
            int counter = 0;

            while (!path.IsEmpty())
            {
                var nameParam = new SqlParameter($"@name_{counter}", SqlDbType.NVarChar, DirectoryName.MaxLength)
                {
                    Value = path.Current.Value
                };

                names.Add(nameParam);
                path = path.Next;

                counter++;
            }

            return names;
        }

        private IQueryable<Directory> WithParent(DirectoryId parentDirectoryId)
            => _database
                .Directories
                .Where(dir => dir.ParentId == parentDirectoryId);
    }
}