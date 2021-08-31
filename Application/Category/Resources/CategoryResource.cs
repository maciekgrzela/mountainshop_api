using System;
using Domain.Models;

namespace Application.Category.Resources
{
    public class CategoryResource : BaseDateTimeInfoEntry
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
    }
}