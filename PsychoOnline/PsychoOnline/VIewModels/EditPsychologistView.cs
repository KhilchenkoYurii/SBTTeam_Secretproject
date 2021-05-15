using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PsychoOnline.VIewModels
{
    public class EditPsychologistView
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public List<IdentityRole> AllRoles { get; set; }

        public IList<string> UserRoles { get; set; }

        public bool UnMakePsychologist { get; set; }

        public EditPsychologistView()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}
