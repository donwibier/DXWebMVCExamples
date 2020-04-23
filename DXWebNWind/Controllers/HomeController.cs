using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DXWebNWind.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

		DXWebNWind.Code.NWindEF.NWindDBContext db = new DXWebNWind.Code.NWindEF.NWindDBContext();

		[ValidateInput(false)]
		public ActionResult GridViewPartial()
		{
			var model = db.Orders;
			PopulateViewBag();

			return PartialView("_GridViewPartial", model.ToList());
		}

		private void PopulateViewBag()
		{
			ViewBag.LookupCustomers = (from c in db.Customers
									   select new
									   {
										   CustomerID = c.CustomerID,
										   CompanyName = c.CompanyName
									   }).ToList();
			ViewBag.LookupEmployees = (from e in db.Employees
									   select new
									   {
										   EmployeeID = e.EmployeeID,
										   Name = e.LastName + ", " + e.FirstName
									   }).ToList();
			ViewBag.LookupShippers = (from s in db.Shippers
									  select new
									  {
										  ShipperID = s.ShipperID,
										  Name = s.CompanyName
									  }).ToList();

		}

		[HttpPost, ValidateInput(false)]
		public ActionResult GridViewPartialAddNew(DXWebNWind.Code.NWindEF.Orders item)
		{
			var model = db.Orders;
			if (ModelState.IsValid)
			{
				try
				{
					model.Add(item);
					db.SaveChanges();
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
				ViewData["EditError"] = "Please, correct all errors.";
			PopulateViewBag();
			return PartialView("_GridViewPartial", model.ToList());
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult GridViewPartialUpdate(DXWebNWind.Code.NWindEF.Orders item)
		{
			var model = db.Orders;
			if (ModelState.IsValid)
			{
				try
				{
					var modelItem = model.FirstOrDefault(it => it.OrderID == item.OrderID);
					if (modelItem != null)
					{
						this.UpdateModel(modelItem);
						db.SaveChanges();
					}
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
				ViewData["EditError"] = "Please, correct all errors.";
			PopulateViewBag();
			return PartialView("_GridViewPartial", model.ToList());
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult GridViewPartialDelete(System.Int32 OrderID)
		{
			var model = db.Orders;
			if (OrderID >= 0)
			{
				try
				{
					var item = model.FirstOrDefault(it => it.OrderID == OrderID);
					if (item != null)
						model.Remove(item);
					db.SaveChanges();
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			PopulateViewBag();
			return PartialView("_GridViewPartial", model.ToList());
		}
	}
}