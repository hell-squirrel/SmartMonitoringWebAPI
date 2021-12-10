using SmartMonitoring.Commons.Exceptions;
using SmartMonitoring.Domain.Models;
using SmartMonitoring.Infrastructure.Interfaces;
using SmartMonitoring.Service.Interfaces;
using SmartMonitoring.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace SmartMonitoring.Service
{
    public class SmartMonitoringService : ISmartMonitoringService
    {
        private readonly IAssignmentRepository _assignmentRepository;

        public SmartMonitoringService(IAssignmentRepository assignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
        }

        public async Task<MonitoringAssignmentView> CreateAssignments(MonitoringAssignmentCreate model)
        {
            using var transaction = new TransactionScope();

            var assignment = await _assignmentRepository.GetMonitoringAssignmentByName(model.Name);
            if (assignment != null)
            {
                throw new OperationException("MonitoringAssignment with this name already exist!");
            }

            var monitoringAssignmentId = await _assignmentRepository.Insert(new MonitoringAssignment
            {
                Name = model.Name,
                Maintainer = model.Maintainer,
                Port = model.Port
            });

            if (model.Labels != null && model.Labels.Any())
            {
                foreach (var description in model.Labels)
                {
                    await _assignmentRepository.Insert(new MonitoringLabel
                    {
                        AssignmentId = monitoringAssignmentId,
                        LabelDescription = description
                    });
                }
            }
            transaction.Complete();
            return await GetAssignmentByName(model.Name);
        }

        public async Task<MonitoringAssignmentView> DeleteAssignments(string name)
        {
            var monitoringAssignment = await GetMonitoringAssignmentByName(name);
            var monitoringLabel = await GetMonitoringLabelByAssignmentId(monitoringAssignment.Id);

            _ = await _assignmentRepository.DeleteMonitoringLabel(monitoringAssignment.Id);
            _ = await _assignmentRepository.DeleteMonitoringAssignment(monitoringAssignment.Id);

            return MapDataModeltoResponse(monitoringAssignment, monitoringLabel);
        }

        public async Task<IEnumerable<MonitoringAssignmentView>> GetAllAssignments(int pageSize = 100, int pageNumber = 1)
        {
            var monitoringLabelsTask = _assignmentRepository.GetAllMonitoringLabels();
            var monitoringAssignmentsTask = _assignmentRepository.GetAllMonitoringAssignments(pageSize, pageNumber);

            await Task.WhenAll(new Task[] { monitoringAssignmentsTask, monitoringLabelsTask });

            var monitoringLabels = monitoringLabelsTask.Result;
            var monitoringAssignments = monitoringAssignmentsTask.Result;

            var resultData = monitoringAssignments.Select(assignment =>
            {
                return MapDataModeltoResponse(assignment, monitoringLabels);
            });

            return resultData;
        }

        public async Task<IEnumerable<MonitoringAssignmentView>> GetAssignmentByLabel(string assignmentLabel)
        {
            var monitoringLabel = await _assignmentRepository.GetMonitoringLabelByDescription(assignmentLabel);
            if (!monitoringLabel.Any())
                throw new OperationException($"{nameof(MonitoringLabel)} is not found by name:{assignmentLabel}");

            var result = new List<MonitoringAssignmentView>();
            foreach (var assignmentId in monitoringLabel.Select(x => x.AssignmentId).Distinct())
            {
                var monitoringAssignment = await GetMonitoringAssignmentById(assignmentId);

                var monitoringLabels = await _assignmentRepository.GetMonitoringLabelsById(assignmentId);
                if (monitoringLabel == null)
                    throw new OperationException($"{nameof(MonitoringLabel)} is not found by name:{assignmentLabel}");

                var resultData = MapDataModeltoResponse(monitoringAssignment, monitoringLabels);
                result.Add(resultData);
            }
            return result;
        }

        public async Task<MonitoringAssignmentView> GetAssignmentByName(string assignmentName)
        {
            var monitoringAssignment = await _assignmentRepository.GetMonitoringAssignmentByName(assignmentName);
            if (monitoringAssignment == null)
                throw new OperationException($"{nameof(MonitoringAssignment)} is not found by name:{assignmentName}");

            var monitoringLabels = await GetMonitoringLabelByAssignmentId(monitoringAssignment.Id);

            var resultData = MapDataModeltoResponse(monitoringAssignment, monitoringLabels);
            return resultData;
        }

        public async Task<MonitoringAssignmentView> UpdateAssignments(string name, MonitoringAssignmentUpdate updateModel)
        {
            var monitoringAssignment = await GetMonitoringAssignmentByName(name);
            var monitoringLabels = await GetMonitoringLabelByAssignmentId(monitoringAssignment.Id);

            var updatedMonitoringAssignment = await UpdateMonitoringAssignment(updateModel, monitoringAssignment);
            var updatedMonitoringLabels = await UpdateMonitoringLabels(updateModel, monitoringLabels, monitoringAssignment.Id);

            var resultData = MapDataModeltoResponse(updatedMonitoringAssignment, updatedMonitoringLabels);
            return resultData;
        }

        /// <summary>
        /// Create response model with mapped labels
        /// </summary>
        /// <param name="monitoringAssignment"></param>
        /// <param name="labels"></param>
        /// <returns></returns>
        private MonitoringAssignmentView MapDataModeltoResponse(MonitoringAssignment monitoringAssignment, IEnumerable<MonitoringLabel> labels)
        {
            return new MonitoringAssignmentView
            {
                Id = monitoringAssignment.Id,
                Maintainer = monitoringAssignment.Maintainer,
                Name = monitoringAssignment.Name,
                Port = monitoringAssignment.Port,
                Labels = labels.Where(x => x.AssignmentId == monitoringAssignment.Id).Select(x => x.LabelDescription)
            };
        }

        private async Task<IEnumerable<MonitoringLabel>> UpdateMonitoringLabels(MonitoringAssignmentUpdate updateModel, IEnumerable<MonitoringLabel> monitoringLabels, int assignmentId)
        {
            if (updateModel.Labels != null && updateModel.Labels.Any())
            {
                _ = await _assignmentRepository.DeleteMonitoringLabels(monitoringLabels.Select(x => x.Id));

                monitoringLabels = updateModel.Labels.Select(x => new MonitoringLabel
                {
                    AssignmentId = assignmentId,
                    LabelDescription = x
                });

                foreach (var label in monitoringLabels)
                {
                    await _assignmentRepository.Insert(label);
                }
            }
            return monitoringLabels;
        }

        private async Task<MonitoringAssignment> GetMonitoringAssignmentByName(string name)
        {
            var monitoringAssignment = await _assignmentRepository.GetMonitoringAssignmentByName(name);
            if (monitoringAssignment == null)
                throw new OperationException($"{nameof(MonitoringAssignment)} is not found by Name:{name}");

            return monitoringAssignment;
        }

        private async Task<MonitoringAssignment> GetMonitoringAssignmentById(int assignmentId)
        {
            var monitoringAssignment = await _assignmentRepository.GetMonitoringAssignmentById(assignmentId);
            if (monitoringAssignment == null)
                throw new OperationException($"{nameof(MonitoringAssignment)} is not found by id:{assignmentId}");

            return monitoringAssignment;
        }

        private async Task<IEnumerable<MonitoringLabel>> GetMonitoringLabelByAssignmentId(int assignmentId)
        {
            var monitoringLabel = await _assignmentRepository.GetMonitoringLabelByAssignmentId(assignmentId);
            if (monitoringLabel == null)
                throw new OperationException($"{nameof(MonitoringLabel)} is not found by assignment id:{assignmentId}");

            return monitoringLabel;
        }

        private async Task<MonitoringAssignment> UpdateMonitoringAssignment(MonitoringAssignmentUpdate updateModel, MonitoringAssignment monitoringAssignment)
        {
            var (updateMonitoringAssignment, updatedModel) = ValidateMonitoringAssignmentUpdatedValues(monitoringAssignment, updateModel);
            if (updateMonitoringAssignment)
            {
                _ = await _assignmentRepository.Update(updatedModel);
            }

            return updatedModel;
        }

        private Tuple<bool, MonitoringAssignment> ValidateMonitoringAssignmentUpdatedValues(MonitoringAssignment currentModel, MonitoringAssignmentUpdate newModel)
        {
            var needToUpdate = false;
            if (newModel.Maintainer != null && currentModel.Maintainer != newModel.Maintainer)
            {
                needToUpdate = true;
                currentModel.Maintainer = newModel.Maintainer;
            }

            if (newModel.Port >= 1 && newModel.Port <= 65535 && currentModel.Port != newModel.Port)
            {
                needToUpdate = true;
                currentModel.Port = newModel.Port;
            }

            return new Tuple<bool, MonitoringAssignment>(needToUpdate, currentModel);

        }
    }
}
