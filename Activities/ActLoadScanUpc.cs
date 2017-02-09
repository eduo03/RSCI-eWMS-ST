using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Debenhams.Models;
using Debenhams.DataAccess;
using Debenhams.Adapter;
using Debenhams.ApiConnection;

namespace Debenhams.Activities
{
	[Activity (Label = "ActLoadScanUpc",Theme = "@android:style/Theme.Holo.Light.NoActionBar")]			
	public class ActLoadScanUpc : Activity
	{
		private ListView lvUpc;
		private EditText txtScanUpc,txtload_code,txtbox_code;
		private Button btnScanUpc,btnDone,btnEncodeUpc;

		tblLoadUpc tblloadupc = new tblLoadUpc();

		private static String ACTION_SOFTSCANTRIGGER = "com.motorolasolutions.emdk.datawedge.api.ACTION_SOFTSCANTRIGGER";
		private static String EXTRA_PARAM = "com.motorolasolutions.emdk.datawedge.api.EXTRA_PARAMETER";
		private static String DWAPI_TOGGLE_SCANNING = "TOGGLE_SCANNING";
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.LayLoadScanUpc);

			lvUpc = FindViewById<ListView> (Resource.Id.lvUpc);
			txtScanUpc = FindViewById < EditText> (Resource.Id.txtScanUpc);
			txtload_code = FindViewById < EditText> (Resource.Id.txtmove_doc);
			txtbox_code = FindViewById < EditText> (Resource.Id.txtbox_code);
			btnScanUpc = FindViewById<Button> (Resource.Id.btnScanUpc);
			btnDone = FindViewById<Button> (Resource.Id.btnDone);
			btnEncodeUpc = FindViewById<Button> (Resource.Id.btnEncodeUPC);

			btnEncodeUpc.Click += new EventHandler (btnUpdateqty_Clicked);

			btnScanUpc.Click += new EventHandler (btnScanUPC_Clicked);
			btnDone.Click += delegate {
				btnDone_Clicked();
			};
			lvUpc.ItemClick += new System.EventHandler<AdapterView.ItemClickEventArgs> (lvUpc_ItemClicked);

			txtScanUpc.AfterTextChanged += delegate {ScanUPC();};

