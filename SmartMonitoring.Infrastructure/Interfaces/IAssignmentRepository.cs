using SmartMonitoring.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMonitoring.Infrastructure.Interfaces
{
    public interface IAssignmentRepository : IDisposable
    {
        #region MonitoringLabel

        Task<IEnumerable<MonitoringLabel>> GetAllMonitoringLabels();

        Task<IEnumerable<MonitoringLabel>> GetMonitoringLabelByDescription(string description);

        Task<MonitoringLabel> GetMonitoringLabelById(int id);

        Task<IEnumerable<MonitoringLabel>> GetMonitoringLabelByAssignmentId(int id);

        Task<IEnumerable<MonitoringLabel>> GetMonitoringLabelsById(int assignmentId);

        Task<bool> DeleteMonitoringLabels(IEnumerable<int> ids);

        Task<int> Insert(MonitoringLabel monitoringLabel);

        Task<bool> DeleteMonitoringLabel(int id);
        #endregion


        #region MonitoringAssignment
        Task<IEnumerable<MonitoringAssignment>> GetAllMonitoringAssignments(int pageSize = 100, int pageNumber = 1);

        Task<MonitoringAssignment> GetMonitoringAssignmentByName(string name);

        Task<MonitoringAssignment> GetMonitoringAssignmentById(int id);

        Task<int> Insert(MonitoringAssignment monitoringAssignment);

        Task<bool> Update(MonitoringAssignment monitoringAssignment);

        Task<bool> DeleteMonitoringAssignment(int id);

        #endregion
    }
}
