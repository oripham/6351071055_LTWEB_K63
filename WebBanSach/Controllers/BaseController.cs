using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models;

namespace WebBanSach.Controllers
{
    public class BaseController : Controller
    {


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            QLBANSACHEntities _context = new QLBANSACHEntities();
            ViewBag.chudeList = _context.CHUDEs.ToList();
            ViewBag.nhaxuatbanList = _context.NHAXUATBANs.ToList();
        }
    }
}