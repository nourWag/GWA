using GWA.Domaine.Entities;
using GWA.Service.Categories;
using GWA.Service.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.IO;
using System.Net;
using PagedList;
using GWA.WEB1.Models.Product;
using Microsoft.AspNet.Identity;
using IdentitySample.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using GWA.Data.Context;
using Microsoft.AspNet.Identity.EntityFramework;
using GWA.WEB1.Helpers;

namespace GWA.WEB1.Controllers.Products
{
    public class ProductController : Controller
    {

        GWAContext context;


        ProductService ps = null;
        CategoryService cs = null;
        public ProductController()
        {
            context = new GWAContext();
            ps = new ProductService();
            cs = new CategoryService();
        }

        public User getCurrentUser()
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var user = UserManager.FindById(User.Identity.GetUserId());
            return user;
        }

        #region CRUD
        // GET: Product
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

           // User u = ps.getBestSeller();

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var prod = ps.GetAll();

            if (!String.IsNullOrEmpty(searchString))
            {
                prod = prod.Where(s => s.Name.Contains(searchString)
                                       || s.reference.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    prod = prod.OrderByDescending(s => s.Name);
                    break;
                case "Date":
                    prod = prod.OrderBy(s => s.CreationDate);
                    break;
                case "date_desc":
                    prod = prod.OrderByDescending(s => s.CreationDate);
                    break;
                default:
                    prod = prod.OrderBy(s => s.Name);
                    break;
            }


            List<ProductViewModel> pvm = new List<ProductViewModel>();
            foreach (var item in prod)
            {
                pvm.Add(
                    new ProductViewModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        CreationDate = item.CreationDate,
                        CategoryId = item.IdCategory,
                        CurrentPrice = item.CurrentPrice,
                        reference = item.reference,
                        status = item.status,
                        UpdateDate = item.UpdateDate,
                        ImageUrl = item.ImageUrl,
                        //IdUser = item.IdUser
                        //BestSeller = u
                    });
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            
            
            return View(pvm.ToPagedList(pageNumber, pageSize) );
            
        }

        // GET: Product/Details/5
        public ActionResult Details(int id)
        {
            Product p = new Product();
            p = ps.GetById(id);

            ProductViewModel pvm = new ProductViewModel
            {
                CategoryId = p.IdCategory,
                CreationDate = p.CreationDate,
                CurrentPrice = p.CurrentPrice,
                IdUser = getCurrentUser().Id,
                Name = p.Name,
                reference = p.reference,
                status = p.status,
                UpdateDate =p.UpdateDate,
                ImageUrl = p.ImageUrl

            };
            return View(pvm);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            var pvm = new ProductViewModel();
            List<Category> Categories = cs.GetAll().ToList() ;
            pvm.Category = Categories.ToSelectListItems();

            return View(pvm);
          
        }

        // POST: Product/Create
        [HttpPost]
        public ActionResult Create(ProductViewModel pvm, HttpPostedFileBase Image)
        {
            pvm.ImageUrl = Image.FileName;
            Product t = new Product
            {
                IdCategory = pvm.CategoryId,
                CreationDate = DateTime.Now,
                CurrentPrice = pvm.CurrentPrice,
                //IdUser = pvm.IdUser,
                Name = pvm.Name,
                reference= pvm.reference,
                status = false,
                UpdateDate = DateTime.Now,
                ImageUrl = pvm.ImageUrl
                
                
            };
            ps.Add(t);
            ps.Commit();

            // Sauvgarde de l'image

            var path = Path.Combine(Server.MapPath("~/Content/Upload/"), Image.FileName);
            Image.SaveAs(path);
            return RedirectToAction("Index");

        }

        // GET: Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product p = new Product();
            p = ps.GetById((long)id);

