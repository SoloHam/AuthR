using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static CustomAuthProvider;

public class HomeController : Controller
{
    public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Login()
    {
        return View();
    }
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string username, string password)
    {
        var user = new IdentityUser()
        {
            UserName = username
        };

        var created = await _userManager.CreateAsync(user, password);
        await _userManager.AddClaimAsync(user, new Claim("i", "I'm authorized by nani"));
        await _userManager.AddClaimAsync(user, new Claim(DynamicPolicies.SecurityLevel, "2"));

        if (created.Succeeded)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var redirectUri = Url.Action(nameof(VerifyEmail), new { userId = user.Id, token });
            return Content(redirectUri.ToString());
        }

        return StatusCode(500);
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return NotFound();
        }

        var signIn = await _signInManager.PasswordSignInAsync(user, password, true, false);

        if (signIn.Succeeded)
        {
            return RedirectToAction(nameof(Secret));
        }

        return BadRequest("Invalid Credentials");
    }

    public async Task<IActionResult> VerifyEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var res = await _userManager.ConfirmEmailAsync(user, token);
        // if(res.)
        return RedirectToAction(nameof(Login));
    }
    public async Task<IActionResult> Authenticate()
    {
        var claims = new List<Claim>(){
            new Claim("Email", "i@dent.ity")
        };
        var identity = new ClaimsIdentity(claims, "main");
        var principal = new ClaimsPrincipal(new[] { identity });

        await HttpContext.SignInAsync("IdentityCookie", principal);

        return Ok();
    }

    [Authorize(Policy = "NaniPolicy")]
    public async Task<IActionResult> Secret()
    {
        return await Task.FromResult(View());
    }

    [SecurityLevel(1)]
    public async Task<IActionResult> SecretOne()
    {
        return await Task.FromResult(Content("Wohoo! Newbie..."));
    }
    [SecurityLevel(3)]
    public async Task<IActionResult> SecretTwo()
    {
        return await Task.FromResult(Content("Tryna get serious"));
    }
    [SecurityLevel(10)]
    public async Task<IActionResult> SecretThree()
    {
        return await Task.FromResult(Content("Greetings... sensei."));
    }
}