using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevicesApi.Domain
{
    public class Device
    {
        [Key]
        public int Device_id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Location { get; set; }
    }
}
