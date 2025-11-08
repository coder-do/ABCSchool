namespace ABCSharedLibrary.Models.Requests.User
{
    public class ChangeUserStatusRequest
    {
        public string UserId { get; set; }
        public bool Activation { get; set; }
    }
}
