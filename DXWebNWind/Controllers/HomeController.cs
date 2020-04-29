using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using DXWebNWind.Code.NWindEF;
using DXWebNWind.Models;
using DXWebNWind.Code;

namespace DXWebNWind.Controllers
{

	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public TResult DBExec<TResult>(Func<NWindDBContext, TResult> func)
		{
			using (NWindDBContext ctx = new NWindDBContext())
			{
				var result = func(ctx);
				return result;
			}
		}
		public void DBExec(Action<NWindDBContext> action)
		{
			using (NWindDBContext ctx = new NWindDBContext())
			{
				action(ctx);
			}
		}
		public IEnumerable<LookupItem<TID>> DBGetLookup<TID>(Func<NWindDBContext, IEnumerable<LookupItem<TID>>> func)
		{
			var result = DBExec(func);
			return result;

		}

		//NWindDBContext nwindCtx = new NWindDBContext();
		protected HomeViewModel GetModel()
		{
			var result = new HomeViewModel
			{
				Customers = DBGetLookup<string>((db) => (from c in db.Customers
																	select new LookupItem<string>
																	{
																		ID = c.CustomerID,
																		Text = c.CompanyName
																	}).ToList()),
				Employees = DBGetLookup<int>((db) => (from c in db.Employees
																 select new LookupItem<int>
																 {
																	 ID = c.EmployeeID,
																	 Text = c.LastName + ", " + c.FirstName
																 }).ToList()),
				Shippers = DBGetLookup<int>((db) => (from s in db.Shippers
																select new LookupItem<int>
																{
																	ID = s.ShipperID,
																	Text = s.CompanyName
																}).ToList()),
				Orders = DBExec<IEnumerable<Orders>>((db) => db.Orders.ToList())
			};

			return result;
		}

		[ValidateInput(false)]
		public ActionResult GridViewPartial()
		{
			var model = GetModel();
			return PartialView("_GridViewPartial", model);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult GridViewPartialAddNew(DXWebNWind.Code.NWindEF.Orders item)
		{
			if (ModelState.IsValid)
			{
				try
				{
					DBExec((db) =>
					{
						db.Orders.AddRange(new Orders[] { item });
						db.SaveChanges();						
					});
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
				ViewData["EditError"] = "Please, correct all errors.";

			return PartialView("_GridViewPartial", GetModel());
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult GridViewPartialUpdate(DXWebNWind.Code.NWindEF.Orders item)
		{
			if (ModelState.IsValid)
			{
				try
				{
					DBExec((db) =>
					{
						var modelItem = db.Orders.FirstOrDefault(it => it.OrderID == item.OrderID);
						if (modelItem != null)
						{
							this.UpdateModel(modelItem);
							db.SaveChanges();
						}
					});
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
				ViewData["EditError"] = "Please, correct all errors.";

			return PartialView("_GridViewPartial", GetModel());
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult GridViewPartialDelete(System.Int32 OrderID)
		{
			if (OrderID >= 0)
			{
				try
				{
					DBExec((db) =>
					{
						var item = db.Orders.FirstOrDefault(it => it.OrderID == OrderID);
						if (item != null)
							db.Orders.Remove(item);
						db.SaveChanges();
					});
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}

			return PartialView("_GridViewPartial", GetModel());
		}

		// added
		protected HomeViewDetailModel GetDetailModel(int orderID)
		{
			var result = new HomeViewDetailModel
			{
				OrderID = orderID,
				Products = DBGetLookup<int>((db) => (from c in db.Products
														   select new LookupItem<int>
														   {
															   ID = c.ProductID,
															   Text = c.ProductName
														   }).ToList()),
				Details = DBExec<IEnumerable<Order_Details>>((db) => (from n in db.Order_Details
																	  where (n.OrderID == orderID)
																	  select n).ToList())
			};
			return result;
		}

		[ValidateInput(false)]
		public ActionResult GridViewDetailPartial(int orderID)
		{
			//changed			
			return PartialView("_GridViewDetailPartial", GetDetailModel(orderID));
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult GridViewDetailPartialAddNew(int orderID, Order_Details item)
		{
			//changed
			if (ModelState.IsValid)
			{
				try
				{
					DBExec((db) =>
					{
						db.Order_Details.AddRange(new Order_Details[] { item }); //changed
						db.SaveChanges();
					});
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
				ViewData["EditError"] = "Please, correct all errors.";

			return PartialView("_GridViewDetailPartial", GetDetailModel(orderID));
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult GridViewDetailPartialUpdate(int orderID, int newProductID, Order_Details item)
		{
			//changed
			if (ModelState.IsValid)
			{
				try
				{
					//changed
					DBExec((db) =>
					{
						var modelItem = db.Order_Details.FirstOrDefault(it => it.OrderID == orderID && it.ProductID == item.ProductID);
						if (modelItem != null)
						{
							this.UpdateModel(modelItem);
							db.SaveChanges();

							if (item.ProductID != newProductID)
							{
								if (db.Order_Details.Where(d => d.OrderID == orderID && d.ProductID == newProductID).Count() > 0)
									throw new Exception($"Product with ProductID = {newProductID} already exists in this order");

								db.Database.ExecuteSqlCommand("UPDATE [Order Details] SET ProductID = @p0 WHERE OrderID = @p1 AND ProductID = @p2;",
									newProductID, orderID, item.ProductID);
								db.SaveChanges();
							}
						}
					});
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
				ViewData["EditError"] = "Please, correct all errors.";

			return PartialView("_GridViewDetailPartial", GetDetailModel(orderID));
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult GridViewDetailPartialDelete(int orderID, int productID)
		{
			//changed
			if (orderID >= 0 && productID > 0)
			{
				try
				{
					//changed
					DBExec((db) =>
					{
						var item = db.Order_Details.FirstOrDefault(it => it.OrderID == orderID && it.ProductID == productID);
						if (item != null)
							db.Order_Details.Remove(item);
						db.SaveChanges();
					});
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			return PartialView("_GridViewDetailPartial", GetDetailModel(orderID));
		}
	}
}