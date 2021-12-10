using SmartMonitoring.Service.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMonitoring.Service.Interfaces
{
    public interface ISmartMonitoringService
    {
        Task<IEnumerable<MonitoringAssignmentView>> GetAllAssignments(int pageSize = 100, int pageNumber = 1);

        Task<MonitoringAssignmentView> GetAssignmentByName(string assignmentName);

        Task<IEnumerable<MonitoringAssignmentView>> GetAssignmentByLabel(string assignmentLabel);

        Task<MonitoringAssignmentView> CreateAssignments(MonitoringAssignmentCreate model);

        Task<MonitoringAssignmentView> DeleteAssignments(string name);

        Task<MonitoringAssignmentView> UpdateAssignments(string name, MonitoringAssignmentUpdate model);

    }
}
