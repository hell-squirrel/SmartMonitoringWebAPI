using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using SmartMonitoring.Commons.Configuration;
using SmartMonitoring.Infrastructure.Interfaces;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace SmartMonitoring.Infrastructure.Services
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private static string _findTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name = '{0}'";
        private static string _monitoringLabelsTable = "MonitoringLabels";
        private static string _monitoringAssignmentTable = "MonitoringAssignment";

        private readonly DatabaseConfiguration _databaseConfiguration;
        private readonly ILogger _logger;

        public DatabaseInitializer(DatabaseConfiguration databaseConfiguration, ILogger<DatabaseConfiguration> logger)
        {
            _databaseConfiguration = databaseConfiguration;
            _logger = logger;
        }

        /// <summary>
        /// Setup base database configuration
        /// </summary>
        /// <returns>Successful task result</returns>
        public async Task Setup()
        {
            try
            {
                using var connection = new SqliteConnection(_databaseConfiguration.Name);
                connection.Open();
                await CreateMonitoringLabels(connection);
                await CreateMonitoringAssignment(connection);
                connection.Close();
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Validate or create MonitoringLabels table
        /// </summary>
        /// <param name="connection">to database</param>
        /// <returns>Successful task result</returns>
        private async Task CreateMonitoringLabels(DbConnection connection)
        {
            var tableMonitoringLabels = await connection.QueryAsync<string>(string.Format(_findTableQuery, _monitoringLabelsTable));
            var tableNameMonitoringLabels = tableMonitoringLabels.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableNameMonitoringLabels) && tableNameMonitoringLabels == _monitoringLabelsTable)
                return;

            connection.Execute("CREATE TABLE MonitoringLabels (" +
              "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
              "assignmentId INT NOT NULL," +
              "labelDescription VARCHAR(30) NOT NULL," +
              "FOREIGN KEY(assignmentId) REFERENCES MonitoringAssignment(id))");
        }

        /// <summary>
        /// Validate or create MonitoringAssignment table
        /// </summary>
        /// <param name="connection">Connection to database</param>
        /// <returns>Successful task result</returns>
        private async Task CreateMonitoringAssignment(DbConnection connection)
        {
            var tableMonitoringAssignment = await connection.QueryAsync<string>(string.Format(_findTableQuery, _monitoringAssignmentTable));
            var tableNameMonitoringAssignment = tableMonitoringAssignment.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableNameMonitoringAssignment) && tableNameMonitoringAssignment == _monitoringAssignmentTable)
                return;

            connection.Execute("CREATE TABLE MonitoringAssignment (" +
                "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "name VARCHAR(30) NOT NULL," +
                "port INT NOT NULL," +
                "maintainer VARCHAR(100) NULL)");
        }
    }
}
