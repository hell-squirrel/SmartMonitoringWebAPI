using System.Collections.Generic;

namespace SmartMonitoring.Service.Model
{
    public class MonitoringAssignmentView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Port { get; set; }
        public string Maintainer { get; set; }
        public IEnumerable<string> Labels { get; set; }
    }
}
