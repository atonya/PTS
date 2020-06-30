using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using PublicTalkSchedule.Data;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        ////private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            ////IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            ////_emailSender = emailSender;
            _db = db;
            _roleManager = roleManager;        }

        [BindProperty]
        public InputModel Input { get; set; }


        [Display(Name = "User Role:")]
        public string Role { get; set; }
        public string[] Roles = new[] { " Admin", " DataEntry", " Speaker - (Default)" };     //NOTE the space at the beginning of the string



        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
                        
            [Required]
            [Display(Name = "Full Name:")]
            public string Name { get; set; }

            [Display(Name = "Speaker #:")]
            public int? SpkNum { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        //****OnPose ****
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser 
                { 
                    UserName = Input.Email, 
                    Email = Input.Email,
                    Name = Input.Name,
                    SpkNum = Input.SpkNum
                };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    //after user is successfull added, create roles if the do not exist
                    if(!await _roleManager.RoleExistsAsync(SD.AdminEndUser)) 
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.SpeakerEndUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.SpeakerEndUser));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.DataEntryEndUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.DataEntryEndUser));
                    }

                    //assign use to default role  SpeakerEndUser is the default user
                    ////await _userManager.AddToRoleAsync(user, SD.AdminEndUser);

                    //get selected role, if there is one. otherwise set role to SpeakerEndUser (the default)
                    var sRole = Request.Form["Role"];

                    if (sRole == " Admin")
                    {
                        await _userManager.AddToRoleAsync(user, SD.AdminEndUser);
                        return RedirectToPage("/Users/Index");
                    }
                    else if (sRole == " DataEntry")
                    {
                        await _userManager.AddToRoleAsync(user, SD.DataEntryEndUser);
                        return RedirectToPage("/Users/Index");
                    }
                    else
                    {
                        //assign user to default role  SpeakerEndUser is the default role
                        await _userManager.AddToRoleAsync(user, SD.SpeakerEndUser);
                        return RedirectToPage("/Users/Index");
                    }



                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    ////await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                       //// $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
