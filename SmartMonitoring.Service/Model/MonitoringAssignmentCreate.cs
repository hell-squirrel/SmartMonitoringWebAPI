using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartMonitoring.Service.Model
{
    public class MonitoringAssignmentCreate
    {
        [Required]
        [StringLength(30, MinimumLength = 4)]
        public string Name { get; set; }

        [Required]
        [Range(0, 65535)]
        public int Port { get; set; }

        [EmailAddress]
        public string Maintainer { get; set; }

        public IEnumerable<string> Labels { get; set; }
    }
}
