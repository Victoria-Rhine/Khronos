﻿using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BeyondTheTutor.Models;
using BeyondTheTutor.DAL;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Routing;
using reCAPTCHA.MVC;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace BeyondTheTutor.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.Current = "AccountLogin";
            ViewBag.ReturnUrl = returnUrl;

            if (TempData["msg"] != null)
                switch (TempData["msg"].ToString()) // resetFail and Suc ref. from task: ResetPassword; and confirmSuc & Fail from ConfirmEmail.
                {
                    case "resetFail":
                        ViewBag.error = "Something went wrong, please try again later.";
                        break;
                    case "resetSuc":
                        ViewBag.msg = "Congrats! You've successfully changed your password! Please proceed by logging in.";
                        break;
                    case "confirmFail":
                        ViewBag.error = "Oops, something went wrong. Please try again later.";
                        break;
                    case "confirmSuc":
                        ViewBag.msg = "Congrats! You've successfully confirmed your email. Please proceed by logging in.";
                        break;
                    default:
                        break;
                }

            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            //var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            // Error in call here. PasswordSingInAsync needs username not email, so either they need to be the same or you have to modify the
            // login page and loginviewmodel to get the username

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    BeyondTheTutorContext db = new BeyondTheTutorContext();
                    // Require the user to have a confirmed email before they can log on.
                    var user = await UserManager.FindByNameAsync(model.Email);
                    // Resolve the user via their email
                    var roles = await UserManager.GetRolesAsync(user.Id);

                    var confirmedByEmail = await UserManager.IsEmailConfirmedAsync(user.Id);
                    var confirmedByAdmin = false;
                    var userID = UserManager.FindByName(model.Email).Id;

                    var currentUserID = db.BTTUsers.Where(m => m.ASPNetIdentityID.Equals(userID)).FirstOrDefault().ID;

                    if (roles.Contains("Tutor"))
                        confirmedByAdmin = db.Tutors.Find(currentUserID).AdminApproved;
                     else if( roles.Contains("Professor"))
                        confirmedByAdmin = db.Professors.Find(currentUserID).AdminApproved;


                    if (user != null)
                    {
                        if (!confirmedByEmail && roles.Contains("Student"))
                        {
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            ViewBag.error = "You must have a confirmed email to log on.";
                            return View();
                        }
                        else if (!(roles.Contains("Admin") || roles.Contains("Student")) && (!confirmedByEmail || !confirmedByAdmin)) 
                        {
                            ViewBag.error = "You must confirm your email and/or get special permission by emailing: Admin@BeyondTheTutor.com";
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            return View();
                        }
                    }

                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "admin" });
                    }
                    else if (roles.Contains("Professor"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "professor" });
                    }
                    else if (roles.Contains("Student"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "student" });
                    }
                    else if (roles.Contains("Tutor"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "tutor" });
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.Current = "AccountRegister";
            ViewBag.ReCapKey = System.Web.Configuration.WebConfigurationManager.AppSettings["ReCapKey"];
            
            return View();
        }

        public class CaptchaResponse
        {
            [JsonProperty("success")]
            public string Success { get; set; }

            [JsonProperty("error-codes")]
            public List<string> ErrorCodes { get; set; }
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegistrationTypes model)
        {
            bool isStudent, isTutor, isProfessor, _error;
            isStudent = isTutor = isProfessor = _error = false;
            string _email, _password, _firstname, _lastname, _confmessage, _class_standing, _vnumber ;

            _email = _password = _firstname = _lastname = _class_standing = _vnumber = null;

            short _classof = 0000;

            var response = Request["g-recaptcha-response"];
            //secret that was generated in key value pair
            string secret = System.Web.Configuration.WebConfigurationManager.AppSettings["ReCapSecretKey"];

            var client = new WebClient();
            var reply =
                client.DownloadString(
                    string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

            //when response is false check for the error message
            if (captchaResponse.Success.Equals(false))
            {
                if (captchaResponse.ErrorCodes.Count <= 0) return View();

                var error = captchaResponse.ErrorCodes[0].ToLower();
                switch (error)
                {
                    case ("missing-input-secret"):
                        ViewBag.Message = "The secret parameter is missing.";
                        break;
                    case ("invalid-input-secret"):
                        ViewBag.Message = "The secret parameter is invalid or malformed.";
                        break;

                    case ("missing-input-response"):
                        ViewBag.Message = "The response parameter is missing.";
                        break;
                    case ("invalid-input-response"):
                        ViewBag.Message = "The response parameter is invalid or malformed.";
                        break;

                    default:
                        ViewBag.Message = "Error occured. Please try again";
                        break;
                }

                _error = true;
            }
            else
            {
                ViewBag.Message = "Valid";
            }

            if (ModelState.IsValid && !_error)
            {
                if (model.studentVM != null)
                {
                    isStudent = true;
                    _firstname = model.studentVM.FirstName;
                    _lastname = model.studentVM.LastName;
                    _password = model.studentVM.Password;
                    _class_standing = model.studentVM.ClassStanding;
                    _classof = model.studentVM.GraduatingYear;
                    _email = model.studentVM.Email;
                }
                if (model.tutorVM != null)
                {
                    isTutor = true;
                    _firstname = model.tutorVM.FirstName;
                    _lastname = model.tutorVM.LastName;
                    _password = model.tutorVM.Password;
                    _vnumber = model.tutorVM.VNumber;
                    _classof = model.tutorVM.ClassOf;
                    _email = model.tutorVM.Email;
                }
                if (model.professorVM != null)
                {
                    isProfessor = true;
                    _firstname = model.professorVM.FirstName;
                    _lastname = model.professorVM.LastName;
                    _password = model.professorVM.Password;
                    _email = model.professorVM.Email;
                }

                if (isTutor || isProfessor)
                {
                    _confmessage = "Confirm your account email and wait for admin approval";
                    ViewBag.Message = "Once you've confirmed that " + _email + " is your email address and recieved admin approval, you'll be able to use your account.";
                }
                else
                {
                    ViewBag.Message = "Once you've confirmed that " + _email + " is your email address, you can continue to your account.";
                    _confmessage = "Confirm your account email";
                }

                //var user = new ApplicationUser { UserName = model.FirstName + " " + model.LastName, Email = model.Email };
                var user = new ApplicationUser
                {
                    UserName = _email,
                    Email = _email 
                };

                var result = await UserManager.CreateAsync(user, _password);

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, _confmessage, _firstname);

                    TempData["Message"] = ViewBag.Message;

                    // Won't be shown to the user if we redirect to home
                    ViewBag.Message = "Check your email and confirm your account; you must be confirmed "
                        + "if you ever need to recover your password.";
                    // TODO: Handle errors, do this upon refactoring into repository pattern
                    // Succeeded in creating a new Identity account, so let's create a new 


                    var special_user = new BTTUser
                    {
                        FirstName = _firstname,
                        LastName = _lastname,
                        ASPNetIdentityID = user.Id                    
                    };

                    BeyondTheTutorContext db = new BeyondTheTutorContext();

                    if (model.studentVM != null)
                    {
                        var sub_user = new Student
                        {
                            ClassStanding = _class_standing,
                            GraduatingYear = _classof
                        };

                        sub_user.BTTUser = special_user;
                        db.BTTUsers.Add(special_user);
                        db.Students.Add(sub_user);
                        UserManager.AddToRole(user.Id, "Student");
                    }
                    if (model.tutorVM != null)
                    {
                        var sub_user = new Tutor
                        {
                            VNumber = _vnumber,
                            ClassOf = _classof,
                        };

                        sub_user.BTTUser = special_user;
                        db.BTTUsers.Add(special_user);
                        db.Tutors.Add(sub_user);
                        UserManager.AddToRole(user.Id, "Tutor");
                    }
                    if (model.professorVM != null)
                    {
                        var sub_user = new Professor
                        {

                        };
                        sub_user.BTTUser = special_user;
                        db.BTTUsers.Add(special_user);
                        db.Professors.Add(sub_user);
                        UserManager.AddToRole(user.Id, "Professor");
                    }


                    await db.SaveChangesAsync();

                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            if (model.professorVM != null)
                ViewBag.validationError = "professor";
            else if (model.tutorVM != null)
                ViewBag.validationError = "tutor";
            else if (model.studentVM != null)
                ViewBag.validationError = "student";

            ViewBag.ReCapKey = System.Web.Configuration.WebConfigurationManager.AppSettings["ReCapKey"];

            return View(model);
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject, string name)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            string bodyOfEmail = "Hello " + name + ", please follow <a href=\"" + callbackUrl + "\">this link</a> to confirm your <i>Beyond The Tutor</i> account";
            await UserManager.SendEmailAsync(userID, subject, bodyOfEmail);

            return callbackUrl;
        }

        // Our addition, to send out an email with a confirmation link
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendConfirmationEmail(string urlOfReferrer)
        {
            string id = User.Identity.GetUserId();
            await SendEmailConfirmationTokenAsync(id, "Confirm your account", ",");
            ViewBag.EmailSent = true;
            return RedirectToAction("Index", new RouteValueDictionary(new { controller = "Home", action = "Index", message = AccountMessageId.EmailSentSuccess }));

        }

        
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                TempData["msg"] = "confirmFail";
                return RedirectToAction("Login");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                TempData["msg"] = "confirmSuc";
            } else { TempData["msg"] = "confirmFail"; }

            return RedirectToAction("Login");
        }


        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ViewBag.ReCapKey = System.Web.Configuration.WebConfigurationManager.AppSettings["ReCapKey"];

            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            bool _error = false;
            var response = Request["g-recaptcha-response"];
            //secret that was generated in key value pair
            string secret = System.Web.Configuration.WebConfigurationManager.AppSettings["ReCapSecretKey"];

            var client = new WebClient();
            var reply =
                client.DownloadString(
                    string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

            //when response is false check for the error message
            if (captchaResponse.Success.Equals(false))
            {
                if (captchaResponse.ErrorCodes.Count <= 0) return View();

                var error = captchaResponse.ErrorCodes[0].ToLower();
                switch (error)
                {
                    case ("missing-input-secret"):
                        ViewBag.Message = "The secret parameter is missing.";
                        break;
                    case ("invalid-input-secret"):
                        ViewBag.Message = "The secret parameter is invalid or malformed.";
                        break;

                    case ("missing-input-response"):
                        ViewBag.Message = "The response parameter is missing.";
                        break;
                    case ("invalid-input-response"):
                        ViewBag.Message = "The response parameter is invalid or malformed.";
                        break;

                    default:
                        ViewBag.Message = "Error occured. Please try again";
                        break;
                }

                _error = true;
            }
            else
            {
                ViewBag.Message = "Valid";
            }


            if (ModelState.IsValid && !_error)
            {

                var user = await UserManager.FindByNameAsync(model.Email);

                TempData["msg"] = model.Email.ToString() ;


                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction("Index", "Home");
                }

                // For more information on how to 
                // enable account confirmation and password reset 
                // please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "DO NOT PRESS IF THIS EMAIL ISN'T ASSOSIATED WITH BeyondTheTutor\nTo reset your password please click the following link: <a href=\"" + callbackUrl + "\">RESET PASSWORD</a>");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReCapKey = System.Web.Configuration.WebConfigurationManager.AppSettings["ReCapKey"];

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var z = model.Code != null ? TempData["msg"] = "resetSuc" : TempData["msg"] = "resetFail"; // goes to Login with TempData stating the result

            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("Login", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }

            AddErrors(result);
            return RedirectToAction("Login", "Account");
        }


        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}