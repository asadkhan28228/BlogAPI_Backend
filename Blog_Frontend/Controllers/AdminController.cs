
﻿using BlogMVC.Helpers;
using BlogMVC.Models.Users;
using BlogMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApiService _apiService;

        public AdminController(
            ApiService apiService)
        {
            _apiService = apiService;
        }

        // ==========================================================
        // ADMIN DASHBOARD
        // ==========================================================

        public IActionResult Dashboard()
        {
            if (!IsAdmin())
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            return View();
        }

        // ==========================================================
        // USERS
        // ==========================================================

        public async Task<IActionResult> Users()
        {
            if (!IsAdmin())
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            try
            {
                var users =
                    await _apiService
                        .GetAsync<List<UserViewModel>>(
                            "api/Users");

                return View(
                    users ??
                    new List<UserViewModel>());
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;

                return View(
                    new List<UserViewModel>());
            }
        }

        // ==========================================================
        // UPDATE ROLE
        // ==========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(
            int id,
            string role)
        {
            if (!IsAdmin())
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            if (id <= 0)
            {
                TempData["Error"] =
                    "Invalid user ID.";

                return RedirectToAction(
                    "Users");
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                TempData["Error"] =
                    "Role is required.";

                return RedirectToAction(
                    "Users");
            }

            try
            {
                await _apiService.PutAsync<string>(
                    $"api/Users/{id}/role",
                    new
                    {
                        role
                    });

                TempData["Success"] =
                    "User role updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }

            return RedirectToAction(
                "Users");
        }

        // ==========================================================
        // CHECK ADMIN
        // ==========================================================

        private bool IsAdmin()
        {
            var accessToken =
                HttpContext.Session.GetString(
                    SessionKeys.AccessToken);

            var role =
                HttpContext.Session.GetString(
                    SessionKeys.Role);

            return
                !string.IsNullOrWhiteSpace(
                    accessToken)
                &&
                string.Equals(
                    role,
                    "Admin",
                    StringComparison.OrdinalIgnoreCase);
        }
    }
}
