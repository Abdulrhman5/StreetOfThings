namespace AdministrationGateway
{
    public class LoginDto
    {
        public string username { get; set; }

        public string password { get; set; }

        public string grant_type { get; set; }

        public string LoginInfo { get; set; }
    }
}