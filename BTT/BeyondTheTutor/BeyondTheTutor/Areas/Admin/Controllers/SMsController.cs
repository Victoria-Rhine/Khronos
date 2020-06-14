﻿using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using BeyondTheTutor.DAL;
using BeyondTheTutor.Models.SMSModels;
using Microsoft.AspNet.Identity;


namespace BeyondTheTutor.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SMsController : Controller
    {
        private BeyondTheTutorContext db = new BeyondTheTutorContext();

        // GET: Admin/SMs/Create
        public ActionResult Create()
        {
            ViewBag.Current = "AdminAddMessage";

            // get the current user logged in
            var userID = User.Identity.GetUserId();
            var currentUserID = db.BTTUsers.Where(m => m.ASPNetIdentityID.Equals(userID)).FirstOrDefault().ID;
            ViewBag.userID = currentUserID;

            // get a list of all tutors
            var Tutors = db.BTTUsers.Where(a => a.ID == a.Tutor.ID)
                .Select(a => new
                {
                    ID = a.ID,
                    Name = a.FirstName + " " + a.LastName
                });
            ViewBag.Tutors = Tutors;

            return View();
        }

        public ActionResult SendMessageAdmin()
        {
            // get data from form
            string subject = Request.QueryString["subject"];
            string message = Request.QueryString["message"];
            string tutor = Request.QueryString["receiver"];
            string priority = Request.QueryString["priority"];

            // get id for logged in admin
            var userID = User.Identity.GetUserId();
            var currentUserID = db.BTTUsers.Where(m => m.ASPNetIdentityID.Equals(userID)).FirstOrDefault().ID;

            // the only bad input would be an empty message so check for that here
            if (message == null || message.IsEmpty() == true)
            {
                return Json("Message Cannot Be Empty", JsonRequestBehavior.AllowGet);
            }
            else
            {
                SM SMSMessage = new SM
                {
                    DateSent = DateTime.Now,
                    Message = message,
                    Sender = Convert.ToInt32(currentUserID),
                    Priority = Convert.ToInt32(priority)
                };

                // check if subject of tutor is empty
                if (tutor == null || tutor.IsEmpty() == true)
                {
                    SMSMessage.Receiver = null;
                }
                else
                {
                    SMSMessage.Receiver = Convert.ToInt32(tutor);
                }
                if (subject == null || subject.IsEmpty() == true)
                {
                    SMSMessage.Subject = "No subject";
                }
                else
                {
                    SMSMessage.Subject = subject;
                }

                // check if entry is valid and save to db and have modal popup
                if (ModelState.IsValid)
                {
                    db.SMS.Add(SMSMessage);
                    db.SaveChanges();

                    return Json("Message Sent", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Something Went Wrong", JsonRequestBehavior.AllowGet);
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
