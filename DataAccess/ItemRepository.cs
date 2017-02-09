
using System;
using Debenhams.Models;
using SQLite;
using System.Collections.Generic;
using Android.Graphics;
using System.Linq;
using Debenhams.ApiConnection;
using Debenhams.DataAccess;
namespace Debenhams.DataAccess
{
	public class ItemRepository
	{

		public ItemRepository ()
		{
		}

		#region >>>>>>>>>>POLIST<<<<<<<<<<

		public static List<tblTLList> GetLoadList(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblTLList>().ToList()
					.Where (t => t.load_code.Contains (move_doc) && t.piler_id.Equals(GlobalVariables.GlobalUserid))
					.ToList ();
			}
		}

		public static long UpdateTLList(tblTLList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}
		/*
		public static List<tblTLListDetail> GetLoadListDetail(string move_doc,string box_code)
		{
			tblTLListDetail itemlist = new tblTLListDetail ();
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblTLListDetail>().ToList()
					.Where (t=> t.move_doc.Equals(move_doc) && t.box_code.Contains(box_code)).ToList();
			}
		}
		*/

		public static List<tblLoadUpc> GetLoadUpc(string box_code)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadUpc>().ToList()
					.Where (t=> t.box_code.Equals(box_code))
					.OrderByDescending(t=> t.dates)
					.OrderBy(t=> t.status).ToList();
			}
		}
			
		public static tblLoadUpc GetPrimarymts(string box_code)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadUpc> ().ToList()
					.Where (t=> t.box_code==box_code && t.variance=="1")
					.FirstOrDefault ();
			}
		}
			
		public static tblTLListDetail GetBoxCode(string move_doc,string box_code)
		{
			box_code = box_code.Substring (0, Convert.ToInt32 (box_code.Length-1));
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblTLListDetail> ()
					.Where (t => t.load_code==move_doc && t.box_code==box_code)
					.FirstOrDefault ();
			}
		}


		public static tblTLListDetail GetBoxCode1(string box_code)
		{
			box_code = box_code.Substring (0, Convert.ToInt32 (box_code.Length-1));
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblTLListDetail> ()
					.Where (t => t.box_code==box_code)
					.FirstOrDefault ();
			}
		}


		public static List<tblTLListDetail> GetLoadListDetails(string load_code)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblTLListDetail>().ToList()
					.Where (t => t.load_code==load_code && t.piler_id==GlobalVariables.GlobalUserid)
					.ToList ();
			}
		}


		public static tblTLListDetail GetLoadBoxCode(string move_doc,string box_code)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblTLListDetail> ()
					.Where (t => t.load_code==move_doc && t.box_code==box_code)
					.FirstOrDefault ();
			}
		}

		/*
		public static tblTLList GetLoadBoxCode1(string box_code)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblTLList> ()
					.Where (t => t.move_doc==box_code)
					.FirstOrDefault ();
			}
		}
		*/

		public static long UpdateLoadListDetail(tblTLListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static long UpdateLoadListDetail1(tblTLListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}


		public static tblLoadUpc GetLoadUPC(string box_code,string upc)
		{
			upc = upc.Substring (0, Convert.ToInt32 (upc.Length-1));
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadUpc> ()
					.Where (t => t.box_code==box_code && t.upc==upc).SingleOrDefault ();
			}
		}


		public static long UpdateLoadUPC(tblLoadUpc item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				item.dates = DateTime.Now.ToString ();
				database.Update(item);
				return item.id;
			}
		}

		public static tblLoadUpc[] ChkBoxVariance (string box_code)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadUpc> ()
					.Where(t => t.box_code==box_code && t.oqty != t.rqty).ToArray ();
			}
		}


		public static long AddRPoListUpc(string move_doc, string box_code,string upc,string description,string oqty,string rqty,string status)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblLoadUpc
						{
							move_doc = move_doc,
							box_code = box_code,
							upc=upc,
							description = description,
							oqty = oqty,
							rqty = rqty,
							status=status,
							dates=DateTime.Now.ToString(),
							variance="1"
						}
					);
			}
		}

		public static long UserLogin(tblUser item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		#endregion

		public static tblLoadUpc GetRRLtUPC1(string box_code,string upc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblLoadUpc> ()
					.Where (t => t.box_code==box_code && t.upc==upc).SingleOrDefault ();
			}
		}

		public static long AddRRLListDetail(string move_doc,string box_code,string upc,string description,string oqty,string rqty,string status)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblLoadUpc
						{
							move_doc=move_doc,
							box_code= box_code,
							upc=upc,
							description = description,
							oqty = oqty,
							rqty = rqty,
							status=status,
							dates=DateTime.Now.ToString(),
							variance="1"
						}
					);
			}
		}


		public static tblTLList ChkRTLList(string load_code)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblTLList> ().ToList()
					.Where(t => t.load_code.Equals(load_code) && t.piler_id.Equals(GlobalVariables.GlobalUserid))
					.FirstOrDefault ();
			}
		}

		public static tblTLListDetail ChkRTLListDetailOrig(string load_code,string box_code)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblTLListDetail> ().ToList()
					.Where(t=>t.load_code==load_code && t.box_code==box_code && t.piler_id== GlobalVariables.GlobalUserid)
					.FirstOrDefault ();
			}
		}

		public static long AddRTLList(string load_code,string store_id)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblTLList
						{
							load_code = load_code,
							piler_id = GlobalVariables.GlobalUserid,
							store_id = store_id,
							status = "Open"
						}
					);
			}
		}

		public static long AddRTLListDetailOrig(string load_code,string box_code)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblTLListDetail
						{
							load_code = load_code,
							box_code = box_code,
							piler_id = GlobalVariables.GlobalUserid,
							status = "Open"
						}
					);
			}
		}



		public static long AddRTLListDetailUpc(string move_doc, string box_code,string upc,string description,string oqty,string mts)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblLoadUpc
						{
							move_doc = move_doc,
							box_code = box_code,
							upc = upc,
							description = description,
							oqty = oqty,
							rqty = "0",
							status = "Open",
							dates=DateTime.Now.ToString(),
							variance=mts
						}
					);
			}
		}

		public static tblTLList ChkPTLListExist(string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblTLList> ()
					.Where(t=>t.load_code==move_doc)
					.SingleOrDefault ();
			}
		}

		public static long DeleteRTLList(tblTLList item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		public static tblTLListDetail[] ChkRTLListDetail ( string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblTLListDetail> ()
					.Where(t => t.load_code==move_doc).ToArray ();
			}
		}

		public static tblLoadUpc[] ChkRTLListDetailUPC ( string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblLoadUpc> ()
					.Where(t => t.move_doc==move_doc).ToArray ();
			}
		}

		public static long DeleteRTLListDetail(tblTLListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}

		public static long DeleteRTLListDetails(tblLoadUpc item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Delete(item);
				return item.id;
			}
		}


		public static tblTLListDetail[] ChkRBoxListVariance ( string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblTLListDetail> ()
					.Where(t => t.load_code==move_doc && t.status != "Finish").ToArray ();
			}
		}

		public static tblLoadUpc[] ApiPTLListDetailUpc ( string box)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Query<tblLoadUpc>("select * from tblLoadUpc where box_code='"+ box +"'").ToArray ();
			}
		}


		public static tblLoadUpc[] ApiPTLListDetailBox (string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Query<tblLoadUpc>("select move_doc,box_code,upc,rqty from tblLoadUpc where move_doc='"+ move_doc +"'").ToArray ();
			}
		}

		public static void DeleteRTLList1(string load_code,string move_doc)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Query<tblTLList>("delete from tblTLList where load_code='"+ load_code +"'move_doc='"+ move_doc +"'");
			}
		}

		public static void DeleteRTLListDetail1(string box_code)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Query<tblTLListDetail>("delete from tblTLListDetail where box_code='"+ box_code +"'");
			}
		}

		public static void DeleteRTLListDetailUpc1(string box_code)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Query<tblLoadUpc>("delete from tblLoadUpc where box_code='"+ box_code +"'");
			}
		}


		public static void DeleteDetailUpc(string box_code)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Query<tblLoadUpc>("delete from tblLoadUpc where id='"+ box_code +"'");
			}
		}

		public static void DeleteLoadnotinBox()
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Query<tblTLList>("delete from tblTLList where load_code  not in (select load_code from tblTLListDetail)");
			}
		}

		public static void DeleteHdrError(string load_code)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Query<tblTLList>("delete from tblTLList where load_code='"+ load_code +"'");
			}
		}

		public static void DeleteDtlError(string load_code)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Query<tblTLListDetail>("delete from tblTLListDetail where load_code='"+ load_code +"'");
			}
		}

		public static void DeleteUpcError(string box_code)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Query<tblLoadUpc>("delete from tblLoadUpc where box_code='"+ box_code +"'");
			}
		}

		public static tblTLListDetail[] GetBoxError ( string load_code)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblTLListDetail> ()
					.Where(t => t.load_code==load_code).ToArray ();
			}
		}

	}
}

