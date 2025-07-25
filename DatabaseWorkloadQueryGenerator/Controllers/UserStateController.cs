using DatabaseWorkloadQueryGenerator.Core.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseWorkloadQueryGenerator.Controllers
{

    [Route("[controller]/[action]")]
    [ApiController]
    public class UserStateController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly ICacheService _cache;

        public UserStateController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager/*, ICacheService cache*/)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            //_cache = cache;
        }
        public async Task<IActionResult> Logout()
        {
            var UserId = _userManager.GetUserId(User);
            //if (!string.IsNullOrEmpty(UserId))
            //    await _cache.RemoveAsync($"user:{UserId}");

            await _signInManager.SignOutAsync();
            return LocalRedirect("/");
        }

        public async Task<IActionResult> RefreshUser(string returnUrl)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return LocalRedirect("/");
            }

            await _signInManager.RefreshSignInAsync(user);

            //await _cache.RemoveAsync($"user:{user.Id}");
            //await _cache.SetAsync($"user:{user.Id}", user);

            return LocalRedirect(returnUrl);
        }
    }
}
