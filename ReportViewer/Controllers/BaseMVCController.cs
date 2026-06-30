using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReportViewer.Controllers
{
    public class BaseMVCController : Controller
    {
        // GET: BaseMVC
        

        public   string RenderRazorViewToString(Controller controller, string viewName, object model = null)
        {
            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.ToString();
            }
        }
    }
}
public class AllowJsonGetAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
        var jsonResult = filterContext.Result as JsonResult;
        if (jsonResult != null)
        {
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        }
        base.OnResultExecuting(filterContext);
    }
}