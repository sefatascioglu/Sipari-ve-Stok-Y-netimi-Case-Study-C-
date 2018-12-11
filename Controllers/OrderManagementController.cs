using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CaseStudy001.Models;

namespace CaseStudy001.Controllers
{
    public class OrderManagementController : Controller
    {
        private DepotManagementEntities db = new DepotManagementEntities();

        private const string ERROR_REPEATINGSENDORDER = "Daha once aktarilmis siparisler tekrar aktarilamamaktadir.";

        // GET: OrderManagement
        public ActionResult Index()
        {
            var query = from ord in db.Orders where !ord.Status.Trim().Equals("O") && !ord.Status.Trim().Equals("T") select ord;
            List<Order> orderList = query.ToList();
            TempData["idList"] = orderList;
            TempData.Keep();
            return View(orderList);
        }

        // GET: OrderManagement/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        [HttpGet]
        public ActionResult Create(string ProductId)
        {
            // dropdown list dolduruluyor
            ViewBag.ProductId = new SelectList(db.Products, "Id", "ProductName");
            ViewBag.TradeMark = db.Products.First().TradeMark;
            ViewBag.Color = db.Products.First().Color;
            ViewBag.Size = db.Products.First().Size;
            return View();
        }



        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProductId,ProductNumber,Status")] Order order)
        {
            // dropdown list dolduruluyor
            ViewBag.ProductId = new SelectList(db.Products, "Id", "ProductName");
            if (order.ProductNumber.Equals(0))
            {
                Product product = db.Products.Find(order.ProductId);
                ViewBag.TradeMark = product.TradeMark;
                ViewBag.Color = product.Color;
                ViewBag.Size = product.Size;
                return View();
            }
            else if (ModelState.IsValid)
            {
                order.Status = "A";
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }


        public ActionResult sendOrderById(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); ;
            }
            Order findOrderById = db.Orders.Find(id);

            if (findOrderById == null)
            {
                return HttpNotFound();
            }

            if (findOrderById.Status.Trim().Equals("D"))
            {
                TempData["OrderManagementErrorSendOrder"] = ERROR_REPEATINGSENDORDER;
                return RedirectToAction("Index");
            }
            else
            {
                findOrderById.Status = "D";
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult cancelledOrderById(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); ;
            }
            Order findOrderById = db.Orders.Find(id);

            if (findOrderById == null)
            {
                return HttpNotFound();
            }
            if (findOrderById.Status.Trim().Equals("D") || findOrderById.Status.Trim().Equals("I"))
            {
                findOrderById.Status = "A";
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult sendOrdersWithBulk(List<int> listId)
        {
            List<Order> listOrder = (List<Order>)TempData["idList"];
            TempData.Keep();

            if (listOrder != null)
            {
                foreach (var i in listOrder)
                {
                    var findOrderById = db.Orders.Where(a => a.Id.Equals(i.Id)).FirstOrDefault();
                    if (!findOrderById.Status.Trim().Equals("D"))
                        findOrderById.Status = "D";
                }
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }




        // GET: OrderManagement/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "ProductName", order.ProductId);
            return View(order);
        }

        // POST: OrderManagement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProductId,ProductNumber,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "ProductName", order.ProductId);
            return View(order);
        }

        // GET: OrderManagement/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: OrderManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
