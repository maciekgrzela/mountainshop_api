using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class Producer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public List<Product> Products { get; set; }
    }
}