using System.Collections.Generic;

namespace SmartMonitoring.Service.Model
{
    public class MonitoringAssignmentUpdate
    {
        public int Port { get; set; }
        public string Maintainer { get; set; }
        public IEnumerable<string> Labels { get; set; }
    }
}
