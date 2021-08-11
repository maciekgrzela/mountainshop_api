using System;

namespace Application.Complaint.Resources
{
    public class ComplaintResource
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
        public OrderForComplaintResource Order { get; set; }
    }
}