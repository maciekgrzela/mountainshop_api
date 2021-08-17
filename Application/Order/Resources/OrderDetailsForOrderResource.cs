using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Order.Resources
{
    public class OrderDetailsForOrderResource
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserFirstname { get; set; }
        public string UserLastname { get; set; }
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
}