﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using BeyondTheTutor.DAL;
using BeyondTheTutor.Models;
using Microsoft.AspNet.Identity;

namespace BeyondTheTutor.Areas.Admin.Controllers
{
    public class ClassesController : Controller
    {
        private BeyondTheTutorContext db = new BeyondTheTutorContext();
        private ApplicationDbContext context = new ApplicationDbContext();


        // GET: Admin/Classes
        public ActionResult Index()
        {
            ViewBag.Current = "AdminClassesIndex";

            var userID = User.Identity.GetUserId();
            var currentUser = db.BTTUsers.Where(m => m.ASPNetIdentityID.Equals(userID)).FirstOrDefault().FirstName;
            ViewBag.FirstName = currentUser;

            string userInput = Request.QueryString["search"];

            if (userInput == null)
            {
                return View(db.Classes.OrderBy(c => c.Name).ToList());
            }
            else
            {
                ViewBag.searched = userInput;
                var replaceWith = Regex.Match(userInput, @"(?=[a-zA-Z])([^ ])(?=\d)([^ ]{1})").ToString();
                replaceWith = replaceWith.Insert(1, " ");
                var temp = Regex.Replace(userInput, @"(?=[a-zA-Z])([^ ])(?=\d)([^ ]{1})", replaceWith).ToLower();

                userInput = userInput.ToLower();
                var searchedClasses = db.Classes.OrderBy(c => c.Name)
                .Where(c => c.Name.ToLower().Contains(userInput) || c.Name.ToLower().Contains(temp)).ToList();


                return View(searchedClasses);
            }
        }

        // GET: Admin/Classes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // GET: Admin/Classes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Classes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] Class @class)
        {
            if (ModelState.IsValid)
            {
                db.Classes.Add(@class);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@class);
        }

        // GET: Admin/Classes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // POST: Admin/Classes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] Class @class)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@class).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@class);
        }

        // GET: Admin/Classes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // POST: Admin/Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Class @class = db.Classes.Find(id);
            db.Classes.Remove(@class);
            db.SaveChanges();
            return RedirectToAction("Index");
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
