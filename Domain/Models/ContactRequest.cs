using System;

namespace Domain.Models
{
    public class ContactRequest
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
    }
}