using System;
using System.Collections.Generic;

namespace Application.Product.Resources
{
    public class ProductWithCommentsResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int AmountInStorage { get; set; }
        public double NetPrice { get; set; }
        public double PercentageTax { get; set; }
        public double GrossPrice { get; set; }
        public string Gender { get; set; }
        public double? AverageCommentsRate { get; set; }
        public double? PercentageSale { get; set; }
        public int MinimalOrderedAmount { get; set; }
        public DateTime Created { get; set; }
        public ProducerForProductResource Producer { get; set; }
        public CategoryForProductResource Category { get; set; }
        public List<CommentForProductResource> Comments { get; set; }
    }
}