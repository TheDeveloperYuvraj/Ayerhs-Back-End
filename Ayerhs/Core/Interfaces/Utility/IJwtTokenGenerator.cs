namespace Ayerhs.Core.Interfaces.Utility
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(string userId, string username, string role);
    }
}
