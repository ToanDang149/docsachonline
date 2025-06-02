using doantn.Models;
using doantn.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace doantn.Controllers
{
    public class DanhGiaController : Controller
    {
        private readonly DanhGiaService _danhGiaService;

        public DanhGiaController(DanhGiaService danhGiaService)
        {
            _danhGiaService = danhGiaService;
        }

        [HttpPost]
        [Route("DanhGia/Luu")]
        public async Task<IActionResult> LuuDanhGia([FromForm] string maSach, [FromForm] int soSao, [FromForm] string noiDung)
        {
            var maKhachHang = HttpContext.Session.GetString("MaKhachHang");

            if (string.IsNullOrEmpty(maKhachHang))
            {
                return Unauthorized(new { success = false, message = "Chưa đăng nhập." });
            }

            var danhGiaCu = await _danhGiaService.GetDanhGia(maSach, maKhachHang);

            if (danhGiaCu != null)
            {
                danhGiaCu.SoSao = soSao;
                danhGiaCu.NoiDung = noiDung;
                danhGiaCu.ThoiGianDanhGia = DateTime.Now;
                await _danhGiaService.UpdateDanhGia(danhGiaCu);
            }
            else
            {
                var danhGiaMoi = new DanhGia
                {
                    MaSach = maSach,
                    MaKhachHang = maKhachHang,
                    SoSao = soSao,
                    NoiDung = noiDung,
                    ThoiGianDanhGia = DateTime.Now
                };
                await _danhGiaService.AddDanhGia(danhGiaMoi);
            }
            var danhGias = await _danhGiaService.GetDanhGiaBySach(maSach);
            double trungBinh = 0;
            int soLuong = 0;

            if (danhGias != null && danhGias.Any())
            {
                trungBinh = Math.Round(danhGias.Average(dg => dg.SoSao) ?? 0, 1);
                soLuong = danhGias.Count();
            }

            return Ok(new
            {
                success = true,
                message = "",
                trungBinhSao = trungBinh,
                soLuongDanhGia = soLuong
            });
        }
        [HttpGet]
        [Route("DanhGia/DanhSachDanhGia")]
        public async Task<IActionResult> DanhSachDanhGia(string maSach)
        {
            var danhGias = await _danhGiaService.GetDanhGiaBySach(maSach);
            return PartialView("_DanhSachDanhGiaPartial", danhGias);
        }

    }
}
