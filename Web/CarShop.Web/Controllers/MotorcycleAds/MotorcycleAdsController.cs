﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarShop.Data.Models;
using CarShop.Data.Models.Ads;
using CarShop.Data.Models.Images;
using CarShop.Services.Mapping;
using CarShop.Services.MotorcycleAds;
using CarShop.Web.ViewModels.MotorcyclesAds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers.MotorcycleAds
{
    public class MotorcycleAdsController : Controller
    {
        private readonly IMotorcycleAdsService motorcycleAdsService;
        private readonly UserManager<ApplicationUser> userManager;

        public MotorcycleAdsController(IMotorcycleAdsService motorcycleAdsService, UserManager<ApplicationUser> userManager)
        {
            this.motorcycleAdsService = motorcycleAdsService;
            this.userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var user = await userManager.GetUserAsync(this.User);
            var viewModel = new CreateMotorcycleAdViewModel();

            if (user.PhoneNumber != null)
            {
                viewModel.PhoneNumber = user.PhoneNumber;
            }

            if (user.PhoneNumber2 != null)
            {
                viewModel.PhoneNumber2 = user.PhoneNumber2;
            }

            if (user.PhoneNumber3 != null)
            {
                viewModel.PhoneNumber3 = user.PhoneNumber3;
            }

            return this.View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(CreateMotorcycleAdViewModel createMotorcycleAdModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(createMotorcycleAdModel);
            }

            await this.motorcycleAdsService.CreateAsync(createMotorcycleAdModel);

            return this.RedirectToAction("MyMotorcycleAds");
        }

        [Authorize]
        public async Task<IActionResult> MyMotorcycleAds()
        {
            return this.View();
        }

        [Authorize]
        [HttpGet("/MotorcycleAds/Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var truckAd = await this.motorcycleAdsService.GetByIdAsync(id);
            var viewModel = AutoMapperConfig.MapperInstance.Map<EditMotorcycleAdViewModel>(truckAd);

            return this.View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(EditMotorcycleAdViewModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            await this.motorcycleAdsService.EditAsync(inputModel);

            return this.RedirectToAction("MyMotorcycleAds");
        }

        [Authorize]
        [HttpGet("/MotorcycleAds/Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await this.motorcycleAdsService.DeleteAsync(id);

            var user = await this.userManager.GetUserAsync(this.User);
            if (await this.userManager.IsInRoleAsync(user, "Administrator"))
            {
                return this.RedirectToAction("Search");
            }

            return this.RedirectToAction("MyMotorcycleAds");
        }

        [Authorize]
        [HttpGet("/MotorcycleAds/Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var motorcycleAd = await this.motorcycleAdsService.GetByIdAsync(id);
            var viewModel = AutoMapperConfig.MapperInstance.Map<MotorcycleAdDetailsViewModel>(motorcycleAd);

            return this.View(viewModel);
        }

        public async Task<IActionResult> Search()
        {
            var motorcycleAds = new List<MotorcycleAd>();
            if (this.User.Identity.Name != null)
            {
                var user = await this.userManager.GetUserAsync(this.User);
                motorcycleAds = (List<MotorcycleAd>)await this.motorcycleAdsService.GetAllWithoutYoursAsync(user.Id);

                return this.View(motorcycleAds);
            }

            motorcycleAds = (List<MotorcycleAd>)await this.motorcycleAdsService.GetAllWithoutYoursAsync(null);

            return this.View(motorcycleAds);
        }
    }
}