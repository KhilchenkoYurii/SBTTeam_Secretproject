using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using PsychoWeb.Context;

namespace PsychoOnline.Context
{
    public class Record
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public Psychologist Psychologist { get; set; }

        [DataType(DataType.Date)]
        public DateTime InitialDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime AssignedDate { get; set; }
        
        [DataType(DataType.Time)]
        public DateTime ATime { get; set; }
    }
}
