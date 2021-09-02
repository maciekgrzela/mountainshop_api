namespace Application.Producer.Params
{
    public class ProducerParams : PagingParams
    {
        public string NameFilter { get; set; }
        public bool? NameSort { get; set; }
        public string DescriptionFilter { get; set; }
        public bool? DescriptionSort { get; set; }
    }
}