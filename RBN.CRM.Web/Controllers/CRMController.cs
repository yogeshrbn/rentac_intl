using Microsoft.AspNetCore.Mvc;

namespace RBN.CRM.Web.Controllers
{
    public class CRMController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
