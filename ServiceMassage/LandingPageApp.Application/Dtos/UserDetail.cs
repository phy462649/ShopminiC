using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandingPageApp.Application.Dtos
{
    public class UserDetail
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool Status { get; set; }

        // Nếu là customer
        public CustomerDTO? Customer { get; set; }

        // Nếu là staff
        public StaffDTO? Staff { get; set; }
    }
}
