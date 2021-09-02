namespace Application.Contact.Params
{
    public class ContactRequestParams : PagingParams
    {
        public string FirstNameFilter { get; set; }
        public bool? FirstNameSort { get; set; }
        public string LastNameFilter { get; set; }
        public bool? LastNameSort { get; set; }
        public string EmailFilter { get; set; }
        public bool? EmailSort { get; set; }
        public string ContentFilter { get; set; }
        public bool? ContentSort { get; set; }
    }
}