namespace ABCSharedLibrary.Models.Requests.School
{
    public class UpdateSchoolRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EstablishedDate { get; set; }
    }
}
