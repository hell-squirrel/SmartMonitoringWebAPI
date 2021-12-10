using Microsoft.AspNetCore.Mvc;
using SmartMonitoring.Service.Interfaces;
using SmartMonitoring.Service.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMonitoringWebAPI.Controllers
{
    [Route("services")]
    [ApiController]
    public class MonitoringAssignmentController : ControllerBase
    {
        private readonly ISmartMonitoringService _smartMonitoringService;

        public MonitoringAssignmentController(ISmartMonitoringService smartMonitoringService)
        {
            _smartMonitoringService = smartMonitoringService;
        }

        /// <summary>
        /// Get all Monitoring Assignments
        /// </summary>
        /// <param name="label">Get by label</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageNumber">Page Number</param>
        /// <returns></returns>
        [HttpGet]
        public Task<IEnumerable<MonitoringAssignmentView>> GetAssignments([FromQuery] string label, [FromQuery] int pageSize = 100, [FromQuery] int pageNumber = 1)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                return _smartMonitoringService.GetAllAssignments(pageSize, pageNumber);
            }
            else
            {
                return _smartMonitoringService.GetAssignmentByLabel(label);
            }
        }

        /// <summary>
        /// Get Assignment by name
        /// </summary>
        /// <param name="name">Name of Assignment</param>
        /// <returns></returns>
        [HttpGet("{assignmentName}")]
        public Task<MonitoringAssignmentView> GetByName(string assignmentName)
            => _smartMonitoringService.GetAssignmentByName(assignmentName);

        /// <summary>
        /// Create new Assignment with labels if required
        /// </summary>
        /// <param name="model">Model to create Assignment</param>
        /// <returns></returns>
        [HttpPost]
        public Task<MonitoringAssignmentView> CreateAssignment([FromBody] MonitoringAssignmentCreate model)
            => _smartMonitoringService.CreateAssignments(model);

        /// <summary>
        /// Update Assignment
        /// </summary>
        /// <param name="model">Model to update Assignment</param>
        /// <returns></returns>
        [HttpPut("{assignmentName}")]
        public Task<MonitoringAssignmentView> UpdateAssignment(string assignmentName, [FromBody] MonitoringAssignmentUpdate model)
            => _smartMonitoringService.UpdateAssignments(assignmentName,model);

        /// <summary>
        /// Delete Assignment by id
        /// </summary>
        /// <param name="name">Name of Assignment</param>
        /// <returns></returns>
        [HttpDelete("{assignmentName}")]
        public Task<MonitoringAssignmentView> DeleteAssignment(string assignmentName)
            => _smartMonitoringService.DeleteAssignments(assignmentName);
    }
}
