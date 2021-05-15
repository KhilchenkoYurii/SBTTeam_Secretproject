using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PsychoWeb.ViewModels
{
    public class CustomerSettingView
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime BirthDate { get; set; }

        public List<IdentityRole> AllRoles { get; set; }

        public IList<string> UserRoles { get; set; }

        public CustomerSettingView()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}
