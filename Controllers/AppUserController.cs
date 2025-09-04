using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieWeb.Models.System;
using MovieWeb.Services;
using System.Security.Claims;

namespace MovieWeb.Controllers
{
    [Authorize]

    public class AppUserController : Controller
    {
        private readonly IUserService _userService;

        public AppUserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            ViewData["BodyClass"] = "default-bg";
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
                return View(loginRequest);

            var loginResponse = await _userService.CheckUserCredidentals(loginRequest);

            if (loginResponse?.User != null)
            {
                var user = loginResponse.User;

                var roles = (user.Role ?? "User").Split(',');

                var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Name, user.Username),
    new Claim("FullName", user.FullName ?? user.Username)
};

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
                }


                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    authProperties
                );

                return RedirectToAction("Index", "Movie");
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View(loginRequest);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            ViewData["BodyClass"] = "default-bg";
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
                return View(registerRequest);

            bool registerSuccess = await _userService.TryRegisterRequest(registerRequest);

            if (registerSuccess)
                return RedirectToAction("Login", "AppUser");

            ModelState.AddModelError("", "Registration failed. Please try again.");
            return View(registerRequest);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> AdminUsers()
        {
            var users = await _userService.GetAllUsersAsync(); 
            return View(users); 
        }
    }
}
