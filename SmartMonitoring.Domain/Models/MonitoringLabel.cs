namespace SmartMonitoring.Domain.Models
{
    public class MonitoringLabel
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public string LabelDescription { get; set; }
    }
}
