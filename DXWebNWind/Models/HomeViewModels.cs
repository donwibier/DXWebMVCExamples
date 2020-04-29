using DXWebNWind.Code.NWindEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXWebNWind.Models
{
	public class LookupItem<TID>
	{
		public TID ID { get; set; }
		public string Text { get; set; }
	}

	public class HomeViewModel
	{
		public IEnumerable<Orders> Orders { get; set; }
		public IEnumerable<LookupItem<string>> Customers { get; set; }
		public IEnumerable<LookupItem<int>> Employees { get; set; }
		public IEnumerable<LookupItem<int>> Shippers { get; set; }
	}

	public class HomeViewDetailModel
	{
		public int OrderID { get; set; }
		public IEnumerable<Order_Details> Details { get; set; }
		public IEnumerable<LookupItem<int>> Products { get; set; }
	}
}