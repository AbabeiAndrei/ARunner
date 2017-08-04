using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARunner.DataLayer.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ARunner.DataLayer.Initial
{
    public class ARunnerContextSeed : IContextSeed
    {
        private readonly ADbContext _context;
        private readonly UserManager<User> _userManager;

        public ARunnerContextSeed(ADbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task ApplicationSeed()
        {

            if(_context.Users.Any())
                return;

            var usr = new User
                      {
                          Access = UserAccess.Admin,
                          CreatedAt = DateTime.UtcNow,
                          Email = "ababeiandrei94@gmail.com",
                          FullName = "Andrei Ababei",
                          PasswordHash = "2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824",
                          RowState = RowState.Created,
                          State = UserState.Active,
                          Metadata = JsonConvert.SerializeObject(new UserSettings
                                                                 {
                                                                     Language = "en-US",
                                                                     Units = Units.Metric,
                                                                     TemporaryPassword = true
                                                                 })
            };

            await _userManager.CreateAsync(usr,"P@sW0rD!");

            await _context.SaveChangesAsync();
        }

        public async Task ApplyMigrations()
        {
            await _context.Database.MigrateAsync();
        }
    }
}
