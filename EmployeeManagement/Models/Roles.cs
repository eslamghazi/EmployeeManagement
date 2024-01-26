using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EmployeeManagement.Models
{
    public class Roles
    {
        public static string SuperAdmin { get { return "Super Admin"; } }
        public static string Admin { get { return "Admin"; } }
        public static string User { get { return "User"; } }

        public static List<SelectListItem> RolesList { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Super Admin", Text = "Super Admin" },
            new SelectListItem { Value = "Admin", Text = "Admin" },
            new SelectListItem { Value = "User", Text = "User"    },
         };

    }

}
