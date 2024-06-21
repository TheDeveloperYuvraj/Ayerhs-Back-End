namespace Ayerhs.Core.Entities.AccountManagement
{
    public class LoginResponseDto
    {
        public string? Token { get; set; }
        public Clients? Client { get; set; }
        public Dictionary<string, string>? Claims { get; set; }
    }
}
