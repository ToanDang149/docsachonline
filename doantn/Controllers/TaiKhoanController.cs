using doantn.Services;
using doantn.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace doantn.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly IUserService _userService;

        public TaiKhoanController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DangKy(DangKyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var taiKhoanTonTai = await _userService.CheckTaiKhoanTonTai(model.TaiKhoan);
                if (taiKhoanTonTai)
                {
                    TempData["DangKyError"] = "Tài khoản đã tồn tại.";
                    return View(model);
                }

                var emailTonTai = await _userService.CheckEmailTonTai(model.Email);
                if (emailTonTai)
                {
                    TempData["DangKyError"] = "Email đã được sử dụng.";
                    return View(model);
                }

                var (isSuccess, message) = await _userService.DangKy(model);
                if (isSuccess)
                {
                    TempData["DangKySuccess"] = "Đăng ký thành công! Bạn có thể đăng nhập ngay.";
                    return RedirectToAction("DangKy");
                }

                TempData["DangKyError"] = message;
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult DangNhap(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DangNhap(DangNhapViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user = await _userService.GetByLogin(model.TaiKhoan, model.MatKhau);
            if (user != null)
            {
                HttpContext.Session.SetString("MaKhachHang", user.MaKhachHang);
                HttpContext.Session.SetString("TaiKhoan", user.TaiKhoan);
                HttpContext.Session.SetString("HoTen", user.HoTen);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("SoDT", user.SoDT ?? "");
                HttpContext.Session.SetInt32("GioiTinh", user.GioiTinh);
                HttpContext.Session.SetString("Cap", user.MaCap);
                TempData["DangNhapSuccess"] = "Đăng nhập thành công!";
                return RedirectToAction("DangNhap", new { returnUrl });
            }
            TempData["DangNhapError"] = "Tài khoản hoặc mật khẩu không đúng.";
            return RedirectToAction("DangNhap");
        }
        [HttpGet]
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}
