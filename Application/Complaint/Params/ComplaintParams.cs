using System;

namespace Application.Complaint.Params
{
    public class ComplaintParams : PagingParams
    {
        public Guid? OrderFilter { get; set; }
        public bool? OrderSort { get; set; }
        public int? NumberFilter { get; set; }
        public bool? NumberSort { get; set; }
        public string AbbreviationFilter { get; set; }
        public bool? AbbreviationSort { get; set; }
        public string DescriptionFilter { get; set; }
        public bool? DescriptionSort { get; set; }
    }
}