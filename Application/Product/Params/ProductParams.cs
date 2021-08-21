using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Product.Params
{
    public class ProductParams : PagingParams
    {
        public string NameFilter { get; set; }
        public string DescriptionFilter { get; set; }
        public List<string> GenderFilter { get; set; }
        public bool? BestRatingDesc { get; set; }
        public bool? CommentsCountDesc { get; set; }
        public bool? TheNewFilter { get; set; }
        public bool? SaleFilter { get; set; }
        public int? AmountInStorageConstraint { get; set; }
        public double? GrossPriceMinFilter { get; set; }
        public double? GrossPriceMaxFilter { get; set; }
        public bool? GrossPriceAsc { get; set; }
        public bool? GrossPriceDesc { get; set; }
        public List<Guid> ProducerIds { get; set; }
        public Guid CategoryId { get; set; }
    }
}