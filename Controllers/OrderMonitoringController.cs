using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CaseStudy001.Models;
using System.Collections;

namespace CaseStudy001.Controllers
{
    public class OrderMonitoringController : Controller
    {
        private DepotManagementEntities db = new DepotManagementEntities();

        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Product);
            return View(orders.ToList());
        }

    }
}
