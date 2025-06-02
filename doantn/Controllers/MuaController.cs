using doantn.Services;
using doantn.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace doantn.Controllers
{
    public class MuaController : Controller
    {
        private readonly MuaService _muaService;
        public MuaController(MuaService muakService)
        {
            _muaService = muakService;
        }
        public IActionResult DichVu()
        {
            return View(); 
        }
        [HttpGet]
        public async Task<IActionResult> GetThongTinCap(string maCap)
        {
            var cap = await _muaService.GetCapByMaCap(maCap);
            if (cap == null)
                return NotFound(new { success = false, message = "Không tìm thấy gói cấp." });

            return Json(new { success = true, data = new { cap.TenCap, cap.Gia } });
        }

        [HttpPost]
        public async Task<IActionResult> MuaGoi([FromBody] MuaGoiVM model)
        {
            var maKhachHang = HttpContext.Session.GetString("MaKhachHang");
            var maCap = model.MaCap;

            if (string.IsNullOrEmpty(maKhachHang))
                return Json(new { success = false, message = "Bạn chưa đăng nhập." });

            var cap = await _muaService.GetCapByMaCap(maCap);
            if (cap == null)
                return Json(new { success = false, message = "Gói dịch vụ không tồn tại." });

            var capNhatOk = await _muaService.CapNhatCapNguoiDung(maKhachHang, maCap);
            if (!capNhatOk)
                return Json(new { success = false, message = "Không thể cập nhật cấp tài khoản." });

            var ghiLichSuOk = await _muaService.GhiLichSuMua(maKhachHang, maCap, cap.Gia);

            if (ghiLichSuOk)
            {
                HttpContext.Session.SetString("Cap", maCap);
                return Json(new { success = true, message = $"Bạn đã mua gói {cap.TenCap} thành công!" });
            }
            else
            {
                return Json(new { success = false, message = "Không thể ghi lịch sử mua." });
            }
        }

    }
}
