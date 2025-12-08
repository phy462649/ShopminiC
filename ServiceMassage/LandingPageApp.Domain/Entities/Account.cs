using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandingPageApp.Domain.Entities
{
    public class Account
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Relationships
        public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
        public virtual ICollection<Customer> Customer { get; set; } = new List<Customer>();

    }

}
