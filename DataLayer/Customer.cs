using System.Collections.Generic;

public class Customer
{
	public Customer()
	{
	}

	public int CustomerID { get; set; }
	public string Name { get; set; }
	public string Address { get; set; }

	[IgnoreDefault]
	public static List<Customer> CustomerList{
		get {
			return DBFactory.GetAll<Customer>(new Customer());
		}
	}

	}
