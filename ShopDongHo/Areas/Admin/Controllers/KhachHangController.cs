using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using ShopDongHo.Models;
using ShopDongHo.Libs;
using System.Data.Entity.Infrastructure;

namespace ShopDongHo.Areas.Admin.Controllers
{
    public class KhachHangController : AuthController
    {
        private ShopDongHoEntities db = new ShopDongHoEntities();

        // GET: KhachHang
        public ActionResult Index()
        {
            return View(db.KhachHang.ToList());
        }

        public ActionResult Logout()
        {
            // Xóa SESSION
            Session.RemoveAll();

            // Quay về trang chủ
            return RedirectToAction("Index", "Home");
        }

        
        // GET: KhachHang/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KhachHang/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,HoVaten,DienThoai,DiaChi,TenDangNhap,MatKhau,XacNhanMatKhau")] KhachHang khachHang)
        {
           /*if (ModelState.IsValid)
            {
                khachHang.MatKhau = Libs.SHA1.ComputeHash(khachHang.MatKhau);
                khachHang.XacNhanMatKhau = Libs.SHA1.ComputeHash(khachHang.XacNhanMatKhau);

                db.KhachHang.Add(khachHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            
           
            }

            return View(khachHang);
           */

            if (ModelState.IsValid)
            {
                var check = db.KhachHang.FirstOrDefault(r => r.TenDangNhap == khachHang.TenDangNhap);
                if (check == null)
                {
                    khachHang.MatKhau = Libs.SHA1.ComputeHash(khachHang.MatKhau);
                    //khachHang.XacNhanMatKhau = Libs.SHA1.ComputeHash(khachHang.XacNhanMatKhau);

                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.KhachHang.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Tên đăng nhập đã tồn tại !!!";
                    return View();
                }
            }
            return View();
        }

        // GET: KhachHang/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHang.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(new KhachHangEdit(khachHang));
        }

        // POST: KhachHang/Edit/5
        // Để bảo vệ khỏi các cuộc tấn công liên quan đến tràn bộ nhớ đệm, hãy kích hoạt các thuộc tính cụ thể mà bạn muốn gắn kết
        // để biết thêm chi tiết xem  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,HoVaten,DienThoai,DiaChi,TenDangNhap,MatKhau,XacNhanMatKhau")] KhachHangEdit khachHang)
        {
            if (ModelState.IsValid)
            {
                KhachHang n = db.KhachHang.Find(khachHang.ID);

                // Giữ nguyên mật khẩu cũ
                if (khachHang.MatKhau == null)
                {
                    n.ID = khachHang.ID;
                    n.HoVaten = khachHang.HoVaten;
                    n.DienThoai = khachHang.DienThoai;
                    n.DiaChi = khachHang.DiaChi;
                    n.TenDangNhap = khachHang.TenDangNhap;
                    //n.XacNhanMatKhau = n.MatKhau;

                }
                else // Cập nhật mật khẩu mới
                {
                    n.ID = khachHang.ID;
                    n.HoVaten = khachHang.HoVaten;
                    n.DienThoai = khachHang.DienThoai;
                    n.DiaChi = khachHang.DiaChi;
                    n.TenDangNhap = khachHang.TenDangNhap;
                    n.MatKhau =Libs.SHA1.ComputeHash(khachHang.MatKhau);
                    //n.XacNhanMatKhau = Libs.SHA1.ComputeHash(khachHang.XacNhanMatKhau);

                }
                db.Entry(n).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(khachHang);
        }

        // GET: KhachHang/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHang.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

        // POST: KhachHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                KhachHang khachHang = db.KhachHang.Find(id);
                if (khachHang != null)
                {
                    db.KhachHang.Remove(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    // Người dùng không tồn tại, thông báo lỗi.
                    TempData["ErrorMessage"] = "Không thể xóa khách hàng của bạn.";
                    return View(); // Trả về trang hiện tại.
                }
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    Console.WriteLine(innerException.Message);
                    innerException = innerException.InnerException;
                }

                // Xử lý lỗi ở đây và thông báo lỗi.
                TempData["ErrorMessage"] = "Không thể xóa khách hàng của bạn.";
                return View(); // Trả về trang hiện tại.
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