			txtload_code.Text =  Intent.GetStringExtra("load_code");
			txtbox_code.Text = Intent.GetStringExtra("box_code"); 
			refreshItems ();
		}

		public void refreshItems()
		{
			var items = ItemRepository.GetLoadUpc (txtbox_code.Text);
			lvUpc.Adapter = new AdpLoadUpc (this, items);
		}
		public override void OnBackPressed()
		{
			Finish ();
		}

		private void btnUpdateqty_Clicked(object sender, EventArgs e)
		{ 
			var inputView = LayoutInflater.Inflate(Resource.Layout.LayInputCredential, null);
			var builder = new AlertDialog.Builder(this);
			builder.SetTitle("Only authorized employee is allowed to update quantity.");
			builder.SetMessage ("Please enter your credential.");
			builder.SetView(inputView);
			builder.SetPositiveButton("Login", Login_Clicked);
			builder.SetNegativeButton("Cancel", delegate { builder.Dispose(); });
			builder.Show();
		}

		private async void Login_Clicked(object sender, DialogClickEventArgs args)
		{
			var dialog = (AlertDialog)sender;
			var txtuser = (EditText)dialog.FindViewById (Resource.Id.txtusername);
			var txtpass = (EditText)dialog.FindViewById (Resource.Id.txtpassword);
			var progressDialog = ProgressDialog.Show(this, "Please wait...", "Verifying Credentials...", true);
			try
			{
				if(txtuser.Text != "" && txtpass.Text != "")
				{
					await ApiConnection1.VerifyUser(GlobalVariables.GlobalUrl +"/VerifyStoreUser/"+ txtuser.Text + "/" + txtpass.Text );
					if(GlobalVariables.Globallogin != "")
					{
						var inputView = LayoutInflater.Inflate(Resource.Layout.LayInputUpc, null);
						var builder = new AlertDialog.Builder(this);
						builder.SetTitle("ENCODE UPC");
						builder.SetView(inputView);
						builder.SetPositiveButton("Add/Update",  Add_Clicked);
						builder.SetNegativeButton("Cancel", delegate { builder.Dispose(); });
						builder.Show();
					}
					else
					{
						Toast.MakeText (this, "Login Failed\nIncorect UserName Or Password..", ToastLength.Long).Show ();
					}
				}
				else
				{
					Toast.MakeText (this, "Login Failed\nIncorect UserName Or Password..", ToastLength.Long).Show ();
				}
			}
			catch(Exception ex) 
			{
				Toast.MakeText (this,"Unable To Login.\n" + ex.Message, ToastLength.Long).Show ();
			}
			progressDialog.Cancel();
		}

		private void Add_Clicked(object sender, DialogClickEventArgs args)
		{
			tblLoadUpc RLDetails = new tblLoadUpc();
			var dialog = (AlertDialog)sender;
			var txtupc = (EditText)dialog.FindViewById (Resource.Id.txtupc);
			var txtqty = (EditText)dialog.FindViewById (Resource.Id.txtqty);

			var items = ItemRepository.GetPrimarymts (txtbox_code.Text);
			var scanItem = ItemRepository.GetRRLtUPC1 (txtbox_code.Text,txtupc.Text);
			if (scanItem == null) {
				ItemRepository.AddRRLListDetail (
					items.move_doc,
					txtbox_code.Text,
					txtupc.Text,
					"Not in MTS",
					"0",
					txtqty.Text,
					"0"
				);
				refreshItems ();
			}
			else 
			{
				var builder = new AlertDialog.Builder(this);
				builder.SetTitle (GlobalVariables.GlobalMessage);
				builder.SetMessage ("The UPC ("+ txtupc.Text +") is already exist in MTS.\nUpdate Quantity?");
				builder.SetPositiveButton("Yes", delegate 
					{
						string stat = "0";
						if (Convert.ToInt32 (txtqty.Text)+1 == Convert.ToInt32 (scanItem.rqty)) {
							stat = "1";
						}
						RLDetails.id = scanItem.id;
						RLDetails.move_doc = scanItem.move_doc;
						RLDetails.box_code = scanItem.box_code;
						RLDetails.upc = scanItem.upc;
						RLDetails.description = scanItem.description;
						RLDetails.oqty = scanItem.oqty;
						RLDetails.rqty = txtqty.Text;
						RLDetails.status = stat;
						RLDetails.variance = scanItem.variance;
						ItemRepository.UpdateLoadUPC (RLDetails);
						refreshItems ();
					});
				builder.SetNegativeButton("No",delegate { builder.Dispose(); });
				builder.Show ();
			}
		}


		private void btnScanUPC_Clicked(object sender, EventArgs e)
		{ 
			txtScanUpc.RequestFocus();
			txtScanUpc.Text = "";
			//txtScanUpc.Text = "011\n";
			var intent = new Intent();    
			intent.SetAction(ACTION_SOFTSCANTRIGGER);    
			intent.PutExtra(EXTRA_PARAM, DWAPI_TOGGLE_SCANNING);
			SendBroadcast(intent); 
		}

		private void ScanUPC()
		{
			string oqty,rqty;

			if (Convert.ToInt32 (txtScanUpc.Text.Length) > 1) 
			{
				if (txtScanUpc.Text.Substring (Convert.ToInt32 (txtScanUpc.Text.Length) - 1, 1)=="\n") 
				{
					if (txtScanUpc.Text != ("")) 
					{
						var scanItem = ItemRepository.GetLoadUPC (txtbox_code.Text,txtScanUpc.Text);
						if (scanItem != null) 
						{
							oqty = scanItem.oqty;
							rqty = scanItem.rqty;

							string stat = "0";
							if (Convert.ToInt32(rqty)+1 == Convert.ToInt32(oqty)) 
							{
								stat = "1";
							}
							tblloadupc.id = scanItem.id;
							tblloadupc.move_doc = scanItem.move_doc;      
							tblloadupc.box_code = scanItem.box_code;
							tblloadupc.upc = scanItem.upc;
							tblloadupc.description = scanItem.description;
							tblloadupc.oqty = oqty;
							tblloadupc.rqty = Convert.ToString (Convert.ToInt32 (rqty) + 1);
							tblloadupc.dates = DateTime.Now.ToString ();
							tblloadupc.status = stat;
							tblloadupc.variance = scanItem.variance;

							ItemRepository.UpdateLoadUPC (tblloadupc);

							refreshItems ();
						} 
						else 
						{
							var builder = new AlertDialog.Builder(this);
							builder.SetTitle (GlobalVariables.GlobalMessage);
							builder.SetMessage ("You scanned a UPC not in the MTS.\n\nUPC: "+txtScanUpc.Text+"\nDo you want to add UPC in MTS?");
							builder.SetPositiveButton("Yes", AddInvalidUPC_Clicked);
							builder.SetNegativeButton("No",delegate { builder.Dispose(); });
							builder.Show ();
						}
					}
				}
			}
		}

		private void lvUpc_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			tblTLList tllist = new tblTLList ();
			var item = ((AdpLoadUpc)lvUpc.Adapter).GetItemDetail(e.Position);
			var intent = new Intent();
			if (item.oqty == "0")
			{
				var builder = new AlertDialog.Builder(this);
				builder.SetTitle (GlobalVariables.GlobalMessage);
				builder.SetMessage ("Unload UPC in MTS");
				builder.SetPositiveButton("Yes", delegate {
					ItemRepository.DeleteDetailUpc(item.id.ToString());
					refreshItems ();
				});
				builder.SetNegativeButton("No",delegate { builder.Dispose(); });
				builder.Show ();
			}
		}

		private void AddInvalidUPC_Clicked(object sender, DialogClickEventArgs args)
		{
			var items = ItemRepository.GetPrimarymts (txtbox_code.Text);
			ItemRepository.AddRPoListUpc (
				items.move_doc,
				txtbox_code.Text,
				txtScanUpc.Text.Substring (0, Convert.ToInt32 (txtScanUpc.Text.Length - 1)),
				"Not in MTS",
				"0",
				"1",
				"0"
			);
			refreshItems ();
		}

		private void btnDone_Clicked()
		{
			var builder = new AlertDialog.Builder(this);
			var chkitem =ItemRepository.ChkBoxVariance (txtbox_code.Text);
			if (chkitem.Count()==0) 
			{
				builder.SetTitle(GlobalVariables.GlobalMessage);
				builder.SetMessage("Done Receiving?");
				builder.SetTitle(GlobalVariables.GlobalMessage);
				builder.SetPositiveButton("Yes", YesDialog_Clicked);
				builder.SetNegativeButton("No", delegate { builder.Dispose(); });
				builder.Show ();
			} 
			else
			{
				builder.SetTitle(GlobalVariables.GlobalMessage);
				builder.SetMessage("Done Receiving?");
				string upc = "";
				foreach (var i in chkitem) 
				{
					upc=upc+i.upc+"\n";
				}
				builder.SetTitle(GlobalVariables.GlobalMessage);
				builder.SetMessage ("Done Receiving?\nThere are still variance with the other following UPC/s\n"+ upc);
				builder.SetPositiveButton("Yes", YesDialog_Clicked);
				builder.SetNegativeButton("No", delegate { builder.Dispose(); });
				builder.Show ();
			}
		}

		private async void YesDialog_Clicked(object sender, DialogClickEventArgs args)
		{
			/*
			tblTLListDetail loaddetail = new tblTLListDetail();
			var loadboxcode =ItemRepository.GetLoadBoxCode (txtmove_doc.Text,txtbox_code.Text);
			loaddetail.id = loadboxcode.id;
			loaddetail.move_doc = loadboxcode.move_doc;
			loaddetail.box_code = loadboxcode.box_code;
			loaddetail.status = "Inprocess";
			ItemRepository.UpdateLoadListDetail (loaddetail);
			Finish ();
			*/
			tblLoadUpc RLDetails = new tblLoadUpc();
			var progressDialog = ProgressDialog.Show (this, "Please wait... ", "Updating Box...", true);
			try
			{

				var updateupc=ItemRepository.ApiPTLListDetailUpc (txtbox_code.Text);
				foreach (var i in updateupc) 
				{
					if (i.oqty!="0")
					{
						await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/UpdateStoreOrderUpc/" + i.box_code + "/" + i.move_doc + "/" + i.upc + "/" + i.rqty ));
					}
					else
					{
						await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/AddStoreOrderUpc/" + i.box_code + "/" + txtload_code.Text + "/" + i.move_doc +"/"+ i.upc + "/" + i.rqty + "/" + GlobalVariables.GlobalUserid ));
						RLDetails.id = i.id;
						RLDetails.move_doc = i.move_doc;
						RLDetails.box_code = i.box_code;
						RLDetails.upc = i.upc;
						RLDetails.description = i.description;
						RLDetails.oqty = "1";
						RLDetails.rqty = i.rqty;
						RLDetails.status = i.status;
						RLDetails.status = i.variance;
						ItemRepository.UpdateLoadUPC (RLDetails);
					}
				}
				//var updateupcbox=ItemRepository.ApiPTLListDetailUpc (txtloadnumber.Text);
				//foreach (var i in updateupcbox) 
				//{
				await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/UpdateStoreOrderBox/" + txtbox_code.Text +"/"+ txtload_code.Text ));
				//}

				//await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/UpdateStoreOrderStatus/"+ txtloadnumber.Text));

				//ItemRepository.DeleteRTLList1 (txtmove_doc.Text,txtbox_code.Text);
				ItemRepository.DeleteRTLListDetail1 (txtbox_code.Text);
				ItemRepository.DeleteRTLListDetailUpc1 (txtbox_code.Text);
				ItemRepository.DeleteLoadnotinBox();


				progressDialog.Cancel ();

				var builder = new AlertDialog.Builder(this);
				builder.SetTitle(GlobalVariables.GlobalMessage);
				builder.SetMessage("Box Successfully Updated");
				builder.SetPositiveButton("OK",Closed_Clicked);
				builder.SetCancelable(false);
				builder.Show();
			} catch (Exception ex) {
				progressDialog.Cancel ();
				Toast.MakeText (this, "Unable To Update Box.\n" + ex.Message, ToastLength.Long).Show ();
			}
		}

		private void Closed_Clicked(object sender, DialogClickEventArgs args)
		{
			var intent = new Intent();
			intent.SetClass(this, typeof(ActLoadList));
			StartActivity(intent);
		}
	}
}

