using SmartMonitoring.Service.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartMonitoring.Tests
{
    public class InputModelsTest
    {
        [Fact]
        public void CreateModel_Valid()
        {
            // Arrange
            var model = new MonitoringAssignmentCreate
            {
                Name = "test name",
                Maintainer = "test@test",
                Port = 1111
            };

            // Act and Assert 
            Assert.True(!ValidateModel(model).Any());
        }

        [Fact]
        public void CreateModel_Fail_Email()
        {
            // Arrange
            var model = new MonitoringAssignmentCreate
            {
                Name = "test name",
                Maintainer = "testtest",
                Port = 1111
            };

            // Act and Assert 
            Assert.Contains(ValidateModel(model), v => v.MemberNames.Contains("Maintainer") &&
                       v.ErrorMessage.Contains("The Maintainer field is not a valid e-mail address."));
        }

        [Fact]
        public void CreateModel_Fail_Port_low()
        {
            // Arrange
            var model = new MonitoringAssignmentCreate
            {
                Name = "test name",
                Maintainer = "test@test",
                Port = -1
            };

            // Act and Assert 
            Assert.Contains(ValidateModel(model), v => v.MemberNames.Contains("Port") &&
                       v.ErrorMessage.Contains("The field Port must be between 0 and 65535."));
        }

        [Fact]
        public void CreateModel_Fail_Port_high()
        {
            // Arrange
            var model = new MonitoringAssignmentCreate
            {
                Name = "test name",
                Maintainer = "test@test",
                Port = 99999999
            };

            // Act and Assert 
            Assert.Contains(ValidateModel(model), v => v.MemberNames.Contains("Port") &&
                       v.ErrorMessage.Contains("The field Port must be between 0 and 65535."));
        }

        [Fact]
        public void CreateModel_Fail_Name_short()
        {
            // Arrange
            var model = new MonitoringAssignmentCreate
            {
                Name = "one",
                Maintainer = "test@test",
                Port = 1111
            };

            // Act and Assert 
            Assert.Contains(ValidateModel(model), v => v.MemberNames.Contains("Name") &&
                       v.ErrorMessage.Contains("The field Name must be a string with a minimum length of 4 and a maximum length of 30."));
        }

        [Fact]
        public void CreateModel_Fail_Name_long()
        {
            // Arrange
            var model = new MonitoringAssignmentCreate
            {
                Name = "test name test name test name test name test name test name test name test name ",
                Maintainer = "test@test",
                Port = 1111
            };

            // Act and Assert 
            Assert.Contains(ValidateModel(model), v => v.MemberNames.Contains("Name") &&
                       v.ErrorMessage.Contains("The field Name must be a string with a minimum length of 4 and a maximum length of 30."));
        }

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
    }
}
