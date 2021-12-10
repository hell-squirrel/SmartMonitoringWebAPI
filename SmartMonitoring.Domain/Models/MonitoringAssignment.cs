namespace SmartMonitoring.Domain.Models
{
    public class MonitoringAssignment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Port { get; set; }
        public string Maintainer { get; set; }
    }
}
