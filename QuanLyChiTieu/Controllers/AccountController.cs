using System.Linq;
using System.Web.Mvc;
using QuanLyThuChi.Data;
using QuanLyThuChi.Models;

namespace QuanLyThuChi.Controllers
{
    public class AccountController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // 🟩 [GET] Đăng ký
        public ActionResult Register()
        {
            return View();
        }

        // 🟩 [POST] Đăng ký
        [HttpPost]
        public ActionResult Register(string username, string password, string confirmPassword)
        {
            // Kiểm tra rỗng
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            // Kiểm tra trùng mật khẩu
            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            // Kiểm tra trùng username
            if (db.Users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View();
            }

            // Thêm user mới
            var user = new User
            {
                Username = username,
                Password = password // (sau có thể mã hoá)
            };

            db.Users.Add(user);
            db.SaveChanges();

            // ✅ Sử dụng TempData thay vì ViewBag
            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }


        // 🟦 [GET] Đăng nhập
        public ActionResult Login()
        {
            return View();
        }

        // 🟦 [POST] Đăng nhập
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                Session["UserId"] = user.Id;
                Session["Username"] = user.Username;
                return RedirectToAction("Index", "Transaction"); // quay về trang chính
            }

            ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
            return View();
        }

        // 🔴 Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // 🟨 [GET] Quên mật khẩu
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // 🟨 [POST] Quên mật khẩu
        [HttpPost]
        public ActionResult ForgotPassword(string username, string newPassword, string confirmPassword)
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                ViewBag.Error = "Tên đăng nhập không tồn tại!";
                return View();
            }

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ mật khẩu mới!";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            // Cập nhật mật khẩu
            user.Password = newPassword;
            db.SaveChanges();

            TempData["Success"] = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

    }
}
