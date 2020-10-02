using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DevicesApi.Domain
{
    public class Reading
    {
        [Required]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public long Timestamp { get; set; }
        [Required]
        public string Reading_type { get; set; }
        [Required]
        public int Raw_value { get; set; }

        [Required]
        [ForeignKey("Device")]
        public int Device_id { get; set; }
    }
}
