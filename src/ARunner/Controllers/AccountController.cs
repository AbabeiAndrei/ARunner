using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using ARunner.BusinessLogic;
using ARunner.BusinessLogic.Exceptions;
using ARunner.BusinessLogic.Filters;
using ARunner.BusinessLogic.Models;
using ARunner.DataLayer.Model;
using ARunner.Managers;
using ARunner.Repositories;
using ARunner.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ARunner.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountManager _manager;
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountRepository _repository;
        private readonly IMailSender _mailSender;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserTokenGenerator _tokenGenerator;

        public AccountController(IAccountManager manager, ILogger<AccountController> logger, IAccountRepository repository, IMailSender mailSender, SignInManager<User> signInManager, UserManager<User> userManager, IUserTokenGenerator tokenGenerator)
        {
            _manager = manager;
            _logger = logger;
            _repository = repository;
            _mailSender = mailSender;
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
        }
        
        [HttpGet]
        [AllowAnonymous]
        [Route("Account/Login")]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");
            
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Account/Register")]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return Redirect("/");

            return View("Register");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Account/Login")]
        public async Task<ActionResult> Login([FromBody] LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Model state is invalid");
                return View();
            }

            if (model == null)
            {
                _logger.LogWarning("Post : model is null");
                return BadRequest("Something wrong happen, please try again");
            }

            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                _logger.LogInformation("Post : Email or Password is null", model.Email, model.Password);
                return BadRequest("Email or password cannot be emopty");
            }

            try
            {
                var user = _manager.Find(model.Email, model.Password);

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                
                if (!result.Succeeded)
                    return View();

                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return Ok(Mapper.Map<UserModel>(user));
            }
            catch (NotFoundException ex)
            {
                _logger.LogInformation("User not found", ex);
                ModelState.AddModelError(ModelErrorStates.USER_NOT_FOUND, "User or password is incorect");
            }
            catch (InvalidStateException<UserState> ex)
            {
                _logger.LogInformation("Invalid user state", ex);

                string message;

                switch (ex.State)
                {
                    case UserState.Suspended:
                        message = "User is suspended";
                        break;
                    case UserState.Invited:
                        message = "User is not acctivated";
                        break;
                    default:
                        message = "User is in an invalid state. Please try again later, or contact an administrator";
                        break;
                }
                
                ModelState.AddModelError(ModelErrorStates.USER_NOT_FOUND, message);
            }

            return BadRequest(ModelState);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Account/Logout")]
        public async Task<ActionResult> Logout()
        {
            await ExecuteLogout();

            return Redirect("/Account/Login");
        }

        private async Task ExecuteLogout()
        {
            if (User.Identity.IsAuthenticated)
                await _signInManager.SignOutAsync();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Account/Register")]
        public async Task<ActionResult> Register([FromBody] RegisterViewModel model)
        {
            await ExecuteLogout();
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("Model state is invalid");
                return View();
            }

            if (model == null)
            {
                _logger.LogWarning("Post : model is null");
                return BadRequest("Something wrong happen, please try again");
            }

            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.FullName))
            {
                _logger.LogInformation("Post : Email or FullName is null", model.Email, model.FullName);
                return BadRequest("Email or FullName cannot be emopty");
            }

            var user = new User(model.Email)
                       {
                           Access = UserAccess.Regular,
                           Email = model.Email,
                           FullName = model.FullName,
                           CreatedAt = DateTime.Now,
                           State = UserState.Invited,
                           SecurityStamp = Guid.NewGuid().ToString()
                       };

            try
            {
                //var result = await _userManager.CreateAsync(user);

                //if (!result.Succeeded)
                //    return BadRequest(result.Errors);

                _repository.Add(user);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
            try
            {
                var mail = _manager.GenerateActivationMail(user);

                await _mailSender.SendMail(mail);
            }
            catch (Exception mex)
            {
                _logger.LogError("Activation mail not send", mex.Message);
            }

            return Redirect("/Account/Login");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Account/SetPassword")]
        public async Task<ActionResult> SetPassword(string token)
        {
            await ExecuteLogout();
            if (string.IsNullOrEmpty(token))
                return BadRequest("Invalid token");

            var decodedToken = WebUtility.UrlDecode(token);

            var parts = decodedToken.Split(':');

            if (parts.Length != 3)
                return BadRequest("Invalid token");

            var uid = parts[2];

            var user = _repository.GetUsers().FirstOrDefault(u => u.Id == uid);

            if(user == null)
                return BadRequest("Invalid token");

            if (user.State > UserState.Invited)
                return Redirect("/Acount/Login");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Account/SetPassword")]
        public async Task<ActionResult> SetPassword([FromBody] SetPasswordModel model)
        {
            await ExecuteLogout();
            if (string.IsNullOrEmpty(model.Token))
                return BadRequest("Invalid token");

            var decodedToken = WebUtility.UrlDecode(model.Token);

            var parts = decodedToken.Split(':');

            if (parts.Length != 3)
                return BadRequest("Invalid token");

            var uid = parts[2];

            var user = _repository.GetUsers().FirstOrDefault(u => u.Id == uid);

            if (user == null)
                return BadRequest("Invalid token");

            if (user.State > UserState.Invited)
                return BadRequest("User already active");

            try
            {
                if (!_tokenGenerator.ValidateToken(user, "Email", parts[1]))
                    return BadRequest("Invalid token");

                user.EmailConfirmed = true;

                if(!_tokenGenerator.ValidateToken(user, "Pass", parts[0]))
                    return BadRequest("Invalid token");

                var result = _manager.SetPassword(user, model.Password);
                
                if (result)
                {
                    user.State = UserState.Active;

                    _repository.Save(user);

                    var res = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
                    if (res.Succeeded)
                        return Redirect("/");

                    return Redirect("/Account/Login");
                }
            }
            catch (Exception mex)
            {
                _logger.LogError(mex.Message);
                return BadRequest();
            }

            return BadRequest("Invalid token");
        }

        [HttpGet]
        [Route("users/{id}")]
        public IActionResult Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Id is mandatory");

            var user = _repository.GetUsers().FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound("User not found");

            return Ok(Mapper.Map<UserModel>(user));
        }

        [HttpGet]
        [Route("users")]
        [Authorize(Policy = "Manager")]
        public PaggingCollection<UserModel> Get(UserFilter filter)
        {
            return filter.ApplyFilter(_repository.GetUsers(), Mapper.Map<UserModel>);
        }

        [HttpPost]
        [Route("users")]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Add([FromBody] UserModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest("Email is not valid");

            if (string.IsNullOrEmpty(model.FullName))
                return BadRequest("Full name is required");

            if (_manager.EmailExists(model.Email))
                return BadRequest("Email already exists");

            var entity = Mapper.Map<User>(model);

            entity.CreatedAt = DateTime.Now;

            await _userManager.CreateAsync(entity);

            var mail = _manager.GenerateActivationMail(entity);

            await _mailSender.SendMail(mail);

            return Created($"users/{entity.Id}", Mapper.Map<UserModel>(entity));
        }

        [HttpPut]
        [Route("users")]
        [Authorize(Policy = "Manager")]
        public IActionResult Edit([FromBody] UserModel model)
        {
            if (model == null)
                return BadRequest("Model is mandatory");

            if (string.IsNullOrEmpty(model.Email))
                return BadRequest("Email is not valid");

            if (string.IsNullOrEmpty(model.FullName))
                return BadRequest("Full name is required");

            if (_manager.EmailExists(model.Email, model.Id))
                return BadRequest("Email already exists");

            var entity = Mapper.Map<User>(model);

            _repository.Save(entity);

            return Ok(entity);
        }

        [HttpDelete]
        [Route("users/{id}")]
        [Authorize(Policy = "Manager")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Id is mandatory");

            var user = _repository.GetUsers().FirstOrDefault(j => j.Id == id);

            if (user == null)
                return NotFound("Entity not found");

            _repository.Delete(user);
            await _userManager.DeleteAsync(user);

            return Ok();
        }

        [HttpPut]
        [Route("users/{id}/suspend")]
        [Authorize(Policy = "Manager")]
        public IActionResult Suspend(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Id is mandatory");

            var user = _repository.GetUsers().FirstOrDefault(j => j.Id == id);

            if (user == null)
                return NotFound("Entity not found");

            user.State = UserState.Suspended;

            _repository.Save(user);

            return Ok();
        }
        
        [HttpPut]
        [Route("users/{id}/resetPassword")]
        [Authorize(Policy = "Manager")]
        public IActionResult ResetPassword(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Id is mandatory");

            var user = _repository.GetUsers().FirstOrDefault(j => j.Id == id);

            if (user == null)
                return NotFound("Entity not found");

            //todo reset password
            //_mailSender.SendMail();

            return Ok();
        }

        [HttpGet]
        [Route("users/sugestions/{name}")]
        [Authorize(Policy = "Admin")]
        public IActionResult UserSuggestions(string name)
        {
            var result = _repository.GetUsers().Where(u => u.FullName.Contains(name) || u.Email.Contains(name) || u.UserName.Contains(name))
                                    .Take(10)
                                    .ToList()
                                    .Select(Mapper.Map<UserBaseModel>);

            return Ok(result);
        }
    }
}
