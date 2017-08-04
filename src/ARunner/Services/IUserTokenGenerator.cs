using ARunner.DataLayer.Model;

namespace ARunner.Services
{
    public interface IUserTokenGenerator
    {
        string GenerateToken(User user, string reason);
        bool ValidateToken(User user, string reason, string token);
    }
}