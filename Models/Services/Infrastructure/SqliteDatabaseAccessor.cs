using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sapientopoli.Models.Services.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Sapientopoli.Models.Services.Infrastructure
{
    public class SqliteDatabaseAccessor : IDatabaseAccessor
    {
        public async Task<DataSet> QueryAsync(FormattableString formattableQuery)
        {
            var queryArguments = formattableQuery.GetArguments();
            var sqliteParameters = new List<SqliteParameter>();
            for (var i = 0; i < queryArguments.Length; i++)
            {
                var parameter = new SqliteParameter(i.ToString(), queryArguments[i]);
                sqliteParameters.Add(parameter);
                queryArguments[i] = "@" + i;
            }

            string query = formattableQuery.ToString();

            // Logging per la query e i parametri
            Console.WriteLine("Esecuzione della query: " + query);
            Console.WriteLine("Parametri:");
            foreach (var parameter in sqliteParameters)
            {
                Console.WriteLine($"{parameter.ParameterName}: {parameter.Value}");
            }

            using (var conn = new SqliteConnection("Data Source=Data/sapientopoli.db"))
            {
                await conn.OpenAsync();
                using (var cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.AddRange(sqliteParameters);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var dataSet = new DataSet();
                        dataSet.EnforceConstraints = false;
                        do
                        {
                            var dataTable = new DataTable();
                            dataSet.Tables.Add(dataTable);
                            dataTable.Load(reader);
                        } while (!reader.IsClosed);
                        return dataSet;
                    }
                }
            }
        }

    }
}