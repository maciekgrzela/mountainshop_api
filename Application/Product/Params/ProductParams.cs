using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Product.Params
{
    public class ProductParams : PagingParams
    {
        public string NameFilter { get; set; }
        public string DescriptionFilter { get; set; }
        public int? AmountInStorageConstraint { get; set; }
        public double? GrossPriceMinFilter { get; set; }
        public double? GrossPriceMaxFilter { get; set; }
        public bool? GrossPriceAsc { get; set; }
        public bool? GrossPriceDesc { get; set; }
        public Guid ProducerId { get; set; }
        public Guid CategoryId { get; set; }
    }
}