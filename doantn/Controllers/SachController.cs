using doantn.Services;
using doantn.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace doantn.Controllers
{
    public class SachController : Controller
    {
        private readonly SachService _sachService;
        private readonly KhachHangService _khachHangService;
        private readonly TuSachService _tuSachService;
        private readonly DanhGiaService _danhGiaService; 
        private readonly FileSachService _fileSachService;

        public SachController(SachService sachService, KhachHangService khachHangService, TuSachService tuSachService, DanhGiaService danhGiaService, FileSachService fileSachService)
        {
            _sachService = sachService;
            _khachHangService = khachHangService;
            _tuSachService = tuSachService;
            _danhGiaService = danhGiaService;
            _fileSachService = fileSachService;
        }

        public async Task<IActionResult> ChiTiet(string maSach)
        {
            var sach = await _sachService.GetChiTiet(maSach);
            var danhSachFile = await _fileSachService.GetDanhSachFile(maSach);
            ViewBag.DanhSachFile = danhSachFile;
            bool daLuu = false;

            if (HttpContext.Session.GetString("MaKhachHang") != null)
            {
                var maKhachHang = HttpContext.Session.GetString("MaKhachHang");
                daLuu = await _tuSachService.KiemTraDaLuu(maKhachHang, maSach);
            }
            var danhGias = await _danhGiaService.GetDanhGiaBySach(maSach);

            ViewBag.DanhGias = danhGias;

            // Tính trung bình số sao
            if (danhGias != null && danhGias.Any())
            {
                var trungBinhSao = danhGias.Average(dg => dg.SoSao) ?? 0;
                ViewBag.TrungBinhSao = Math.Round(trungBinhSao, 1);
                ViewBag.SoLuongDanhGia = danhGias.Count();
            }
            else
            {
                ViewBag.TrungBinhSao = 0;
                ViewBag.SoLuongDanhGia = 0;
            }
            ViewBag.DaLuu = daLuu;
            if (sach == null)
                return NotFound();
            var cungLoai = await _sachService.GetCungTheLoai(sach.MaLoai, sach.MaSach);
            ViewBag.SachCungChuDe = cungLoai;
            var danhGiaList = await _danhGiaService.GetDanhGiaBySach(maSach);
            ViewBag.DanhGias = danhGiaList;
            return View(sach);
        }
        public async Task<IActionResult> DocSach(string maSach)
        {
            var viewModel = await _sachService.GetChiTietSachCuon(maSach);
            if (viewModel == null || viewModel.ChuongNoiDung == null || !viewModel.ChuongNoiDung.Any())
                return NotFound();
            if (HttpContext.Session.GetString("MaKhachHang") != null)
            {
                string maKhachHang = HttpContext.Session.GetString("MaKhachHang");

                bool daDoc = await _khachHangService.KiemTraDaDocSach(maKhachHang, maSach);

                if (!daDoc)
                {
                    await _khachHangService.LuuSachDaDoc(maKhachHang, maSach);

                    await _sachService.TangLuotXem(maSach,maKhachHang);
                }
            }
            return View("DocSachCuon", viewModel); 
        }
        [HttpGet("/Sach/DocSachCuon")]
        public async Task<IActionResult> DocSachCuon(string maSach)
        {
            var viewModel = await _sachService.GetChiTietSachAsync(maSach);
            if (viewModel == null || viewModel.ChuongNoiDung == null || !viewModel.ChuongNoiDung.Any())
                return NotFound();

            return View("DocSach", viewModel);
        }
        public IActionResult DanhSach(string maLoai)
        {
            var dsSach = _sachService.GetSachByLoai(maLoai);;

            var model = new DanhSachSachViewModel
            {
                MaLoai = maLoai,
                DanhSachSach = dsSach
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> TaiFile(string maSach, string dinhDang)
        {
            var maKhachHang = HttpContext.Session.GetString("MaKhachHang");
            var cap = HttpContext.Session.GetString("Cap");

            if (string.IsNullOrEmpty(maKhachHang) || string.IsNullOrEmpty(cap))
            {
                return Json(new { status = "not_logged_in" });
            }

            var capHopLe = new[] { "c2", "c3", "c4" };
            if (!capHopLe.Contains(cap.ToLower()))
            {
                return Json(new { status = "unauthorized" });
            }

            var file = await _fileSachService.GetFile(maSach, dinhDang);
            if (file == null || string.IsNullOrWhiteSpace(file.DuongDan))
            {
                return Json(new { status = "not_found" });
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.DuongDan.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
            {
                return Json(new { status = "not_found" });
            }

            await _fileSachService.GhiLichSuTaiNeuChuaCo(maSach, maKhachHang);

            var downloadUrl = Url.Action("DownloadFile", "Sach", new { maSach, dinhDang });
            return Json(new { status = "success", downloadUrl });
        }
        [HttpGet]
        public async Task<IActionResult> DownloadFile(string maSach, string dinhDang)
        {
            var file = await _fileSachService.GetFile(maSach, dinhDang);
            if (file == null || string.IsNullOrWhiteSpace(file.DuongDan))
                return NotFound();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.DuongDan.TrimStart('/'));
            if (!System.IO.File.Exists(path))
                return NotFound();

            var fileBytes = await System.IO.File.ReadAllBytesAsync(path);
            var fileName = $"{file.TenFile.Trim()}.{file.LoaiFile.Trim().ToLower()}";

            return File(fileBytes, "application/octet-stream", fileName); 
        }

    }
}
