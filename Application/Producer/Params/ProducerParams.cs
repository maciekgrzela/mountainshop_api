namespace Application.Producer.Params
{
    public class ProducerParams : PagingParams
    {
        public string NameFilter { get; set; }
        public bool? NameAsc { get; set; }
        public bool? NameDesc { get; set; }
        public string DescriptionFilter { get; set; }
        public bool? DescriptionAsc { get; set; }
        public bool? DescriptionDesc { get; set; }
    }
}