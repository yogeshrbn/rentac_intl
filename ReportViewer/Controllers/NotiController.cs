using BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace ReportViewer.Controllers
{
   // [CustomActionFilter]
   
    public class NotiController : Controller
    {
        // GET: Noti
        public ActionResult Index()
        {
            return View();
        }
    
        public async Task<NotificationDto> Notify(NotificationDto dto)
        {
            return dto;
        }
    }
}