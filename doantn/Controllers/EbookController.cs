using doantn.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace doantn.Controllers
{
    public class EbookController : Controller
    {
        private readonly SachService _sachService;
        public EbookController(SachService sachService)
        {
            _sachService = sachService;
        }
        public IActionResult Hot()
        {
            var model = new TrangChuViewModel
            {
                EbookHot = _sachService.GetSachHot(),
            };

            return View(model);
        }
        public IActionResult Moi()
        {
            var model = new TrangChuViewModel
            {
                EbookMoi = _sachService.GetSachMoi(),
            };

            return View(model);
        }
        public IActionResult TaiNhieu()
        {
            var model = new TrangChuViewModel
            {
                EbookTaiNhieu = _sachService.GetSachTaiNhieu(),
            };

            return View(model);
        }
    }

}
