namespace inOffice.BusinessLogicLayer.Responses
{
    public class AllReviewsResponse
    {
        public List<CustomReviews>? ListOfReviews { get; set; }
        public bool Success { get; set; }   
    }

    public class CustomReviews
    {
        public string? Review { get; set; }
        public string? ReviewOutput { get; set; }
        public int? DeskIndex { get; set; }
        public string? OfficeName { get; set; }
    }
}
