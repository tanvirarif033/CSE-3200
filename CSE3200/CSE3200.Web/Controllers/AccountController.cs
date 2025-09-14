using CSE3200.Infrastructure.Identity;
using CSE3200.Web.Models;
using CSE3200.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using CSE3200.Web.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace CSE3200.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        //private readonly IEmailSender _emailSender;
        private readonly IOtpService _otpService;
        private readonly IEmailSender _emailSender;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IOtpService otpService,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _otpService = otpService;
            _emailSender = emailSender;
        }

        // ===== Registration =====
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync(string returnUrl = null)
        {
            var model = new RegisterModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsync(RegisterModel model)
        {
            model.ReturnUrl ??= Url.Content("~/");
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);

                user.RegistrationDate = DateTime.UtcNow;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Donor");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId, code, returnUrl = model.ReturnUrl },
                        protocol: Request.Scheme);

                    // await _emailSender.SendEmailAsync(...);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = model.Email, returnUrl = model.ReturnUrl });
                    }
                    else
                    {
                        // After successful registration, redirect to login page instead of signing in
                        TempData["SuccessMessage"] = "Registration successful! Please login with your credentials.";
                        return RedirectToAction("Login", new { returnUrl = model.ReturnUrl });
                    }
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // ===== Username/Password Login =====
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            var model = new LoginModel();

            if (!string.IsNullOrEmpty(model.ErrorMessage))
                ModelState.AddModelError(string.Empty, model.ErrorMessage);

            model.ReturnUrl ??= Url.Content("~/");

            // Clear external cookie for clean login
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            model.ReturnUrl = returnUrl;

            return View(model);
        }

        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            model.ReturnUrl ??= Url.Content("~/");
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Redirect to homepage instead of admin dashboard
                return LocalRedirect("~/");
            }

            if (result.RequiresTwoFactor)
                return RedirectToPage("./LoginWith2fa", new { model.ReturnUrl, model.RememberMe });

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        // ===== Google / External Login =====
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = "/")
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = "/")
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction(nameof(Login), new { returnUrl });

            // If the user already has a login (linked), sign in the user with this external login provider
            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                // Redirect to homepage instead of admin dashboard
                return LocalRedirect("~/");
            }

            // Otherwise, get the email and create/link a local user
            var email = info.Principal.FindFirstValue(ClaimTypes.Email)
                       ?? info.Principal.FindFirstValue("email");

            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Google did not provide an email address.";
                return RedirectToAction(nameof(Login), new { returnUrl });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var given = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                var family = info.Principal.FindFirstValue(ClaimTypes.Surname);
                var displayName = info.Principal.FindFirstValue(ClaimTypes.Name);

                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    RegistrationDate = DateTime.UtcNow,
                    FirstName = given ?? displayName?.Split(' ').FirstOrDefault() ?? "",
                    LastName = family ?? displayName?.Split(' ').Skip(1).FirstOrDefault() ?? ""
                };

                var create = await _userManager.CreateAsync(user);
                if (!create.Succeeded)
                {
                    foreach (var e in create.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);

                    return RedirectToAction(nameof(Login), new { returnUrl });
                }

                // Optional: default role
                await _userManager.AddToRoleAsync(user, "Donor");
            }

            // Link the external login to the local user (ignore if already linked)
            var addLogin = await _userManager.AddLoginAsync(user, info);

            await _signInManager.SignInAsync(user, isPersistent: false);

            // Redirect to homepage instead of admin dashboard
            return LocalRedirect("~/");
        }

        // GET: shows the popup view
        [HttpGet, Authorize]
        public IActionResult Logout() => View();

        // POST: actually logs out
        [HttpPost, ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return LocalRedirect("~/"); // or LocalRedirect(returnUrl ?? "~/");
        }

        // ===== Access Denied =====
        public IActionResult AccessDenied() => View();

        // ===== Helpers =====
        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException(
                    $"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("The default UI requires a user store with email support.");

            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        // In your AccountController, add these methods:

        // ===== Forgot Password =====
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // Generate OTP
                var otp = _otpService.GenerateOtp(user);

                try
                {
                    // Send OTP via email
                    await _emailSender.SendEmailAsync(
                        model.Email,
                        "Password Reset OTP - Disaster Management System",
                        $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 10px; padding: 20px;'>
                    <div style='text-align: center; background-color: #f97316; padding: 15px; border-radius: 8px; margin-bottom: 20px;'>
                        <h2 style='color: white; margin: 0;'>Disaster Management System</h2>
                    </div>
                    <p style='font-size: 16px;'>You have requested to reset your password. Use the OTP code below to proceed:</p>
                    <div style='background-color: #f1f5f9; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 5px; margin: 20px 0; border-radius: 8px;'>
                        {otp}
                    </div>
                    <p style='font-size: 14px; color: #64748b;'>This OTP will expire in 10 minutes. If you didn't request this, please ignore this email.</p>
                    <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 20px 0;'>
                    <p style='font-size: 12px; color: #64748b; text-align: center;'>This is an automated message from the National Disaster Response Portal.</p>
                </div>"
                    );

                    _logger.LogInformation($"OTP email sent to {model.Email}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send OTP email");
                    ModelState.AddModelError(string.Empty, "Failed to send OTP email. Please try again later.");
                    return View(model);
                }

                // Store email in TempData for the next step
                TempData["ResetEmail"] = model.Email;
                return RedirectToAction(nameof(EnterOtp));
            }

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // ===== OTP Verification =====
        [AllowAnonymous]
        public IActionResult EnterOtp()
        {
            // Check if email is available from the forgot password step
            if (TempData["ResetEmail"] == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var model = new EnterOtpModel
            {
                Email = TempData["ResetEmail"].ToString()
            };

            // Keep the email for the form
            TempData.Keep("ResetEmail");

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(EnterOtpModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.Keep("ResetEmail");
                return View("EnterOtp", model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // Validate OTP
            if (!_otpService.ValidateOtp(user, model.OTP))
            {
                ModelState.AddModelError(string.Empty, "Invalid or expired OTP.");
                TempData.Keep("ResetEmail");
                return View("EnterOtp", model);
            }

            // OTP is valid, redirect to reset password page
            TempData["ValidatedEmail"] = model.Email;
            return RedirectToAction(nameof(ResetPassword));
        }

        // In AccountController.cs - Fix the ResetPassword methods

        // ===== Reset Password =====
        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            // Check if email is validated through OTP
            if (TempData["ValidatedEmail"] == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var model = new ResetPasswordModel
            {
                Email = TempData["ValidatedEmail"].ToString()
            };

            // Keep the email for the form
            TempData.Keep("ValidatedEmail");

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.Keep("ValidatedEmail");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            try
            {
                // Generate password reset token
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Reset the password
                var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Password reset successfully for user: {model.Email}");

                    // Clear the TempData after successful reset
                    TempData.Remove("ValidatedEmail");
                    TempData.Remove("ResetEmail");

                    // Sign in the user automatically after password reset
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resetting password for user: {model.Email}");
                ModelState.AddModelError(string.Empty, "An error occurred while resetting your password. Please try again.");
            }

            TempData.Keep("ValidatedEmail");
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}