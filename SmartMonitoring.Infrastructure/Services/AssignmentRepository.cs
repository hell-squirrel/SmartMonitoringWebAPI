using Dapper;
using Microsoft.Data.Sqlite;
using SmartMonitoring.Commons.Configuration;
using SmartMonitoring.Domain.Models;
using SmartMonitoring.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace SmartMonitoring.Infrastructure.Services
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly DbConnection _connection;

        public AssignmentRepository(DatabaseConfiguration databaseConfiguration)
        {
            _connection = new SqliteConnection(databaseConfiguration.Name);
        }

        public async Task<bool> DeleteMonitoringAssignment(int id)
        {
            return await _connection.ExecuteAsync(@"DELETE FROM MonitoringAssignment WHERE id = @Id", new { Id = id }) > 0;
        }

        public async Task<bool> DeleteMonitoringLabel(int id)
        {
            return await _connection.ExecuteAsync(@"DELETE FROM MonitoringLabels WHERE AssignmentId = @Id", new { Id = id }) > 0;
        }
        public async Task<bool> DeleteMonitoringLabels(IEnumerable<int> ids)
        {
            return await _connection.ExecuteAsync(@"DELETE FROM MonitoringLabels WHERE Id in (@Ids)", new { Ids = ids }) > 0;
        }

        public Task<IEnumerable<MonitoringAssignment>> GetAllMonitoringAssignments(int pageSize = 100, int pageNumber = 1)
        {
            return _connection.QueryAsync<MonitoringAssignment>(@"SELECT * FROM MonitoringAssignment LIMIT @Limit OFFSET @Offset",
                new { Limit = pageSize, Offset = (pageNumber - 1) * pageSize });
        }

        public Task<IEnumerable<MonitoringLabel>> GetAllMonitoringLabels()
        {
            return _connection.QueryAsync<MonitoringLabel>(@"SELECT * FROM MonitoringLabels");
        }

        public Task<MonitoringAssignment> GetMonitoringAssignmentByName(string name)
        {
            return _connection.QueryFirstOrDefaultAsync<MonitoringAssignment>(@"SELECT * FROM MonitoringAssignment WHERE Name = @Name", new { Name = name });
        }

        public Task<IEnumerable<MonitoringLabel>> GetMonitoringLabelByDescription(string description)
        {
            return _connection.QueryAsync<MonitoringLabel>(@"SELECT * FROM MonitoringLabels WHERE LabelDescription = @LabelDescription", new { LabelDescription = description });
        }

        public Task<MonitoringLabel> GetMonitoringLabelById(int id)
        {
            return _connection.QueryFirstOrDefaultAsync<MonitoringLabel>(@"SELECT * FROM MonitoringLabels WHERE LabelId = @LabelId", new { LabelId = id });
        }

        public Task<IEnumerable<MonitoringLabel>> GetMonitoringLabelsById(int assignmentId)
        {
            return _connection.QueryAsync<MonitoringLabel>(@"SELECT * FROM MonitoringLabels WHERE AssignmentId in (@AssignmentId)", new { AssignmentId = assignmentId });
        }

        public Task<int> Insert(MonitoringLabel monitoringLabel)
        {
            return _connection.QueryFirstOrDefaultAsync<int>(@"INSERT INTO MonitoringLabels (assignmentId, labelDescription) VALUES (@AssignmentId, @LabelDescription); SELECT last_insert_rowid()", monitoringLabel);
        }

        public Task<int> Insert(MonitoringAssignment monitoringAssignment)
        {
            return _connection.QueryFirstOrDefaultAsync<int>(@"INSERT INTO MonitoringAssignment (name, port, maintainer) VALUES (@Name, @Port, @Maintainer ); SELECT last_insert_rowid()", monitoringAssignment);
        }

        public async Task<bool> Update(MonitoringAssignment monitoringAssignment)
        {
            return await _connection.ExecuteAsync(@"UPDATE MonitoringAssignment SET name = @Name, port = @Port, maintainer = @Maintainer WHERE id = @Id", monitoringAssignment) > 0;
        }

        public Task<IEnumerable<MonitoringLabel>> GetMonitoringLabelByAssignmentId(int id)
        {
            return _connection.QueryAsync<MonitoringLabel>(@"SELECT * FROM MonitoringLabels WHERE AssignmentId in (@AssignmentIds)", new { AssignmentIds = id });
        }
        public Task<MonitoringAssignment> GetMonitoringAssignmentById(int id)
        {
            return _connection.QueryFirstOrDefaultAsync<MonitoringAssignment>(@"SELECT * FROM MonitoringAssignment WHERE Id = @Id", new { Id = id });
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }


    }
}
