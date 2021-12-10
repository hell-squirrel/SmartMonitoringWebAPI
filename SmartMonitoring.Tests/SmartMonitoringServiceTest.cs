using Dapper;
using Microsoft.Data.Sqlite;
using SmartMonitoring.Commons.Configuration;
using SmartMonitoring.Commons.Exceptions;
using SmartMonitoring.Infrastructure.Services;
using SmartMonitoring.Service;
using SmartMonitoring.Service.Model;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SmartMonitoring.Tests
{
    public class SmartMonitoringServiceTest
    {
        private const string InMemoryConnectionString = "Data Source=Monitoring_test.sqlite";
        private readonly DatabaseConfiguration config = new DatabaseConfiguration { Name = InMemoryConnectionString };

        [Fact]
        public void GetAllAssignments_Valid()
        {
            // Arrange
            var connection = CreateDatabaseConnection();
            var id = connection.QueryFirst<int>("INSERT INTO MonitoringAssignment (name, port, maintainer) VALUES ('test', 1111, 'test@test.com'); SELECT last_insert_rowid()");
            connection.QueryFirst<int>("INSERT INTO MonitoringLabels (assignmentId, labelDescription) VALUES (@AssignmentId, 'test:test'); SELECT last_insert_rowid()", new { AssignmentId = id });
            var repo = new AssignmentRepository(config);
            var service = new SmartMonitoringService(repo);

            // Act
            var assignments = service.GetAllAssignments().Result;

            // Assert
            Assert.NotEmpty(assignments);
            var assignment = assignments.First();
            Assert.Equal(1111, assignment.Port);
            Assert.Equal("test", assignment.Name);
            Assert.Equal("test@test.com", assignment.Maintainer);
            Assert.Equal(new List<string> { "test:test" }, assignment.Labels);
            CleareDB(connection);
        }

        [Fact]
        public void GetAssignmentByLabel_Valid()
        {
            // Arrange
            var connection = CreateDatabaseConnection();
            var id = connection.QueryFirst<int>("INSERT INTO MonitoringAssignment (name, port, maintainer) VALUES ('test', 1111, 'test@test.com'); SELECT last_insert_rowid()");
            connection.QueryFirst<int>("INSERT INTO MonitoringLabels (assignmentId, labelDescription) VALUES (@AssignmentId, 'test:test'); SELECT last_insert_rowid()", new { AssignmentId = id });
            var repo = new AssignmentRepository(config);
            var service = new SmartMonitoringService(repo);

            // Act
            var assignments = service.GetAssignmentByLabel("test:test").Result;

            // Assert
            Assert.NotEmpty(assignments);
            var assignment = assignments.First();
            Assert.Equal(1111, assignment.Port);
            Assert.Equal("test", assignment.Name);
            Assert.Equal("test@test.com", assignment.Maintainer);
            Assert.Equal(new List<string> { "test:test" }, assignment.Labels);
            CleareDB(connection);
        }

        [Fact]
        public void GetAssignmentByLabel_Fail_byName()
        {
            // Arrange
            var connection = CreateDatabaseConnection();
            var id = connection.QueryFirst<int>("INSERT INTO MonitoringAssignment (name, port, maintainer) VALUES ('test', 1111, 'test@test.com'); SELECT last_insert_rowid()");
            connection.QueryFirst<int>("INSERT INTO MonitoringLabels (assignmentId, labelDescription) VALUES (@AssignmentId, 'test:test'); SELECT last_insert_rowid()", new { AssignmentId = id });
            var repo = new AssignmentRepository(config);
            var service = new SmartMonitoringService(repo);

            // Act ans Assert
            OperationException exception = Assert.ThrowsAsync<OperationException>(() => service.GetAssignmentByLabel("test")).Result;
            Assert.Equal("MonitoringLabel is not found by name:test", exception.Message);
            CleareDB(connection);
        }

        [Fact]
        public void GetAssignmentByName_Valid()
        {
            // Arrange
            var connection = CreateDatabaseConnection();
            var id = connection.QueryFirst<int>("INSERT INTO MonitoringAssignment (name, port, maintainer) VALUES ('test', 1111, 'test@test.com'); SELECT last_insert_rowid()");
            connection.QueryFirst<int>("INSERT INTO MonitoringLabels (assignmentId, labelDescription) VALUES (@AssignmentId, 'test:test'); SELECT last_insert_rowid()", new { AssignmentId = id });
            var repo = new AssignmentRepository(config);
            var service = new SmartMonitoringService(repo);

            // Act
            var assignment = service.GetAssignmentByName("test").Result;

            // Assert
            Assert.NotNull(assignment);
            Assert.Equal(1111, assignment.Port);
            Assert.Equal("test", assignment.Name);
            Assert.Equal("test@test.com", assignment.Maintainer);
            Assert.Equal(new List<string> { "test:test" }, assignment.Labels);
            CleareDB(connection);
        }

        [Fact]
        public void GetAssignmentByName_Fail_byName()
        {
            // Arrange
            var connection = CreateDatabaseConnection();
            var id = connection.QueryFirst<int>("INSERT INTO MonitoringAssignment (name, port, maintainer) VALUES ('test', 1111, 'test@test.com'); SELECT last_insert_rowid()");
            connection.QueryFirst<int>("INSERT INTO MonitoringLabels (assignmentId, labelDescription) VALUES (@AssignmentId, 'test:test'); SELECT last_insert_rowid()", new { AssignmentId = id });
            var repo = new AssignmentRepository(config);
            var service = new SmartMonitoringService(repo);

            // Act ans Assert
            OperationException exception = Assert.ThrowsAsync<OperationException>(() => service.GetAssignmentByName("test_none")).Result;
            Assert.Equal("MonitoringAssignment is not found by name:test_none", exception.Message);
            CleareDB(connection);
        }

        [Fact]
        public void UpdateAssignments_Valid()
        {
            // Arrange
            var connection = CreateDatabaseConnection();
            var id = connection.QueryFirst<int>("INSERT INTO MonitoringAssignment (name, port, maintainer) VALUES ('test', 1111, 'test@test.com'); SELECT last_insert_rowid()");
            connection.QueryFirst<int>("INSERT INTO MonitoringLabels (assignmentId, labelDescription) VALUES (@AssignmentId, 'test:test'); SELECT last_insert_rowid()", new { AssignmentId = id });
            var repo = new AssignmentRepository(config);
            var service = new SmartMonitoringService(repo);

            var model = new MonitoringAssignmentUpdate
            {
                Maintainer = "test@test",
                Port = 2222
            };

            // Act
            var assignment = service.UpdateAssignments("test", model).Result;

            // Assert
            Assert.NotNull(assignment);
            Assert.Equal(model.Port, assignment.Port);
            Assert.Equal(model.Maintainer, assignment.Maintainer);
            CleareDB(connection);
        }

        [Fact]
        public void CreateAssignments_Valid()
        {
            // Arrange
            var connection = CreateDatabaseConnection();
            var id = connection.QueryFirst<int>("INSERT INTO MonitoringAssignment (name, port, maintainer) VALUES ('test', 1111, 'test@test.com'); SELECT last_insert_rowid()");
            connection.QueryFirst<int>("INSERT INTO MonitoringLabels (assignmentId, labelDescription) VALUES (@AssignmentId, 'test:test'); SELECT last_insert_rowid()", new { AssignmentId = id });
            var repo = new AssignmentRepository(config);
            var service = new SmartMonitoringService(repo);

            var model = new MonitoringAssignmentCreate
            {
                Maintainer = "test@test",
                Name = "test2",
                Port = 2222,
                Labels = new List<string> { "test:test" }
            };
            // Act
            var assignments = service.CreateAssignments(model).Result;

            // Assert
            Assert.Equal(2222, assignments.Port);
            Assert.Equal(model.Labels, assignments.Labels);
            Assert.NotNull(assignments);
            CleareDB(connection);
        }

        [Fact]
        public void CreateAssignments_Fail_InvalidName()
        {
            // Arrange
            var connection = CreateDatabaseConnection();
            var id = connection.QueryFirst<int>("INSERT INTO MonitoringAssignment (name, port, maintainer) VALUES ('test', 1111, 'test@test.com'); SELECT last_insert_rowid()");
            connection.QueryFirst<int>("INSERT INTO MonitoringLabels (assignmentId, labelDescription) VALUES (@AssignmentId, 'test:test'); SELECT last_insert_rowid()", new { AssignmentId = id });
            var repo = new AssignmentRepository(config);
            var service = new SmartMonitoringService(repo);

            var model = new MonitoringAssignmentCreate
            {
                Maintainer = "test@test",
                Name = "test",
                Port = 2222,
                Labels = new List<string> { "test:test" }
            };

            // Act ans Assert
            OperationException exception = Assert.ThrowsAsync<OperationException>(() => service.CreateAssignments(model)).Result;
            Assert.Equal("MonitoringAssignment with this name already exist!", exception.Message);
            CleareDB(connection);
        }

        [Fact]
        public void UpdateAssignments_Fail_byId()
        {
            // Arrange
            var connection = CreateDatabaseConnection();
            var id = connection.QueryFirst<int>("INSERT INTO MonitoringAssignment (name, port, maintainer) VALUES ('test', 1111, 'test@test.com'); SELECT last_insert_rowid()");
            connection.QueryFirst<int>("INSERT INTO MonitoringLabels (assignmentId, labelDescription) VALUES (@AssignmentId, 'test:test'); SELECT last_insert_rowid()", new { AssignmentId = id });
            var repo = new AssignmentRepository(config);
            var service = new SmartMonitoringService(repo);

            var model = new MonitoringAssignmentUpdate
            {
                Maintainer = "test@test",
                Port = 2222
            };

            // Act ans Assert
            OperationException exception = Assert.ThrowsAsync<OperationException>(() => service.UpdateAssignments("test2", model)).Result;
            Assert.Equal("MonitoringAssignment is not found by Name:test2", exception.Message);
            CleareDB(connection);
        }

        [Fact]
        public void DeleteAssignments_Valid()
        {
            // Arrange
            var connection = CreateDatabaseConnection();
            var id = connection.QueryFirst<int>("INSERT INTO MonitoringAssignment (name, port, maintainer) VALUES ('test', 1111, 'test@test.com'); SELECT last_insert_rowid()");
            connection.QueryFirst<int>("INSERT INTO MonitoringLabels (assignmentId, labelDescription) VALUES (@AssignmentId, 'test:test'); SELECT last_insert_rowid()", new { AssignmentId = id });
            var repo = new AssignmentRepository(config);
            var service = new SmartMonitoringService(repo);

            // Act
            var assignments = service.DeleteAssignments("test").Result;

            // Assert
            Assert.Equal(1111, assignments.Port);
            Assert.Equal(new List<string> { "test:test" }, assignments.Labels);
            Assert.NotNull(assignments);
            CleareDB(connection);
        }

        [Fact]
        public void DeleteAssignments_Fail_byId()
        {
            // Arrange
            var connection = CreateDatabaseConnection();
            var id = connection.QueryFirst<int>("INSERT INTO MonitoringAssignment (name, port, maintainer) VALUES ('test', 1111, 'test@test.com'); SELECT last_insert_rowid()");
            connection.QueryFirst<int>("INSERT INTO MonitoringLabels (assignmentId, labelDescription) VALUES (@AssignmentId, 'test:test'); SELECT last_insert_rowid()", new { AssignmentId = id });
            var repo = new AssignmentRepository(config);
            var service = new SmartMonitoringService(repo);

            // Act ans Assert
            OperationException exception = Assert.ThrowsAsync<OperationException>(() => service.DeleteAssignments("none")).Result;
            Assert.Equal("MonitoringAssignment is not found by Name:none", exception.Message);
            CleareDB(connection);
        }

        private SqliteConnection CreateDatabaseConnection()
        {
            var initializer = new DatabaseInitializer(config, null);
            initializer.Setup().Wait();
            using var connection = new SqliteConnection(InMemoryConnectionString);
            return connection;
        }
        
        private void CleareDB(SqliteConnection connection)
        {
            connection.Execute("DELETE FROM MonitoringLabels");
            connection.Execute("DELETE FROM MonitoringAssignment");
        }
    }
}
