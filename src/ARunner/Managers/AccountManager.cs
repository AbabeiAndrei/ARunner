using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ARunner.BusinessLogic.Exceptions;
using ARunner.DataLayer.Model;
using ARunner.Repositories;
using ARunner.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ARunner.Managers
{
    public class AccountManager : IAccountManager
    {
        private readonly IAccountRepository _repository;
        private readonly IConfigurationRoot _configuration;
        private readonly ILogger<AccountManager> _logger;
        private readonly IPasswordHasher<User> _hasher;
        private readonly IUserTokenGenerator _tokenGenerator;

        public AccountManager(IAccountRepository repository, IConfigurationRoot configuration, ILogger<AccountManager> logger, IPasswordHasher<User> hasher, IUserTokenGenerator tokenGenerator)
        {
            _repository = repository;
            _configuration = configuration;
            _logger = logger;
            _hasher = hasher;
            _tokenGenerator = tokenGenerator;
        }

        public User Find(string email, string password)
        {
            var user = _repository.GetUsers().FirstOrDefault(u => u.Email == email);

            var passHahs = _hasher.HashPassword(user, password);

            if (user.PasswordHash == passHahs)
                user = null;

            if (user == null)
                throw new NotFoundException();

            if (user.State != UserState.Active)
                throw new InvalidStateException<UserState>(user.State);

            return user;
        }

        public bool EmailExists(string email, params string[] modelId)
        {
            return _repository.GetUsers().Any(u => u.Email == email && !modelId.Contains(u.Id));
        }

        public MailData GenerateActivationMail(User user)
        {
            var appRoot = _configuration["appHost"].TrimEnd('/');

            var mail = new MailData
            {
                ToName = user.FullName,
                ToAddress = user.Email,
                Subject = $"Account acctivation for {Settings.Settings.APPLICATION_NAME}",
                Body = $"Welcome to {Settings.Settings.APPLICATION_NAME} {Environment.NewLine}" +
                                  "Set password link : " + appRoot + "/Account/SetPassword?token=" + GenerateUserToken(user)
            };


            return mail;
        }

        public bool SetPassword(User user, string password)
        {
            user.PasswordHash = _hasher.HashPassword(user, password);
            _repository.Save(user);
            return true;
        }

        private string GenerateUserToken(User user)
        {
            try
            {
                var tokenEmail = _tokenGenerator.GenerateToken(user, "Email");
                var tokenPassword = _tokenGenerator.GenerateToken(user, "Pass");
                var result = tokenPassword + ":" + tokenEmail + ":" + user.Id;
                return WebUtility.UrlEncode(result);
            }
            catch (Exception mex)
            {
                _logger.LogError(mex.Message);
                throw;
            }
        }
    }
}
