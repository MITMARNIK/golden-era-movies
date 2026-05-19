using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoldenEraMovies.Models;
using GoldenEraMovies.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoldenEraMovies.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        // Listare toti utilizatorii si rolul lor
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            var modelList = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var roles = await _userService.GetUserRolesAsync(user);
                modelList.Add(new UserListViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName ?? "N/A",
                    Email = user.Email ?? "No Email",
                    ProfilePictureUrl = user.ProfilePictureUrl ?? "",
                    Role = roles.FirstOrDefault() ?? "None"
                });
            }

            return View(modelList);
        }

        // Schimbare rol utilizator
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(int userId, string newRole)
        {
            // Protect current user from losing Admin role to prevent self-lockout
            var currentUser = await _userService.GetUserByUsernameAsync(User.Identity.Name);
            if (currentUser != null && currentUser.Id == userId)
            {
                TempData["ErrorMessage"] = "You cannot change your own administrator role to prevent accidental lockout!";
                return RedirectToAction(nameof(Index));
            }

            if (newRole != "Admin" && newRole != "User")
            {
                TempData["ErrorMessage"] = "Invalid role specified.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userService.ChangeUserRoleAsync(userId, newRole);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Role updated successfully to {newRole}!";
            }
            else
            {
                TempData["ErrorMessage"] = string.Join(" ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
