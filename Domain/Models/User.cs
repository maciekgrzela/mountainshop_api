using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models
{
    public class User : IdentityUser
    {
        [MaxLength(150)]
        public string FirstName { get; set; }
        [MaxLength(200)]
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Image { get; set; }
        public List<Comment> Comments { get; set; } 
    }
}