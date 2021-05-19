using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PsychoWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreateDate { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
