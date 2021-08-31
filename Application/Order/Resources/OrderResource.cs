using System;
using System.Collections.Generic;

namespace Application.Order.Resources
{
    public class OrderResource
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public OrderDetailsForOrderResource OrderDetails { get; set; }
        public PaymentMethodForOrderResource PaymentMethod { get; set; }
        public DeliveryMethodForOrderResource DeliveryMethod { get; set; }
        public List<OrderedProductForOrderResource> OrderedProducts { get; set; }
        public string Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime WarrantyIsInForce { get; set; }
    }

    public class OrderDetailsForOrderResource
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string AddressLineOne { get; set; }
        public string PostalCode { get; set; }
        public string Place { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNip { get; set; }
        public string CompanyAddressLineOne { get; set; }
        public string CompanyPostalCode { get; set; }
        public string CompanyPlace { get; set; }
        public string CompanyCountry { get; set; }
        public string CompanyPhoneNumber { get; set; }
    }

    public class PaymentMethodForOrderResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public bool ExternalApi { get; set; }
    }

    public class DeliveryMethodForOrderResource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }

    public class OrderedProductForOrderResource
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public double ProductGrossPrice { get; set; }
        public double ProductPercentageSale { get; set; }
        public double Amount { get; set; }
    }
}