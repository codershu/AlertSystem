using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SystemAlert.Data.Context
{
    public static class MigrationExtensions
    {
        public static void ExecuteFile(this MigrationBuilder migrationBuilder, string filePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var sqlFiles = assembly.GetManifestResourceNames().Where(file => file.EndsWith(filePath));

            if (sqlFiles.Count() != 1)
                throw new InvalidOperationException($"{filePath} Files Fount: {sqlFiles.Count()} Expected Result: 1");

            using (var stream = assembly.GetManifestResourceStream(sqlFiles.First()))
            using (var reader = new StreamReader(stream))
            {
                var sqlScript = reader.ReadToEnd();
                var escapedSqlScript = sqlScript.Replace("'", "''");
                migrationBuilder.Sql($"EXEC(N'{escapedSqlScript}')");
            }
        }

    }
}
