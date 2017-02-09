using SQLite;

namespace Debenhams.Models
{
	public class tblConnectionURL
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string url { get; set; }
	}

	public class tblUser
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string userid { get; set; }
		public string username { get; set; }
		public string password { get; set; }
		public string fname { get; set; }
		public string lname { get; set; }
		public string token { get; set; }
		public string status { get; set; }
	}

	public class tblTLList
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string load_code { get; set; }
		public string piler_id { get; set; }
		public string store_id { get; set; }
		public string status { get; set; }
	}

	public class tblTLListDetail
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string load_code { get; set; }
		public string box_code { get; set; }
		public string piler_id { get; set; }
		public string status { get; set; }
	}

	public class tblLoadUpc
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string move_doc { get; set; }
		public string box_code { get; set; }
		public string upc { get; set; }
		public string description { get; set; }
		public string oqty { get; set; }
		public string rqty { get; set; }
		public string status { get; set; }
		public string dates { get; set; }
		public string variance { get; set; }
	}
		
}

