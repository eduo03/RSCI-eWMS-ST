/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
	[Activity (Label = "ActLoadListDetail",Theme = "@android:style/Theme.Holo.Light.NoActionBar")]			
	public class ActLoadListDetail : Activity
	{

		private Button btnScanBox, BtnDone;
		private EditText txtsearch,txtloadnumber,txtScanUpc;
		private ListView lvBox;

		private static String ACTION_SOFTSCANTRIGGER = "com.motorolasolutions.emdk.datawedge.api.ACTION_SOFTSCANTRIGGER";
		private static String EXTRA_PARAM = "com.motorolasolutions.emdk.datawedge.api.EXTRA_PARAMETER";
		private static String DWAPI_TOGGLE_SCANNING = "TOGGLE_SCANNING";


		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.LayLoadListsDetail);


			txtScanUpc = FindViewById<EditText>(Resource.Id.txtScanUpc);
			txtsearch = FindViewById<EditText>(Resource.Id.txtsearch);
			txtloadnumber = FindViewById<EditText>(Resource.Id.txtloadnumber);
			lvBox = FindViewById<ListView>(Resource.Id.lvBox);
			btnScanBox= FindViewById<Button>(Resource.Id.btnScanBox);
			BtnDone= FindViewById<Button>(Resource.Id.btnDone);

			txtsearch.AfterTextChanged += delegate 
			{
				refreshItems();
			};
			txtloadnumber.Text=Intent.GetStringExtra("move_doc");

			lvBox.ItemClick += new System.EventHandler<AdapterView.ItemClickEventArgs>(lvBox_ItemClicked);

			btnScanBox.Click += new EventHandler (btnScanUpc_Clicked);
		
			BtnDone.Click += new EventHandler (btnDone_Clicked);

			txtScanUpc.AfterTextChanged += delegate {ScanUPC();};


			refreshItems ();
		}

		protected override void OnResume()
		{
			base.OnResume();
			refreshItems();
		}

		private void refreshItems()
		{
			var items = ItemRepository.GetLoadListDetail(txtloadnumber.Text,txtsearch.Text);
			lvBox.Adapter = new AdpLoadListDetail(this, items);
		}

		private void btnScanUpc_Clicked(object sender, EventArgs e)
		{ 
			txtScanUpc.RequestFocus();
			txtScanUpc.Text = "";
			//txtScanUpc.Text = "box123\n";
			var intent = new Intent();    
			intent.SetAction(ACTION_SOFTSCANTRIGGER);    
			intent.PutExtra(EXTRA_PARAM, DWAPI_TOGGLE_SCANNING);
			SendBroadcast(intent); 
		}


		private void lvBox_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			var item = ((AdpLoadListDetail)lvBox.Adapter).GetItemDetail(e.Position);
			if (item.status == "Open") 
			{
				tblTLListDetail loaddetail = new tblTLListDetail();
				var loadboxcode =ItemRepository.GetLoadBoxCode (txtloadnumber.Text,item.box_code);
				loaddetail.id = item.id;
				loaddetail.move_doc = item.move_doc;
				loaddetail.box_code = item.box_code;
				loaddetail.status = "In Process";
				ItemRepository.UpdateLoadListDetail (loaddetail);
			}
			var intent = new Intent();
			intent.SetClass(this, typeof(ActLoadScanUpc));
			intent.PutExtra("id", Convert.ToString(item.id));
			intent.PutExtra("move_doc",item.move_doc);
			intent.PutExtra("box_code", item.box_code);
			intent.PutExtra("status", item.status);
			StartActivity(intent);
		}

		private void testingintent(Activity context,Activity context1)
		{


		}
	
		private void ScanUPC()
		{
			if (Convert.ToInt32 (txtScanUpc.Text.Length) > 1) 
			{
				if (txtScanUpc.Text.Substring (Convert.ToInt32 (txtScanUpc.Text.Length) - 1, 1)=="\n") 
				{
					if (txtScanUpc.Text != ("")) 
					{
						var scanItem = ItemRepository.GetBoxCode (txtloadnumber.Text,txtScanUpc.Text);
						if (scanItem != null) 
						{
							if (scanItem.box_code == "Open") 
							{
								tblTLListDetail loaddetail = new tblTLListDetail();
								var loadboxcode =ItemRepository.GetLoadBoxCode (txtloadnumber.Text,scanItem.box_code);
								loaddetail.id = scanItem.id;
								loaddetail.move_doc = scanItem.move_doc;
								loaddetail.box_code = scanItem.box_code;
								loaddetail.status = "In Process";
								ItemRepository.UpdateLoadListDetail (loaddetail);
							}
							var intent = new Intent();
							intent.SetClass(this, typeof(ActLoadScanUpc));
							intent.PutExtra("id", Convert.ToString(scanItem.id));
							intent.PutExtra("move_doc",scanItem.move_doc);
							intent.PutExtra("box_code", scanItem.box_code);
							intent.PutExtra("status", scanItem.status);
							StartActivity(intent);
						} 
						else 
						{
							var builder = new AlertDialog.Builder(this);
							builder.SetTitle (GlobalVariables.GlobalMessage);
							builder.SetMessage ("You scanned a Box not in Load\nBox: "+txtScanUpc.Text);
							builder.SetPositiveButton("Ok",delegate { builder.Dispose(); });
							builder.Show ();
						}
					}
				}
			}
		}

		private void btnDone_Clicked(object sender, EventArgs e)
		{
			var builder = new AlertDialog.Builder (this);
			var chkitem = ItemRepository.ChkRBoxListVariance (txtloadnumber.Text);
			if (chkitem.Count () == 0) {
				builder.SetTitle (GlobalVariables.GlobalMessage);
				builder.SetMessage ("Finish Receiving MTS?");
			} else {
				string box = "";
				foreach (var i in chkitem) {
					box = box + i.box_code + "\n";
				}
				builder.SetTitle (GlobalVariables.GlobalMessage);
				builder.SetMessage ("Finish Receiving MTS?\nThere are still variance with the other following Box/s\n" + box);
			}
			builder.SetPositiveButton ("Yes", YesDialog_Clicked);
			builder.SetNegativeButton ("No", delegate {builder.Dispose ();});
			builder.Show ();
		}



		private async void YesDialog_Clicked(object sender, DialogClickEventArgs args)
		{
			
			var progressDialog = ProgressDialog.Show (this, "Please wait... ", "Updating MTS...", true);
			try
			{

				var updateupc=ItemRepository.ApiPTLListDetailUpc (txtloadnumber.Text);
				foreach (var i in updateupc) 
				{
					if (i.oqty!="0")
					{
						await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/UpdateStoreOrderUpc/" + i.move_doc + "/" + i.upc + "/" + i.rqty ));
					}
					else
					{

					}
				}
				var updateupcbox=ItemRepository.ApiPTLListDetailUpc (txtloadnumber.Text);
				foreach (var i in updateupcbox) 
				{
					await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/UpdateStoreOrderBox/" + i.move_doc + "/" + i.box_code + "/" + i.upc + "/" + i.rqty ));
				}

				await (ApiConnection1.ApiDebsUpdateData (GlobalVariables.GlobalUrl + "/UpdateStoreOrderStatus/"+ txtloadnumber.Text));

				//ItemRepository.DeleteRTLList1 (txtloadnumber.Text);
				//ItemRepository.DeleteRTLListDetail1 (txtloadnumber.Text);
				//ItemRepository.DeleteRTLListDetailUpc1 (txtloadnumber.Text);

				progressDialog.Cancel ();

				var builder = new AlertDialog.Builder(this);
				builder.SetTitle(GlobalVariables.GlobalMessage);
				builder.SetMessage("MTS Successfully Updated");
				builder.SetPositiveButton("OK",Closed_Clicked);
				builder.Show();
			} catch (Exception ex) {
				progressDialog.Cancel ();
				Toast.MakeText (this, "Unable To Update MTS.\n" + ex.Message, ToastLength.Long).Show ();
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

*/