namespace inOffice.BusinessLogicLayer.Requests
{
    public class MicrosoftUser
    {
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string JobTitle { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
