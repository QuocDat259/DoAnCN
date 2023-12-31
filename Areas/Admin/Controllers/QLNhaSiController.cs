﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using NhaKhoa.Models;
using System.Threading.Tasks;
using System.Text;
using PagedList;
using PagedList.Mvc;
using System.Globalization;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace NhaKhoa.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QLNhaSiController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();
        private readonly UserManager<ApplicationUser> _userManager;
        public QLNhaSiController()
        {
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }

        // GET: Admin/QLNhanVien
        public ActionResult Index()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            // Lấy thông tin người dùng đã đăng nhập
            var userId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(userId);
            
            // Lấy danh sách người dùng thuộc vai trò "NhaSi"
            var nhanVienUsers = userManager.Users.Where(u => u.Roles.Any(r => r.RoleId == "2")).ToList();
            ViewBag.HinhAnh = user.HinhAnh;
            return View(nhanVienUsers);
        }


        // GET: Admin/QLNhaSi/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // GET: Admin/QLNhaSi/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/QLNhaSi/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.UserName,
                    GioiTinh = model.GioiTinh,
                    DiaChi = model.DiaChi,
                    NgaySinh = model.NgaySinh,
                    NgheNghiep = model.NgheNghiep,
                    NgayTao = model.NgayTao,
                    CCCD = model.CCCD,
                    FullName = model.FullName
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Gán quyền "NhaSi" cho tài khoản người dùng
                    await _userManager.AddToRoleAsync(user.Id, "NhaSi");
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View(model);
        }

        // GET: Admin/QLNhaSi/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: Admin/QLNhaSi/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,GioiTinh,DiaChi,Trangthai,Ngaysinh,NgheNghiep,NgayTao,Bangcap,CCCD,FullName")] AspNetUsers aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetUser);
        }

        // GET: Admin/QLNhaSi/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: Admin/QLNhaSi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUsers aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
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
        public ActionResult TKB(DateTime? selectedWeek, string fullName)
        {
            try
            {
                var danhSachThu = db.Thu.ToList();
                var danhSachThoiKhoaBieu = db.ThoiKhoaBieu.OrderBy(e => e.Id_Thu).ThenBy(e => e.NgayLamViec).ToList();
                ViewBag.FullNames = db.AspNetUsers
                .Where(u => u.AspNetRoles.Any(r => r.Name == "NhaSi"))  // Adjust this condition based on your actual data model
                .Select(user => user.FullName)
                .Distinct()
                .ToList();
                ViewBag.fullName = fullName;
                if (danhSachThu.Any() || danhSachThoiKhoaBieu.Any())
                {
                    var now = DateTime.Now;
                    var daysUntilMonday = ((int)now.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
                    var monday = now.AddDays(-daysUntilMonday);
                    DateTime startOfWeek = selectedWeek ?? monday;

                    // Simplify the logic for filtering danhSachThoiKhoaBieu
                    danhSachThoiKhoaBieu = danhSachThoiKhoaBieu
                        .Where(tkb => tkb.NgayLamViec.HasValue && tkb.NgayLamViec.Value.Date == startOfWeek.Date &&
                                      (string.IsNullOrEmpty(fullName) || tkb.AspNetUsers.FullName == fullName))
                        .OrderBy(e => e.Id_Thu)
                        .ThenBy(e => e.NgayLamViec)
                        .ToList();

                    GregorianCalendar calendar = new GregorianCalendar();
                    DateTime[] weeks = GetWeeksInYear(startOfWeek.Year, calendar);

                    if (startOfWeek.Year > DateTime.Now.Year + 1)
                    {
                        return RedirectToAction("TKB", "QLNhaSi");
                    }

                    var viewModel = new ThoiKhoaBieuViewModel
                    {
                        DanhSachThu = danhSachThu,
                        DanhSachThoiKhoaBieu = danhSachThoiKhoaBieu,
                        weeks = weeks,
                        SelectedWeek = startOfWeek
                    };

                    return View(viewModel);
                }
                else
                {
                    ViewBag.ErrorMessage = "Không có dữ liệu để hiển thị.";
                    return View("ErrorView");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Đã xảy ra lỗi khi lấy dữ liệu: {ex.Message}";
                return View("ErrorView");
            }
        }

        //private DateTime[] GetWeeksInYear(int year, GregorianCalendar calendar)
        //{
        //    List<DateTime> weeks = new List<DateTime>();

        //    // Ngày đầu tiên của năm
        //    DateTime startDate = new DateTime(year, 1, 1);

        //    // Đếm số ngày đã được thêm vào mảng
        //    int daysAdded = 0;

        //    // Duyệt qua từng ngày trong năm
        //    for (int i = 0; i < 365; i++)
        //    {
        //        DateTime currentDate = startDate.AddDays(i);

        //        // Nếu là ngày đầu tiên của một tuần, thêm vào mảng
        //        if (currentDate.DayOfWeek == DayOfWeek.Monday)
        //        {
        //            weeks.Add(currentDate);
        //            daysAdded++;
        //        }
        //    }

        //    return weeks.ToArray();
        //}

        // Phương thức chọn năm, mặc định lấy năm hiện tại
        static DateTime[] GetWeeksInYear(int year, GregorianCalendar calendar)
        {
            int totalWeeks = calendar.GetWeekOfYear(new DateTime(year, 12, 31), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            DateTime[] weeks = new DateTime[totalWeeks];

            // Ngày đầu tiên của năm
            DateTime startDate = new DateTime(year, 1, 1);

            // Đếm số ngày đã được thêm vào mảng
            int daysAdded = 0;

            // Duyệt qua từng ngày trong năm
            for (int i = 0; i < totalWeeks * 7; i++)
            {
                DateTime currentDate = startDate.AddDays(i);

                // Nếu là ngày đầu tiên của một tuần, thêm vào mảng
                if (currentDate.DayOfWeek == DayOfWeek.Monday)
                {
                    weeks[calendar.GetWeekOfYear(currentDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) - 1] = currentDate;
                    daysAdded++;
                }
            }

            return weeks;
        }

        public ActionResult ThemThoiKhoaBieu()
        {
            ViewBag.ListPhong = new SelectList(db.Phong, "Id_Phong", "TenPhong");
            ViewBag.ListNhaSi = new SelectList(db.AspNetUsers.Where(u => u.AspNetRoles.Any(r => r.Id == "2")), "Id", "FullName");
            ViewBag.ListKhungGio = new SelectList(db.KhungGio, "Id_khunggio", "TenCa");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemThoiKhoaBieu(ThoiKhoaBieu thoiKhoaBieu)
        {
            if (ModelState.IsValid)
            {
                if (thoiKhoaBieu.NgayLamViec < DateTime.Now.Date)
                {
                    return AddValidationErrorAndReturnView("Ngày làm việc đã quá ngày, Vui lòng chọn ngày khác", thoiKhoaBieu);
                }

                if (thoiKhoaBieu.NgayLamViec == DateTime.Now.Date)
                {
                    return AddValidationErrorAndReturnView("Vui lòng chọn ngày làm việc khác ngày hiện tại", thoiKhoaBieu);
                }

                NgayVaThu ngayVaThu = LayNgayVaThu(thoiKhoaBieu.NgayLamViec);

                thoiKhoaBieu.NgayLamViec = ngayVaThu.NgayLamViec;
                thoiKhoaBieu.Id_Thu = ngayVaThu.IdThu;

                if (thoiKhoaBieu.NgayLamViec == null)
                {
                    return AddValidationErrorAndReturnView("Vui lòng chọn ngày làm việc", thoiKhoaBieu);
                }

                if (thoiKhoaBieu.Id_Nhasi == null )
                {
                    return AddValidationErrorAndReturnView("Vui lòng chọn Nha Sĩ", thoiKhoaBieu);
                }

                if (thoiKhoaBieu.Id_Phong == null)
                {
                    return AddValidationErrorAndReturnView("Vui lòng chọn phòng làm việc", thoiKhoaBieu);
                }

                if (thoiKhoaBieu.Id_khunggio == null || thoiKhoaBieu.Id_khunggio == 0)
                {
                    return AddValidationErrorAndReturnView("Vui lòng chọn ca làm việc", thoiKhoaBieu);
                }

                if (KiemTraTrungLich(thoiKhoaBieu))
                {
                    return AddValidationErrorAndReturnView("Lịch làm việc đã trùng. Vui lòng chọn lịch khác.", thoiKhoaBieu);
                }

                db.ThoiKhoaBieu.Add(thoiKhoaBieu);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            SetupDropdownLists();
            return View(thoiKhoaBieu);
        }
        private ActionResult AddValidationErrorAndReturnView(string errorMessage, ThoiKhoaBieu model)
        {
            ModelState.AddModelError(string.Empty, errorMessage);
            SetupDropdownLists();
            return View(model);
        }
        // Hàm kiểm tra trùng lịch
        private bool KiemTraTrungLich(ThoiKhoaBieu thoiKhoaBieu)
        {
            // Kiểm tra xem lịch làm việc mới có trùng với lịch làm việc đã tồn tại không
            return db.ThoiKhoaBieu.Any(t => t.Id_Phong == thoiKhoaBieu.Id_Phong &&
                                            t.Id_Nhasi == thoiKhoaBieu.Id_Nhasi &&
                                            t.Id_khunggio == thoiKhoaBieu.Id_khunggio &&
                                            t.NgayLamViec == thoiKhoaBieu.NgayLamViec);
        }

        // Hàm thiết lập danh sách dropdown
        private void SetupDropdownLists()
        {
            ViewBag.ListPhong = new SelectList(db.Phong.Select(p => new { p.Id_Phong, p.TenPhong }), "Id_Phong", "TenPhong");
            ViewBag.ListNhaSi = new SelectList(db.AspNetUsers.Where(u => u.AspNetRoles.Any(r => r.Id == "2")), "Id", "FullName");
            ViewBag.ListKhungGio = new SelectList(db.KhungGio, "Id_khunggio", "TenCa");
        }


        public class NgayVaThu
        {
            public DateTime? NgayLamViec { get; set; }
            public int IdThu { get; set; }
        }

        private NgayVaThu LayNgayVaThu(DateTime? ngayLamViec)
        {
            if (ngayLamViec.HasValue)
            {
                // Lấy thứ từ ngày
                string[] daysOfWeek = { "Chủ nhật", "Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy" };
                string tenThu = daysOfWeek[(int)ngayLamViec.Value.DayOfWeek];

                // Lấy Id của TenThu từ bảng Thu
                int idThu = db.Thu.Single(t => t.TenThu == tenThu).Id_Thu;

                // Trả về đối tượng chứa ngày và Id của TenThu
                return new NgayVaThu { NgayLamViec = ngayLamViec, IdThu = idThu };
            }
            else
            {
                // Trường hợp ngày làm việc là null
                return new NgayVaThu { NgayLamViec = null, IdThu = -1 }; // Modify this default value based on your actual model
            }
        }

    }
}
