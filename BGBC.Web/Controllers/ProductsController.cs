using BGBC.Core;
using BGBC.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BGBC.Web.Controllers
{
    public class ProductsController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ProductsController));
        // GET: Products
        private IRepository<Product, int?> _repository;

        public ProductsController()
        {
            _repository = new ProductRepository();
        }
        public ActionResult Index()
        {
            return View(_repository.Get());
        }

        [CustomAuthorize(Roles = "Admin")]
        [Authorize]
        public ActionResult Add()
        {
            if (TempData["productdata"] == null)
                return View();
            else
                return View("Add", (Product)TempData["productdata"]);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Admin")]
        [Authorize]
        public ActionResult Confirm(Product product)
        {
            if (ModelState.IsValid)
            {
                TempData["productdata"] = product;
                return View(product);
            }
            return View("Add", product);
        }

        [CustomAuthorize(Roles = "Admin")]
        [Authorize]        
        public ActionResult ConfirmEdit(Product product)
        {
            if (ModelState.IsValid)
            {
                TempData["productdata"] = product;
                return View(product);
            }
            return View("Edit", product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Admin")]
        [Authorize]
        public ActionResult Add(Product product)
        {
            try
            {
                product = (Product)TempData["productdata"];
                _repository.Add(product);
                TempData["SucessMessage"] = "Product "+ product.Name+" added successfully.";
                return RedirectToAction("Manage");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(product);
        }

        [CustomAuthorize(Roles = "Admin")]
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _repository.Get(id); 
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Admin")]
        [Authorize]
        public ActionResult Edit(Product product)
        {
            try
            {
                    product = (Product)TempData["productdata"];
                    _repository.Update(product);
                    TempData["SucessMessage"] = "Product updated successfully."; 
                    return RedirectToAction("Manage");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(product);
        }


        [CustomAuthorize(Roles = "Admin")]
        [Authorize]
        public ActionResult Manage()
        {
            return View(_repository.Get());
        }

        [CustomAuthorize(Roles = "Admin")]
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = _repository.Get(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Admin")]
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
                    TempData["SucessMessage"] = "Product removed successfully.";
                    return RedirectToAction("Manage");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return RedirectToAction("Manage");
        }

        [Authorize]
        public ActionResult ViewOrders(int id)
        {
            return View(BGBCFunctions.ProductOrders().Where(x => x.ProductID == id).OrderByDescending(s => s.TransDate).ToList());
        }
    }
}
