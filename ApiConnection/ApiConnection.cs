using System;
using System.Threading.Tasks;
using System.Json;
using System.Net;
using System.IO;
using System.Net.Http;
using Android.App;
using Debenhams.DataAccess;
using Debenhams.Models;
using SQLite;
using Android.Widget;
using System.Collections.Generic;
using Android.Graphics;
using System.Linq;
using Debenhams;
namespace Debenhams.ApiConnection
{
	public class ApiConnection1
	{
		tblUser user1 = new tblUser();

		public ApiConnection1 ()
		{
		}

		public static async Task<bool> ApiPStoreLList(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr)
						{
							string load_code = Convert.ToString ((string)item ["load_code"]);
							string store_id = Convert.ToString ((string)item ["store_code"]);
							var load = ItemRepository.ChkRTLList (load_code);
							if (load == null) 
							{
								ItemRepository.AddRTLList (load_code, store_id);
							}

							try 
							{
								await ApiPStoreLListDetail (GlobalVariables.GlobalUrl + "/PStoreListDetail/" + load_code +"/"+ GlobalVariables.GlobalUserid);
							}
							catch(Exception ex)
							{
								ItemRepository.DeleteHdrError(load_code);
								ItemRepository.DeleteDtlError(load_code);

								var chkbox=ItemRepository.GetBoxError (load_code);
								foreach (var i in chkbox) 
								{
									ItemRepository.DeleteUpcError (i.box_code);
								}
								Toast.MakeText(Android.App.Application.Context, "Unable To Download.\n"+ex.Message, ToastLength.Long).Show ();
							}
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> ApiPStoreLListDetail(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							string load_code=item ["load_code"];
							string box_code=item ["box_number"];

							try 
							{
								var po = ItemRepository.ChkRTLListDetailOrig (load_code,box_code);
								if (po == null) 
								{
									await ApiPStoreLListDetailUpc (GlobalVariables.GlobalUrl + "/PStoreListDetailUpc/" + box_code);
									ItemRepository.AddRTLListDetailOrig (load_code, box_code);
								}
							}
							catch(Exception ex)
							{
								ItemRepository.DeleteUpcError (box_code);
								Toast.MakeText(Android.App.Application.Context, "Unable To Download.\n Load :"+ "" + "\n"+ex.Message, ToastLength.Long).Show ();
							}

						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> ApiPStoreLListDetailUpc(string url)
		{
			bool result = false;
			string mts = "1";
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							string move_doc=Convert.ToString ((int)item ["so_no"]);
							string box_code=Convert.ToString ((string)item ["box_number"]);
							string upc=Convert.ToString ((string)item ["upc"]);
							string description=Convert.ToString ((string)item ["description"]);
							string oqty=Convert.ToString ((int)item ["quantity_packed"]);
							ItemRepository.AddRTLListDetailUpc (move_doc, box_code, upc, description, oqty,mts);
							mts="0";
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> ApiDebsUpdateData(string url)
		{
			bool result = false;

			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{

				}
			}
			return result;
		}


		public static async Task<bool> UserLogin(string url)
		{
			tblUser tbluser = new tblUser ();
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							tbluser.id= 1;
							tbluser.userid=Convert.ToString((int)item["id"]);
							tbluser.username=item ["username"];
							tbluser.password=item ["password"];
							tbluser.fname=item ["fname"];
							tbluser.lname=item ["lname"];
							tbluser.token="";
							tbluser.status="1";
							GlobalVariables.GlobalUserid=Convert.ToString((int)item["id"]);
							ItemRepository.UserLogin (tbluser);
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> VerifyUser(string url)
		{
			GlobalVariables.Globallogin="";
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							GlobalVariables.Globallogin=Convert.ToString((int)item["id"]);
						}
					}
				}
			}
			return result;
		}

		/*

		#region >>>>>>>>>>Picking<<<<<<<<<<

		public static async Task<bool> ApiPTLList(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							string movedoc = Convert.ToString ((int)item ["move_doc_number"]);
							var tl = ItemRepository.ChkPTLListExist (movedoc);
							if (tl == null) 
							{
								using (var database = WMSDatabase.NewConnection ()) {
									database.Insert
								(new  tblPickingList {
										move_doc = movedoc,
										store_id = item ["store_code"],
										store_name = item ["store_name"],
										status = "Open"
									}
									);
								}
								await ApiPTLListDetail (GlobalVariables.GlobalUrl + "/PTLListDetail/" + Convert.ToString ((int)item ["move_doc_number"]));
						
							}
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> ApiPTLListDetail(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr)
						{
							using (var database = WMSDatabase.NewConnection ())
							{
								database.Insert
								(new  tblPickingListDetail
									{
										move_doc = Convert.ToString((int)item["move_doc_number"]),
										picking_id= Convert.ToString((int)item["id"]),
										upc = item["upc"],
										sku = item["sku"],
										dept = item["dept"],
										style = item["short_description"],
										descr = item["short_description"],
										oqty = Convert.ToString((int)item["quantity_to_pick"]),
										rqty = Convert.ToString((int)item["moved_qty"]),
										status = "Open"
									}
								);
							}
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> GetPTLBoxCode(string url)
		{
			tblBox tblbox = new tblBox ();
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							var box_code = item ["box_code"];
							var store_id = item ["store_id"];
							var move_doc = item ["move_doc"];
							var number = item ["number"];
							var total = item ["total"];

							ItemRepository.AddPBox (box_code,store_id,move_doc,number,total);
						}
					}
				}
			}
			return result;
		}

		#endregion


		#region
		public static async Task<bool> UserLogin(string url)
		{
			tblUser tbluser = new tblUser ();
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							tbluser.id= 1;
							tbluser.userid=Convert.ToString((int)item["id"]);
							tbluser.username=item ["username"];
							tbluser.password=item ["password"];
							tbluser.fname=item ["fname"];
							tbluser.lname=item ["lname"];
							tbluser.token=item ["token"];
							tbluser.status="1";
							//GlobalVariables.GlobalUserid=Convert.ToString((int)item["id"]);
							ItemRepository.UserLogin (tbluser);
						}
					}
				}
			}
			return result;
		}
		#endregion
		*/
	}
}

