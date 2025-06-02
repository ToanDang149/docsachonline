using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace doantn.Attributes
{
    public class AuthorizeCapAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _requiredCap;

        public AuthorizeCapAttribute(string requiredCap)
        {
            _requiredCap = requiredCap;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var maCap = context.HttpContext.Session.GetString("Cap");

            if (string.IsNullOrEmpty(maCap))
            {
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", null);
                return;
            }

            if (maCap != _requiredCap)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}
