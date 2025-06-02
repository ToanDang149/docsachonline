using doantn.Services;
using Microsoft.AspNetCore.Mvc;

namespace doantn.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly ThongKeService _thongKeService;
        private readonly SachService _sachService;
        public ThongKeController(ThongKeService thongKeService, SachService sachService)
        {
            _thongKeService = thongKeService;
            _sachService = sachService;
        }

        [HttpGet]
        public async Task<IActionResult> Sach(string maSach)
        {
            var sach = await _sachService.GetChiTiet(maSach);
            if (sach == null)
                return NotFound();
            var tongXem = await _thongKeService.TinhTongLuotXem(maSach);
            var tongTai = await _thongKeService.TinhTongLuotTai(maSach);
            var topXem = await _thongKeService.TinhLuotXemCaoNhatThang(maSach);
            var topTai = await _thongKeService.TinhLuotTaiCaoNhatThang(maSach);
            var danhGia = await _thongKeService.ThongKeDanhGia(maSach);

            ViewBag.DiemTrungBinh = danhGia.DiemTrungBinh;
            ViewBag.TongDanhGia = danhGia.TongDanhGia;
            ViewBag.StarCounts = danhGia.StarCounts;
            ViewBag.MaxStar = danhGia.StarCounts.Values.Any() ? danhGia.StarCounts.Values.Max() : 1;
            ViewData["MaSach"] = maSach;
            ViewData["TenSach"] = sach.TenSach;
            ViewData["ThoiGianCatNhap"] = sach.ThoiGianCatNhap;
            ViewData["Now"] = DateTime.Now;
            ViewBag.TongLuotXem = tongXem;
            ViewBag.TongLuotTai = tongTai;
            ViewBag.ThangXemCaoNhat = topXem.Thang;
            ViewBag.LuotXemCaoNhatThang = topXem.TongLuotXem;
            ViewBag.ThangTaiCaoNhat = topTai.Thang;
            ViewBag.LuotTaiCaoNhatThang = topTai.TongLuotTai;
            return View("ThongKeLuotXem"); 
        }
        [HttpGet]
        public async Task<IActionResult> GetLuotXemVaTai(string maSach, DateTime? from, DateTime? to)
        {
            var sach = await _sachService.GetChiTiet(maSach);
            if (sach == null) return NotFound();

            var fromDate = from ?? sach.ThoiGianCatNhap;
            var toDate = to?? DateTime.Now;

            var xemTask = _thongKeService.GetLuotXemTheoKhoangDayDu(maSach, fromDate, toDate);
            var taiTask = _thongKeService.GetLuotTaiTheoKhoangDayDu(maSach, fromDate, toDate);

            await Task.WhenAll(xemTask, taiTask);

            var result = xemTask.Result
                .Select(x => new {
                    Ngay = x.Ngay,
                    SoLuotXem = x.SoLuot,
                    SoLuotTai = taiTask.Result.FirstOrDefault(y => y.Ngay == x.Ngay)?.SoLuot ?? 0
                });
            return Json(result);
        }

    }
}
