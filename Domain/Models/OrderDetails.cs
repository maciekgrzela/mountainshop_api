using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class OrderDetails
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        [MaxLength(200)]
        public string AddressLineOne { get; set; }
        [MinLength(6), MaxLength(6)]
        public string PostalCode { get; set; }
        [MaxLength(200)]
        public string Place { get; set; }
        [MaxLength(150)]
        public string Country { get; set; }
        [MinLength(11), MaxLength(11)]
        public string PhoneNumber { get; set; }
        [MaxLength(1000)]
        public string CompanyName { get; set; }
        [MinLength(10), MaxLength(10)]
        public string CompanyNip { get; set; }
        [MaxLength(200)]
        public string CompanyAddressLineOne { get; set; }
        [MinLength(6), MaxLength(6)]
        public string CompanyPostalCode { get; set; }
        [MaxLength(200)]
        public string CompanyPlace { get; set; }
        [MaxLength(150)]
        public string CompanyCountry { get; set; }
        [MinLength(11), MaxLength(11)]
        public string CompanyPhoneNumber { get; set; }
    }
}