using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using PsychoOnline.Context;
using PsychoWeb.Context;

namespace PsychoOnline.VIewModels
{
    public class CreateRecordView
    {
        [Required]
        [Display(Name = "Psychologist")]
        public List<Psychologist> Psychologists { get; set; }

        [Required]
        [Display(Name = "AssignedDate")]
        [DataType(DataType.Date)]
        public DateTime AssignedDate { get; set; }

        [Required]
        [Display(Name = "ATime")]
        [DataType(DataType.Time)]
        public DateTime ATime { get; set; }
    }
}
