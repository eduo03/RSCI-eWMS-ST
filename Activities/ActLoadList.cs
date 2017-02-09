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
	public class ActLoadList : Activity
	{
		//private static String ACTION_SOFTSCANTRIGGER = "com.motorolasolutions.emdk.datawedge.api.ACTION_SOFTSCANTRIGGER";
		//private static String EXTRA_PARAM = "com.motorolasolutions.emdk.datawedge.api.EXTRA_PARAMETER";
		//private static String DWAPI_TOGGLE_SCANNING = "TOGGLE_SCANNING";

		private EditText txtsearch;
		private ListView lvpo;
		private Button btnsearch;
		tblUser tbluser = new tblUser();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LayLoadList);


			var menuButton = FindViewById (Resource.Id.menuHdrButton);
			menuButton.Click += (sender, e) => {
				leftdrawer();
			};

			var menuPicking = FindViewById (Resource.Id.MenuReceiving);
			menuPicking.Click += (sender, e) => {
				leftdrawer();
			};


			var MenuLogout = FindViewById (Resource.Id.MenuLogout);
			MenuLogout.Click += (sender, e) => 
			{
				GlobalVariables.GlobalUserid = "";
				tbluser.id = 1;
				tbluser.userid = "";
				tbluser.password= "";
				tbluser.userid = "";
				tbluser.fname = "";
				tbluser.lname = "";
				tbluser.token = "";
				tbluser.status = "0";
				ItemRepository.UserLogin (tbluser);
				var intent = new Intent ();
				intent.SetClass (this, typeof(ActLogin));
				StartActivity (intent);
			};

			lvpo = FindViewById<ListView>(Resource.Id.lvpo);
			lvpo.ItemClick += new System.EventHandler<AdapterView.ItemClickEventArgs>(lvpo_ItemClicked);

			txtsearch = FindViewById<EditText>(Resource.Id.txtsearch);
			txtsearch.AfterTextChanged += delegate 
			{
				refreshItems();
			};

			btnsearch = FindViewById<Button> (Resource.Id.btnsearch);
			btnsearch.Click += delegate {
				ApiPTLlist();
			};

			/*
			btnScanBox= FindViewById<Button>(Resource.Id.btnScanBOX);
			btnScanBox.Click += new EventHandler (btnScanUpc_Clicked1);

			txtScanBox = FindViewById<EditText>(Resource.Id.txtScanBox);
			txtScanBox.AfterTextChanged += delegate {ScanUPC();};
			*/

		}

		private void leftdrawer()
		{
			var menu = FindViewById<ActLeftDrawer> (Resource.Id.ActLeftDrawer);
			menu.AnimatedOpened = !menu.AnimatedOpened;
		}

		protected override void OnResume()
		{
			base.OnResume();
			refreshItems();
		}

		private void refreshItems()
		{
			var items = ItemRepository.GetLoadList(txtsearch.Text);
			lvpo.Adapter = new AdpLoadList(this, items);
		}

		public override void OnBackPressed()
		{
			leftdrawer ();
		}

		private async void ApiPTLlist()
		{
			var progressDialog = ProgressDialog.Show(this, "Please wait...", "Downloading Data From Server...", true);
			try
			{
				await ApiConnection1.ApiPStoreLList(GlobalVariables.GlobalUrl +"/PStoreList/"+GlobalVariables.GlobalUserid);
				ItemRepository.DeleteLoadnotinBox();
				Toast.MakeText (this,"Downloading Successfully", ToastLength.Long).Show ();
				refreshItems ();
			}
			catch(Exception ex) 
			{
				Toast.MakeText (this,"Unable To Download Data.\n" + ex.Message, ToastLength.Long).Show ();
			}
			progressDialog.Cancel();
		}

		/*
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
							if (scanItem.move_doc == "Open") 
							{
								tblTLList loaddetail = new tblTLList();
								var loadboxcode =ItemRepository.GetLoadBoxCode1 (scanItem.move_doc);
								loaddetail.id = scanItem.id;
								loaddetail.move_doc = scanItem.load_code;
								loaddetail.move_doc = scanItem.move_doc;
								loaddetail.move_doc = scanItem.piler_id;
								loaddetail.move_doc = scanItem.store_id;
								loaddetail.status = "In Process";
								ItemRepository.UpdateLoadListDetail1 (loaddetail);
							}
							var intent = new Intent();
							intent.SetClass(this, typeof(ActLoadScanUpc));
							intent.PutExtra("id", Convert.ToString(scanItem.id));
							intent.PutExtra("load_code",scanItem.load_code);
							intent.PutExtra("box_code", scanItem.move_doc);
							intent.PutExtra("store_id", scanItem.store_id);
							intent.PutExtra("status", scanItem.status);
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
		*/

		private void lvpo_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			tblTLList tllist = new tblTLList ();
			var item = ((AdpLoadList)lvpo.Adapter).GetItemDetail(e.Position);
			var intent = new Intent();
			if (item.status == "Open")
			{
				tllist.id = item.id;
				tllist.load_code = item.load_code;
				tllist.piler_id = item.piler_id;
				tllist.store_id = item.store_id;
				tllist.status = "In Process";
				ItemRepository.UpdateTLList (tllist);
			}

			intent.SetClass(this, typeof(ActLoadListBox));
			intent.PutExtra("id",item.id.ToString());
			intent.PutExtra("load_code", item.load_code);
			intent.PutExtra("store_id", item.store_id);
			intent.PutExtra("status", item.status);
			StartActivity(intent);
		}

	}
}


