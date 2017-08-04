using System.Threading.Tasks;
using ARunner.DataLayer.Model;
using ARunner.Services;
using Microsoft.AspNetCore.Identity;

namespace ARunner.Managers
{
    public interface IAccountManager
    {
        User Find(string email, string password);
        bool EmailExists(string email, params string[] exceptIds);
        MailData GenerateActivationMail(User user);
        bool SetPassword(User user, string password);
    }
}