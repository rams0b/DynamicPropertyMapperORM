using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Index()
        {
			List<CustomerOrder> orders = DBFactory.GetAll<CustomerOrder>(new CustomerOrder{ }, "usp_GetOrders");
            return View(orders);
        }

        // GET: Order/Details/5
		[HttpPost]
        public ActionResult Filter(FormCollection form)
        {
			DateTime startDate;
			DateTime endDate;
			if(!DateTime.TryParse(form["startDate"].ToString(),out startDate))
			{ 
				startDate = new DateTime(1800,01,01);
			}

			if (!DateTime.TryParse(form["endDate"].ToString(), out endDate))
			{
				endDate = new DateTime(3000, 01, 01);
			}

			int CustomerID = int.Parse(form["CustomerID"].ToString());
			ViewBag.CustomerID = CustomerID;
			List<CustomerOrder> orders = DBFactory.GetAll<CustomerOrder>(new CustomerOrder {
				CustomerID = CustomerID,
				StartDate = startDate,
				EndDate = endDate
			}, "usp_GetOrders");
			return View("Index",orders);
	    }

        // GET: Order/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Order/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Order/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Order/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Order/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Order/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
