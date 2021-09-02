namespace Application.Category.Params
{
    public class CategoryParams : PagingParams
    {
        public string NameFilter { get; set; }
        public bool? NameSort { get; set; }
        public string DescriptionFilter { get; set; }
        public bool? DescriptionSort { get; set; }
    }
}