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
	[Activity (Label = "ActLoadList",Theme = "@android:style/Theme.Holo.Light.NoActionBar")]			
	public class ActLoadListBox : Activity
	{
		private static String ACTION_SOFTSCANTRIGGER = "com.motorolasolutions.emdk.datawedge.api.ACTION_SOFTSCANTRIGGER";
		private static String EXTRA_PARAM = "com.motorolasolutions.emdk.datawedge.api.EXTRA_PARAMETER";
		private static String DWAPI_TOGGLE_SCANNING = "TOGGLE_SCANNING";

		private EditText txtsearch,txtScanBox,txtload_code;
		private ListView lvpo;
		private Button btnScanBox;
		tblUser tbluser = new tblUser();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LayLoadListBox);
		
			lvpo = FindViewById<ListView>(Resource.Id.lvpo);
			lvpo.ItemClick += new System.EventHandler<AdapterView.ItemClickEventArgs>(lvpo_ItemClicked);

			txtsearch = FindViewById<EditText>(Resource.Id.txtsearch);
			txtsearch.AfterTextChanged += delegate 
			{
				refreshItems();
			};
				


			btnScanBox= FindViewById<Button>(Resource.Id.btnScanBox);
			btnScanBox.Click += new EventHandler (btnScanUpc_Clicked1);

			txtload_code = FindViewById<EditText>(Resource.Id.txtload_code);
			txtload_code.Text = Intent.GetStringExtra ("load_code");

			txtScanBox = FindViewById<EditText>(Resource.Id.txtScanBox);
			txtScanBox.AfterTextChanged += delegate {ScanUPC();};

		}

		protected override void OnResume()
		{
			base.OnResume();
			refreshItems();
		}

		private void refreshItems()
		{
			var items = ItemRepository.GetLoadListDetails(Intent.GetStringExtra("load_code"));
			lvpo.Adapter = new AdpLoadListDetail(this, items);
		}



		private void btnScanUpc_Clicked1(object sender, EventArgs e)
		{ 
			txtScanBox.RequestFocus();
			txtScanBox.Text = "";
			//txtScanUpc.Text = "box123\n";
			var intent = new Intent();    
			intent.SetAction(ACTION_SOFTSCANTRIGGER);    
			intent.PutExtra(EXTRA_PARAM, DWAPI_TOGGLE_SCANNING);
			SendBroadcast(intent); 
		}

		private void ScanUPC()
		{
			if (Convert.ToInt32 (txtScanBox.Text.Length) > 1) 
			{
				if (txtScanBox.Text.Substring (Convert.ToInt32 (txtScanBox.Text.Length) - 1, 1)=="\n") 
				{
					if (txtScanBox.Text != ("")) 
					{
						var scanItem = ItemRepository.GetBoxCode1 (txtScanBox.Text);
						if (scanItem != null) 
						{
							if (scanItem.status == "Open") 
							{
								tblTLListDetail loaddetail = new tblTLListDetail();
								loaddetail.id = scanItem.id;
								loaddetail.load_code = scanItem.load_code;
								loaddetail.box_code = scanItem.box_code;
								loaddetail.piler_id = scanItem.piler_id;
								loaddetail.status = "In Process";
								ItemRepository.UpdateLoadListDetail1 (loaddetail);
							}
							var intent = new Intent();
							intent.SetClass(this, typeof(ActLoadScanUpc));
							intent.PutExtra("id", Convert.ToString(scanItem.id));
							intent.PutExtra("load_code",scanItem.load_code);
							intent.PutExtra("box_code", scanItem.box_code);
							StartActivity(intent);
						} 
						else 
						{
							var builder = new AlertDialog.Builder(this);
							builder.SetTitle (GlobalVariables.GlobalMessage);
							builder.SetMessage ("You scanned a Box not in List\nBox: "+ txtScanBox.Text);
							builder.SetPositiveButton("Ok",delegate { builder.Dispose(); });
							builder.Show ();
						}
					}
				}
			}
		}


		private void lvpo_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			tblTLListDetail loaddetail = new tblTLListDetail();
			var item = ((AdpLoadListDetail)lvpo.Adapter).GetItemDetail(e.Position);
			var intent = new Intent();
			if (item.status == "Open")
			{
				loaddetail.id = item.id;
				loaddetail.load_code = item.load_code;
				loaddetail.box_code = item.box_code;
				loaddetail.piler_id = item.piler_id;
				loaddetail.status = "In Process";
				ItemRepository.UpdateLoadListDetail1 (loaddetail);
			}

			intent.SetClass(this, typeof(ActLoadScanUpc));
			intent.PutExtra("id",item.id.ToString());
			intent.PutExtra("load_code", item.load_code);
			intent.PutExtra("box_code", item.box_code);
			intent.PutExtra("status", item.status);
			StartActivity(intent);
		}

	}
}


