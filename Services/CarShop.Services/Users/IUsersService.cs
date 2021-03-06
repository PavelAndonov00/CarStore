﻿using CarShop.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

    namespace CarShop.Services.Users
{
    public interface IUsersService
    {
        Task<string> GetUserIdByUsernameAsync(string userName);

        Task<bool> CheckForUniqueUsernameAsync(string userName);

        Task<bool> CheckForUniqueEmail(string email);

        Task DeleteUserAsync(ApplicationUser user);

        Task PersistChangedPersonalData(ApplicationUser user, string firstName, string lastName, string country, string city, string phone, string phone2, string phone3);

    }
}
