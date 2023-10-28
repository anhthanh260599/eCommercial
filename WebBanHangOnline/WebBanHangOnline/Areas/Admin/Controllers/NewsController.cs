﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    public class NewsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/News
        public ActionResult Index()
        {
            var items = db.News.OrderByDescending(x=>x.Id).ToList();
            return View(items);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(News model, bool preview = false) 
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin người dùng hiện tại
                var currentUserId = User.Identity.GetUserId(); // Sử dụng UserManager để lấy UserId của người dùng hiện tại

                // Sử dụng DbContext để tìm ApplicationUser có UserId tương ứng
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = userManager.FindById(currentUserId);

                if (currentUser != null)
                {
                    // Gán giá trị FullName của người dùng hiện tại cho CreateBy của đối tượng News
                    model.CreateBy = currentUser.FullName;
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy người dùng
                    // Chẳng hạn, bạn có thể gán một giá trị mặc định hoặc xử lý khác
                    model.CreateBy = "Người dùng không tồn tại"; // Ví dụ
                }

                if (preview)
                {
                    model.IsActive = false;
                }

                model.CreateDate = DateTime.Now;
                model.CategoryID = 18; // CategoryID của Tin tức (check trong DB)
                model.ModifierDate = DateTime.Now;
                model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title);  // chuyển có dấu thành không dấu, mục đích để làm url sau này
                db.News.Add(model);
                db.SaveChanges();


                if (preview)
                {
                    // Nếu truyền tham số preview = true, thì chuyển hướng đến trang Preview với tham số id là model.Id
                    return RedirectToAction("Preview", new { id = model.Id });
                }
                else
                {
                    // Ngược lại, chuyển hướng đến trang Index hoặc trang khác tùy theo logic của ứng dụng của bạn
                    return RedirectToAction("Index");
                }
                //return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Preview(int id)
        {
            var item = db.News.Find(id);
            return View(item);
        }

        public ActionResult Edit(int id)
        {
            var item = db.News.Find(id);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(News model, bool preview = false)
        {
            if (ModelState.IsValid)
            {
                db.News.Attach(model);

                // Lấy thông tin người dùng hiện tại
                var currentUserId = User.Identity.GetUserId(); // Sử dụng UserManager để lấy UserId của người dùng hiện tại

                // Sử dụng DbContext để tìm ApplicationUser có UserId tương ứng
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = userManager.FindById(currentUserId);

                if (currentUser != null)
                {
                    // Gán giá trị FullName của người dùng hiện tại cho CreateBy của đối tượng News
                    model.CreateBy = currentUser.FullName;
                }
                else
                {
                    // Xử lý trường hợp không tìm thấy người dùng
                    // Chẳng hạn, bạn có thể gán một giá trị mặc định hoặc xử lý khác
                    model.CreateBy = "Người dùng không tồn tại"; // Ví dụ
                }

                if(preview)
                {
                    model.IsActive = false;
                }

                model.ModifierDate = DateTime.Now;
                model.Alias = WebBanHangOnline.Models.Common.Filter.FilterChar(model.Title); // chuyển có dấu thành không dấu, mục đích để làm url sau này
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();


                if (preview)
                {
                    // Nếu truyền tham số preview = true, thì chuyển hướng đến trang Preview với tham số id là model.Id
                    return RedirectToAction("Preview", new { id = model.Id });
                }
                else
                {
                    // Ngược lại, chuyển hướng đến trang Index hoặc trang khác tùy theo logic của ứng dụng của bạn
                    return RedirectToAction("Index");
                }

                //return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id) 
        {

            var item = db.News.Find(id);
            if(item != null) 
            {
                db.News.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult Delete_Preview(int id)
        {

            var item = db.News.Find(id);
            if (item != null)
            {
                db.News.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult IsActive(int id)
        {

            var item = db.News.Find(id);
            if (item != null)
            {
                item.IsActive =! item.IsActive;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, IsActive = item.IsActive });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult DeleteSelected (string ids)
        {
            if(!string.IsNullOrEmpty(ids))
            {
                var items = ids.Split(',');
                if(items!=null && items.Any())
                {
                    foreach(var item in items)
                    {
                        var obj = db.News.Find(Convert.ToInt32(item));
                        db.News.Remove(obj);
                        db.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}