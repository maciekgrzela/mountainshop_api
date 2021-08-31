using System.Collections.Generic;

namespace Application.Product.Validators
{
    public static class ProductsCustomValidators
    {
        public static bool GenderIsValid(string gender)
        {
            var validList = new List<string> {"male", "female", "unisex"};
            return validList.Contains(gender);
        }
    }
}