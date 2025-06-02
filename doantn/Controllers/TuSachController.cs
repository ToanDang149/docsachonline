using doantn.Services;
using doantn.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace doantn.Controllers
{
    public class TuSachController : Controller
    {
        private readonly TuSachService _tuSachService;

        public TuSachController(TuSachService tuSachService)
        {
            _tuSachService = tuSachService;
        }

        [HttpPost]
        [Route("TuSach/Luu")]
        public async Task<IActionResult> LuuVaoTuSach([FromBody] LuuTuSachRequest request)
        {
            string maKhachHang = HttpContext.Session.GetString("MaKhachHang");
            if (HttpContext.Session.GetString("MaKhachHang") == null)
            {
                return Unauthorized(); 
            }
      
            bool daLuu = await _tuSachService.KiemTraDaLuu(maKhachHang, request.MaSach);
            if (daLuu)
            {
                await _tuSachService.XoaKhoiTuSach(maKhachHang, request.MaSach);
            }
            else
            {
                await _tuSachService.LuuVaoTuSach(maKhachHang, request.MaSach);
            }
            return Ok(new { daLuu = !daLuu });
        }
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("MaKhachHang") == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            string maKhachHang = HttpContext.Session.GetString("MaKhachHang");

            var danhSachTuSach = await _tuSachService.LayDanhSachTuSachAsync(maKhachHang);

            return View(danhSachTuSach);
        }
        [HttpPost]
        [Route("TuSach/Xoa")]
        public async Task<IActionResult> XoaTuSach([FromBody] LuuTuSachRequest request)
        {
            if (HttpContext.Session.GetString("MaKhachHang") == null)
            {
                return Unauthorized();
            }

            string maKhachHang = HttpContext.Session.GetString("MaKhachHang");

            await _tuSachService.XoaKhoiTuSach(maKhachHang, request.MaSach);

            return Ok();
        }
    }
}
