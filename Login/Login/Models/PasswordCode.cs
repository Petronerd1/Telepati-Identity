using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Login.Models
{
    public class PasswordCode
    {
        public int ID { get; set; }
        public User User { get; set; }
        public int UserID { get; set; }
        [StringLength(6)]
        public string Code { get; set; }
    }
}
