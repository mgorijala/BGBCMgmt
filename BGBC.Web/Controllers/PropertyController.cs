﻿using BGBC.Core;
using BGBC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BGBC.Web.Controllers
{
    public class PropertyController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PropertyController));
        // GET: Property
        private IRepository<Property, int> _repository;
        private IRepository<ZipCode, int> _Zip;
        public PropertyController()
        {
            _repository = new PropertyRepository();
            _Zip = new ZipRepository();
        }

        public ActionResult Index()
        {
            //_repository.Get()
            return View(_repository.Get());
        }

        [Authorize]
        public ActionResult Add()
        {

            PopulateDropDown();
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(Property property)
        {
            var query = from state in ModelState.Values
                        from error in state.Errors
                        select error.ErrorMessage;

            int newval = Convert.ToInt32(property.Zip);
            ZipCode zipcode = _Zip.Get(newval);

            // int value = _Zip.Get(newval);

            if (ModelState.IsValid)
            {
                // IF(F20=28,F20,MOD(F20,28))
                int totalDay = Convert.ToInt32(property.RentDueDay + property.GracePeriod);
                int finalday = 0;
                if ((totalDay == 56) || (totalDay == 28))
                {
                    finalday = 28;
                }
                else
                {
                    finalday = totalDay % 28;
                }
                property.FinalDueDay = Convert.ToInt16(finalday);
               

                if (zipcode != null)
                {
                    return View(property);
                }
                else
                {
                    ModelState.AddModelError("Zip", "The Zip you entered is not in our location");
                }
            }
            PopulateDropDown();
            return View("Add", property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Add(Property property, string Command)
        {
            if (Command == "Edit") { PopulateDropDown(); return View("Add", property); }
            try
            {
                PopulateDropDown();
                property.UserID = ((BGBC.Core.CustomPrincipal)(User)).UserId;
                _repository.Add(property);
                TempData["SucessMessage"] = "Property " + property.Name + " added successfully.";
                return RedirectToAction("MyProperties", "Owner");

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return View(property);
        }

        [Authorize]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property property = _repository.Get(id.Value);
            if (property == null)
            {
                return HttpNotFound();
            }
            if (property.UserID != ((BGBC.Core.CustomPrincipal)(User)).UserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PopulateDropDown();
            return View(property);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditConfirm(Property property)
        {
            if (ModelState.IsValid)
            {
                PopulateDropDown();
                return View(property);
            }
            return View("Edit", property);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AdminConfirm(Property property)
        {
            ViewBag.UserID = property.UserID;
            if (ModelState.IsValid)
            {
                return View(property);
            }
            return View("AdminEdit", property);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(Property property, string Command)
        {
            if (Command == "Edit") { PopulateDropDown(); return View("Edit", property); }
            try
            {
                PopulateDropDown();
                TempData["SucessMessage"] = "Property updated successfully.";
                _repository.Update(property);
                return RedirectToAction("MyProperties", "Owner");

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return View(property);
        }

        [CustomAuthorize(Roles = "Owner")]
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property property = _repository.Get((int)id);

            if (property == null)
            {
                return HttpNotFound();
            }
            return View(property);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Owner")]
        [Authorize]
        public ActionResult DeletePost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _repository.Remove(_repository.Get((int)id));
                    TempData["SucessMessage"] = "Property removed successfully.";
                    return RedirectToAction("MyProperties", "Owner");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return RedirectToAction("MyProperties", "Owner");
        }

        [CustomAuthorize(Roles = "Admin")]
        [Authorize]
        public ActionResult AdminEdit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Property property = _repository.Get((int)id);
            ViewBag.UserID = property.UserID;
            if (property == null)
            {
                return HttpNotFound();
            }
            PopulateDropDown();

            return View(property);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Admin")]
        [Authorize]
        public ActionResult AdminEdit(Property property)
        {
            ViewBag.UserID = property.UserID;
            try
            {
                _repository.Update(property);
                TempData["SucessMessage"] = "Property Updated successfully.";

                // PropertyId is wrong -- It display's empty Values
                return RedirectToAction("OwnerProperties", "Admin", new { id = property.UserID });

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return AdminEdit(property);
        }

        private void PopulateDropDown()
        {
            ViewBag.MonthDays = Enumerable.Range(1, 28).Select(i => new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            ViewBag.States = new List<SelectListItem>()
            {
                new SelectListItem() {Text="Alabama", Value="AL"},
                new SelectListItem() { Text="Alaska", Value="AK"},
                new SelectListItem() { Text="Arizona", Value="AZ"},
                new SelectListItem() { Text="Arkansas", Value="AR"},
                new SelectListItem() { Text="California", Value="CA"},
                new SelectListItem() { Text="Colorado", Value="CO"},
                new SelectListItem() { Text="Connecticut", Value="CT"},
                new SelectListItem() { Text="District of Columbia", Value="DC"},
                new SelectListItem() { Text="Delaware", Value="DE"},
                new SelectListItem() { Text="Florida", Value="FL"},
                new SelectListItem() { Text="Georgia", Value="GA"},
                new SelectListItem() { Text="Hawaii", Value="HI"},
                new SelectListItem() { Text="Idaho", Value="ID"},
                new SelectListItem() { Text="Illinois", Value="IL"},
                new SelectListItem() { Text="Indiana", Value="IN"},
                new SelectListItem() { Text="Iowa", Value="IA"},
                new SelectListItem() { Text="Kansas", Value="KS"},
                new SelectListItem() { Text="Kentucky", Value="KY"},
                new SelectListItem() { Text="Louisiana", Value="LA"},
                new SelectListItem() { Text="Maine", Value="ME"},
                new SelectListItem() { Text="Maryland", Value="MD"},
                new SelectListItem() { Text="Massachusetts", Value="MA"},
                new SelectListItem() { Text="Michigan", Value="MI"},
                new SelectListItem() { Text="Minnesota", Value="MN"},
                new SelectListItem() { Text="Mississippi", Value="MS"},
                new SelectListItem() { Text="Missouri", Value="MO"},
                new SelectListItem() { Text="Montana", Value="MT"},
                new SelectListItem() { Text="Nebraska", Value="NE"},
                new SelectListItem() { Text="Nevada", Value="NV"},
                new SelectListItem() { Text="New Hampshire", Value="NH"},
                new SelectListItem() { Text="New Jersey", Value="NJ"},
                new SelectListItem() { Text="New Mexico", Value="NM"},
                new SelectListItem() { Text="New York", Value="NY"},
                new SelectListItem() { Text="North Carolina", Value="NC"},
                new SelectListItem() { Text="North Dakota", Value="ND"},
                new SelectListItem() { Text="Ohio", Value="OH"},
                new SelectListItem() { Text="Oklahoma", Value="OK"},
                new SelectListItem() { Text="Oregon", Value="OR"},
                new SelectListItem() { Text="Pennsylvania", Value="PA"},
                new SelectListItem() { Text="Rhode Island", Value="RI"},
                new SelectListItem() { Text="South Carolina", Value="SC"},
                new SelectListItem() { Text="South Dakota", Value="SD"},
                new SelectListItem() { Text="Tennessee", Value="TN"},
                new SelectListItem() { Text="Texas", Value="TX"},
                new SelectListItem() { Text="Utah", Value="UT"},
                new SelectListItem() { Text="Vermont", Value="VT"},
                new SelectListItem() { Text="Virginia", Value="VA"},
                new SelectListItem() { Text="Washington", Value="WA"},
                new SelectListItem() { Text="West Virginia", Value="WV"},
                new SelectListItem() { Text="Wisconsin", Value="WI"},
                new SelectListItem() { Text="Wyoming", Value="WY"}
            };
        }
    }
}