using Domain.Models;

namespace Application.Product.Resources
{
    public class PropertyValueForProductResource
    {
        public Domain.Models.ProductsProperty ProductsProperty { get; set; }
        public string Value { get; set; }
    }
}