using doantn.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace doantn.Controllers
{
    public class ThuVienController : Controller
    {
        private readonly SachService _sachService;
        public ThuVienController(SachService sachService)
        {
            _sachService = sachService;
        }
        public IActionResult Index()
        {
            var model = new TrangChuViewModel
            {
                EbookMoi = _sachService.GetSachMoi(),
            };

            return View(model);
        }
    }
}