            ProductViewModel pvm = new ProductViewModel
            {
                CategoryId = p.IdCategory,
                CreationDate = p.CreationDate,
                CurrentPrice = p.CurrentPrice,
                IdUser = getCurrentUser().Id,
                Name = p.Name,
                reference = p.reference,
                status = p.status,
                UpdateDate = DateTime.Now, 
                ImageUrl = p.ImageUrl

            };
            List<Category> Categories = cs.GetAll().ToList();
            pvm.Category = Categories.ToSelectListItems();
            return View(pvm);
        }

        // POST: Product/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, ProductViewModel pvm , HttpPostedFileBase Image)
        {
            Product p = new Product();
            p = ps.GetById(id);
            p.ImageUrl = Image.FileName;
            p.IdCategory = pvm.CategoryId;
            p.CreationDate = pvm.CreationDate;
            p.CurrentPrice = pvm.CurrentPrice;
            p.Name = pvm.Name;
            p.reference = pvm.reference;
            p.status = pvm.status;
            p.UpdateDate = DateTime.Now;

            


            ps.Update(p);
            ps.Commit();
            var path = Path.Combine(Server.MapPath("~/Content/Upload/"), Image.FileName);
            Image.SaveAs(path);
            return RedirectToAction("Index");

        }

        // GET: Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product p = ps.GetById((long)id);
            if (p == null)
            {
                return HttpNotFound();
            }
            ProductViewModel pvm = new ProductViewModel
            {
                CategoryId = p.IdCategory,
                CreationDate = p.CreationDate,
                CurrentPrice = p.CurrentPrice,
                IdUser = getCurrentUser().Id,
                Name = p.Name,
                reference = p.reference,
                status = p.status,
                UpdateDate = p.UpdateDate,
                ImageUrl = p.ImageUrl

            };
            return View(pvm);
        }

        // POST: Product/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Product p = ps.GetById((long)id);
            ps.Delete(p);
            ps.Commit();
            return RedirectToAction("Index");
        }

#endregion


        #region Specific Methods 

        public ActionResult getNewProducts()
        {
            List<ProductViewModel> pvm = new List<ProductViewModel>();
            List<Product> prod = ps.NewProducts().ToList();

            foreach (var item in prod)
            {
                pvm.Add(
                    new ProductViewModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        CreationDate = item.CreationDate,
                        CategoryId = item.IdCategory,
                        CurrentPrice = item.CurrentPrice,
                        reference = item.reference,
                        status = item.status,
                        UpdateDate = item.UpdateDate,
                        ImageUrl = item.ImageUrl,
                        //IdUser = item.IdUser
                    });
            }

            return View(pvm);
        }


        public ActionResult getAvailableProducts()
        {
            List<ProductViewModel> pvm = new List<ProductViewModel>();
            List<Product> prod = ps.availableProduct().ToList();

            foreach (var item in prod)
            {
                pvm.Add(
                    new ProductViewModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        CreationDate = item.CreationDate,
                        CategoryId = item.IdCategory,
                        CurrentPrice = item.CurrentPrice,
                        reference = item.reference,
                        status = item.status,
                        UpdateDate = item.UpdateDate,
                        ImageUrl = item.ImageUrl,
                        //IdUser = item.IdUser
                    });
            }

            return View(pvm);
        }

        public ActionResult getSelledProducts()
        {
            List<ProductViewModel> pvm = new List<ProductViewModel>();
            List<Product> prod = ps.selledProduct().ToList();

            foreach (var item in prod)
            {
                pvm.Add(
                    new ProductViewModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        CreationDate = item.CreationDate,
                        CategoryId = item.IdCategory,
                        CurrentPrice = item.CurrentPrice,
                        reference = item.reference,
                        status = item.status,
                        UpdateDate = item.UpdateDate,
                        ImageUrl = item.ImageUrl,
                        //IdUser = item.IdUser
                    });
            }

            return View(pvm);
        }


        public ActionResult getMyProducts()
        {
            var UserManager = new UserManager<User>(new UserStore<User>(context));
            var user = UserManager.FindById(User.Identity.GetUserId());

            List<ProductViewModel> pvm = new List<ProductViewModel>();
            List<Product> prod = ps.myProducts(user.Id).ToList();

            foreach (var item in prod)
            {
                pvm.Add(
                    new ProductViewModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        CreationDate = item.CreationDate,
                        CategoryId = item.IdCategory,
                        CurrentPrice = item.CurrentPrice,
                        reference = item.reference,
                        status = item.status,
                        UpdateDate = item.UpdateDate,
                        ImageUrl = item.ImageUrl,
                        IdUser = item.User.Id
                    });
            }

            return View(pvm);
        }
        #endregion
    }
}
