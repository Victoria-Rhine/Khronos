﻿namespace BeyondTheTutor.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;


    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress] //MAKE SURE THIS STAYS IF MAKING NEW MODELS
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        //custom regular expression, please save
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+((@wou.edu)|(@mail.wou.edu))$",
            ErrorMessage = "Please make sure your email is related to Western Oregon University.")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress] // make sure email address attribute is here
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }


    public class RegistrationTypes
    {
        public StudentRegistraionViewModel studentVM { get; set; }
        public TutorRegistrationViewModel tutorVM { get; set; }
        public ProfessorRegistrationViewModel professorVM { get; set; }

    }


    public class StudentRegistraionViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        //custom regular expression, please save
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+((@wou.edu)|(@mail.wou.edu))$",
            ErrorMessage = "Please make sure your email is related to Western Oregon University.")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must have at least 2 characters")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must have at least 2 characters")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Class of")]
        public Int16 GraduatingYear { get; set; }

        [Required]
        [Display(Name = "Class Standing")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Please make sure you entered your class correctly.")]
        public string ClassStanding { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }


    public class TutorRegistrationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        //custom regular expression, please save
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+((@wou.edu)|(@mail.wou.edu))$",
            ErrorMessage = "Please make sure your email is related to Western Oregon University.")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must have at least 2 characters")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must have at least 2 characters")]
        public string LastName { get; set; }

        //[Required]
        [Display(Name = "Class of")]
        public short ClassOf { get; set; }

        [Required]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Please make sure you entered your V Number correctly.")]
        //Custom V number checker for correctness
        [RegularExpression(@"^[vV][0-9]{8}$", ErrorMessage = "Your V number seems to be broken.")]
        public string VNumber { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ProfessorRegistrationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        //custom regular expression, please save
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+((@wou.edu)|(@mail.wou.edu))$",
            ErrorMessage = "Please make sure your email is related to Western Oregon University.")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must have at least 2 characters")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must have at least 2 characters")]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class AdminRegistrationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        //custom regular expression, please save
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+((@wou.edu)|(@mail.wou.edu))$",
            ErrorMessage = "Please make sure your email is related to Western Oregon University.")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must have at least 2 characters")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must have at least 2 characters")]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }


    public class ResetPasswordViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        //custom regular expression, please save
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+((@wou.edu)|(@mail.wou.edu))$",
            ErrorMessage = "Please make sure your email is related to Western Oregon University.")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        //custom regular expression, please save
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+((@wou.edu)|(@mail.wou.edu))$",
            ErrorMessage = "Please make sure your email is related to Western Oregon University.")]
        public string Email { get; set; }
    }

    // For passing simple message data to the view
    public enum AccountMessageId
    {
        EmailSentSuccess,
        EmailConfirmationNeeded
    }
}
