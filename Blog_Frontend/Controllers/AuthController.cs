
﻿using BlogMVC.Helpers;
using BlogMVC.Models.Auth;
using BlogMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;

        public AuthController(
            ApiService apiService)
        {
            _apiService = apiService;
        }

        // ==========================================================
        // LOGIN GET
        // ==========================================================

        [HttpGet]
        public IActionResult Login()
        {
            if (IsLoggedIn())
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            return View();
        }

        // ==========================================================
        // LOGIN POST
        // ==========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result =
                    await _apiService
                        .PostAsync<AuthResponseModel>(
                            "api/Auth/login",
                            new
                            {
                                email = model.Email,

                                password = model.Password
                            });

                if (result == null ||
                    string.IsNullOrWhiteSpace(
                        result.Token))
                {
                    ModelState.AddModelError(
                        "",
                        "Login failed.");

                    return View(model);
                }

                // ==================================================
                // SAVE SESSION
                // ==================================================

                HttpContext.Session.SetString(
                    SessionKeys.AccessToken,
                    result.Token);

                HttpContext.Session.SetString(
                    SessionKeys.RefreshToken,
                    result.RefreshToken);

                HttpContext.Session.SetInt32(
                    SessionKeys.UserId,
                    result.UserId);

                HttpContext.Session.SetString(
                    SessionKeys.Username,
                    result.Username);

                HttpContext.Session.SetString(
                    SessionKeys.Email,
                    result.Email);

                // ==================================================
                // SAVE ROLE
                // ==================================================

                HttpContext.Session.SetString(
                    SessionKeys.Role,
                    string.IsNullOrWhiteSpace(
                        result.Role)
                        ? "User"
                        : result.Role);

                TempData["Success"] =
                    "Login successful.";

                // ==================================================
                // ADMIN DIRECTLY ADMIN DASHBOARD
                // ==================================================

                if (string.Equals(
                    result.Role,
                    "Admin",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction(
                        "Dashboard",
                        "Admin");
                }

                return RedirectToAction(
                    "Index",
                    "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    ex.Message);

                return View(model);
            }
        }

        // ==========================================================
        // REGISTER GET
        // ==========================================================

        [HttpGet]
        public IActionResult Register()
        {
            if (IsLoggedIn())
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            return View();
        }

        // ==========================================================
        // REGISTER POST
        // ==========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // ==================================================
                // IMPORTANT:
                // Role frontend se send nahi ho raha.
                // Backend automatically User banata hai.
                // ==================================================

                await _apiService
                    .PostAsync<AuthResponseModel>(
                        "api/Auth/register",
                        new
                        {
                            username =
                                model.Username,

                            email =
                                model.Email,

                            password =
                                model.Password
                        });

                TempData["Success"] =
                    "Registration successful. Please login.";

                return RedirectToAction(
                    "Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "",
                    ex.Message);

                return View(model);
            }
        }

        // ==========================================================
        // LOGOUT
        // ==========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var refreshToken =
                HttpContext.Session.GetString(
                    SessionKeys.RefreshToken);

            if (!string.IsNullOrWhiteSpace(
                refreshToken))
            {
                try
                {
                    await _apiService
                        .PostAsync<string>(
                            "api/Auth/logout",
                            new
                            {
                                refreshToken
                            });
                }
                catch
                {
                    // Local logout still happens.
                }
            }

            HttpContext.Session.Clear();

            TempData["Success"] =
                "Logout successful.";

            return RedirectToAction(
                "Index",
                "Home");
        }

        // ==========================================================
        // CHECK LOGIN
        // ==========================================================

        private bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(
                HttpContext.Session.GetString(
                    SessionKeys.AccessToken));
        }
    }
}
