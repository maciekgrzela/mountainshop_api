using System;

namespace Application.Complaint.Params
{
    public class ComplaintParams : PagingParams
    {
        public Guid OrderId { get; set; }
    }
}