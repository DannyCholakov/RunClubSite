using RunClubSite.Interfaces;
using RunClubSite.Models;
using RunClubSite.ViewModels;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RunClubSite.Controllers
{

	public class DashboardController : Controller
	{
		private readonly IDashboardRepository _dashboardRespository;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IPhotoService _photoService;
		public DashboardController(IDashboardRepository dashboardRespository, IHttpContextAccessor httpContextAccessor, IPhotoService photoService)
		{
			_dashboardRespository = dashboardRespository;
			_httpContextAccessor = httpContextAccessor;
			_photoService = photoService;
		}
		private void MapUserEdit(AppUser user, EditUserDashboardViewModel editVM, ImageUploadResult photoResult)
		{
			user.Id = editVM.Id;
			user.Pace = editVM.Pace;
			user.Distance = editVM.Distance;
			user.ProfileImageUrl = photoResult.Url.ToString();
			user.City = editVM.City;
			user.State = editVM.State;
		}
		public async Task<IActionResult> Index()
		{
			var userRaces = await _dashboardRespository.GetAllUserRaces();
			var userClubs = await _dashboardRespository.GetAllUserClubs();
			var dashboardViewModel = new DashboardViewModel()
			{

				Clubs = userClubs,
				Races = userRaces
			};
			return View(dashboardViewModel);
		}
		public async Task<IActionResult> EditUserProfile()
		{
			var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
			var user = await _dashboardRespository.GetUserById(curUserId);
			if (user == null) return View("Error");
			var editUserViewModel = new EditUserDashboardViewModel()
			{
				Id = curUserId,
				Pace = user.Pace,
				Distance = user.Distance,
				ProfileImageUrl = user.ProfileImageUrl,
				City = user.City,
				State = user.State
			};
			return View(editUserViewModel);

		}
		[HttpPost]
		public async Task<IActionResult> EditUserProfile(EditUserDashboardViewModel editVM)
		{
			if (!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Failed to edit profile");
				return View("EditUserProfile", editVM);
			}
			AppUser user = await _dashboardRespository.GetByIdNoTracking(editVM.Id);
			if (user.ProfileImageUrl == "" || user.ProfileImageUrl == null)
			{
				var photoResult = await _photoService.AddPhotoAsync(editVM.Image);
				MapUserEdit(user, editVM, photoResult);
				_dashboardRespository.Update(user);
				return RedirectToAction("Index");
			}
			else
			{
				try
				{
					await _photoService.DeletePhotoAsync(user.ProfileImageUrl);
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", "Could not delete photo");
					return View(editVM);

				}

				var photoResult = await _photoService.AddPhotoAsync(editVM.Image);
				MapUserEdit(user, editVM, photoResult);
				_dashboardRespository.Update(user);
				return RedirectToAction("Index");
			}
		}

	}

}
