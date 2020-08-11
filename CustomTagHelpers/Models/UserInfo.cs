using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomTagHelpers.Models
{
    public class UserInfo
    {
        [Required]
        public string Name { get; set; }
        public float Surname { get; set; }
        public string Gender { get; set; }
        [Display(Name = "Birth Date")]
        public string Age { get; set; }
        public string Info { get; set; }
        public bool Confirm { get; set; }
    }
}
