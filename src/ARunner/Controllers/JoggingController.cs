using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ARunner.BusinessLogic;
using ARunner.BusinessLogic.Filters;
using ARunner.BusinessLogic.Models;
using ARunner.BusinessLogic.Utils;
using ARunner.DataLayer;
using ARunner.DataLayer.Model;
using ARunner.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ARunner.Controllers
{
    [Authorize]
    public class JoggingController : Controller
    {
        private readonly IJoggingsRepository _repository;
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<User> _userManager;

        public JoggingController(IJoggingsRepository repository, IAccountRepository accountRepository, UserManager<User> userManager)
        {
            _repository = repository;
            _accountRepository = accountRepository;
            _userManager = userManager;
        }

        public IActionResult Home()
        {
            return View();
        }

        [HttpGet]
        [Route("joggings/{id}")]
        public IActionResult Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Id is mandatory");

            var jogging = _repository.GetJoggings().FirstOrDefault(j => j.Id == id);

            if (jogging == null)
                return NotFound("Entity not found");

            return Ok(Mapper.Map<JoggingViewModel>(jogging));
        }

        [HttpGet]
        [Route("joggings")]
        [Authorize]
        public async Task<PaggingCollection<JoggingViewModel>> Get(JoggingFilter filter)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return null;

            filter = filter ?? new JoggingFilter();

            if (user.Id != filter.UserId)
            {
                if (user.Access != UserAccess.Admin)
                    return null;
            }

            return filter.ApplyFilter(_repository.GetJoggings(), Mapper.Map<JoggingViewModel>);
        }

        [HttpPost]
        [Route("joggings")]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] JoggingViewModel model)
        {
            if(model.Created == DateTime.MinValue)
                return BadRequest("Date must be setted");

            if (model.Distance <= 0)
                return BadRequest("Distance must be positive");

            if(model.Time <= 0)
                return BadRequest("Time must be positive");

            var userReq = await _userManager.GetUserAsync(User);

            if (userReq == null)
                return BadRequest();

            if (userReq.Id != model.UserId)
            {
                if (userReq.Access != UserAccess.Admin)
                    return Forbid();
            }

            var entity = Mapper.Map<Jogging>(model);

            var user = _accountRepository.GetUsers().FirstOrDefault(u => u.Id == model.UserId);

            if (user == null)
                return BadRequest("User selected not found");

            entity.User = user;

            _repository.Add(entity);
            
            return Created($"joggings/{entity.Id}", Mapper.Map<JoggingViewModel>(entity));
        }
        
        [HttpPut]
        [Route("joggings")]
        public async Task<IActionResult> Edit([FromBody] JoggingViewModel model)
        {
            if (model == null)
                return BadRequest("Model is mandatory");

            var userReq = await _userManager.GetUserAsync(User);

            if (userReq == null)
                return BadRequest();

            if (userReq.Id != model.UserId)
            {
                if (userReq.Access != UserAccess.Admin)
                    return Forbid();
            }

            var entity = Mapper.Map<Jogging>(model);
            entity.User = _accountRepository.GetUsers().FirstOrDefault(u => u.Id == model.UserId);

            _repository.Save(entity);


            return Ok(entity);
        }

        [HttpDelete]
        [Route("joggings/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Id is mandatory");

            var jogging = _repository.GetJoggings().FirstOrDefault(j => j.Id == id);

            if (jogging == null)
                return NotFound("Entity not found");

            var userReq = await _userManager.GetUserAsync(User);

            if (userReq == null)
                return BadRequest();

            if (userReq.Id != jogging.User.Id)
            {
                if (userReq.Access != UserAccess.Admin)
                    return Forbid();
            }

            _repository.Delete(jogging);

            return Ok();
        }

        [HttpGet]
        [Route("/joggings/statistics/{userId}")]
        public IActionResult Statistics(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("UserId is mandatory");

            var joggings = _repository.GetJoggings().Where(j => j.User.Id == userId).ToList();
            
            var result = joggings.GroupBy(j => j.Created.Date.FirstDayOfWeek().Date)
                                 .Select(g =>
                                         {
                                             var first = g.First();
                                             return new JoggingWeekUserStatisticsModel
                                                    {
                                                        From = first.Created.FirstDayOfWeek(),
                                                        To = first.Created.LastDayOfWeek(),
                                                        RunTotal = g.Sum(j => j.Distance),
                                                        TimeSpendRunning = g.Sum(j => j.Time),
                                                        AverageSpeed = g.Average(j => j.Distance / j.Time)
                                                    };
                                         })
                                .OrderBy(model => model.From).ToList();

           //return Ok(new PaggingCollection<JoggingWeekUserStatisticsModel>(result, EntityFilter.Default));
            return Ok(result);

        }

    }
}
