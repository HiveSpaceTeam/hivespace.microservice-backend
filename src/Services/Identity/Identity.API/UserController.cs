//using Duende.IdentityModel;
//using Duende.IdentityServer.Services;
//using Duende.IdentityServer.Stores;
//using Humanizer;
//using Identity.API.Models;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System.Net;
//using System.Security.Claims;

//namespace Identity.API;

//[Route("api/v1/users")]
//[ApiController]
//public class AccountController : Controller
//{
//    private readonly UserManager<ApplicationUser> _userManager;
//    private readonly SignInManager<ApplicationUser> _signInManager;
//    private readonly IIdentityServerInteractionService _interaction;
//    private readonly IClientStore _clientStore;
//    private readonly IAuthenticationSchemeProvider _schemeProvider;
//    private readonly IAuthenticationHandlerProvider _handlerProvider;
//    private readonly IEventService _events;

//    public AccountController(
//        UserManager<ApplicationUser> userManager,
//        SignInManager<ApplicationUser> signInManager,
//        IIdentityServerInteractionService interaction,
//        IClientStore clientStore,
//        IAuthenticationSchemeProvider schemeProvider,
//        IAuthenticationHandlerProvider handlerProvider,
//        IEventService events)
//    {
//        _userManager = userManager;
//        _signInManager = signInManager;
//        _interaction = interaction;
//        _clientStore = clientStore;
//        _schemeProvider = schemeProvider;
//        _handlerProvider = handlerProvider;
//        _events = events;
//    }

//    //[HttpPost("signup")]
//    //[ProducesResponseType((int)HttpStatusCode.OK)]
//    //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
//    //public async Task<IActionResult> Signup([FromBody] CreateUserRequestDto requestDto)
//    //{
//    //    var result = await _userService.CreateUserAsync(requestDto);
//    //    return Ok(result);
//    //}

//    [HttpPost("login")]
//    [ProducesResponseType((int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
//    public async Task<IActionResult> Login([FromBody] LoginRequestDto requestDto)
//    {
//        var user = await _userManager.FindByNameAsync(requestDto.UserName);
//        if (user != null && await _userManager.CheckPasswordAsync(user, requestDto.Password))
//        {
//            // Step 3: Sign in the user using IdentityServer cookie auth
//            var claims = new List<Claim>
//        {
//            new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
//            new Claim(JwtClaimTypes.Name, user.UserName),
//            // You can add email, roles, etc. as needed
//        };

//            var identity = new ClaimsIdentity(claims, "password");
//            var principal = new ClaimsPrincipal(identity);

//            await HttpContext.SignInAsync("Identity.Application", principal);

//            // Step 4: Redirect back to IdentityServer to continue OIDC flow
//            var redirectUrl = $"/connect/authorize?client_id=vue-client" +
//                              $"&redirect_uri={Uri.EscapeDataString(dto.ReturnUrl)}" +
//                              $"&response_type=code" +
//                              $"&scope=openid profile api1" +
//                              $"&state=xyz";

//            return Ok(new { redirect = redirectUrl });
//        }

//        return Unauthorized();
//    }
//}

//public class LoginRequestDto
//{
//    public string PhoneNumber { get; set; } = string.Empty;
//    public string UserName { get; set; } = string.Empty;
//    public string Password { get; set; } = string.Empty;
//}