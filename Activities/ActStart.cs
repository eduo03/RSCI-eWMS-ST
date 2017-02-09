
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Debenhams.Activities
{
	[Activity (Label = "RSCI(Store)", Theme = "@android:style/Theme.Light.NoTitleBar")]		
	//[Activity (Label = "RSCI(Store)",MainLauncher=true , Theme = "@android:style/Theme.Light.NoTitleBar")]			
	public class ActStart : Activity
	{
		System.Timers.Timer t;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LayStart);
			t = new System.Timers.Timer();
			t.Interval = 1500;
			t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
			t.Start();
		}

		protected void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			t.Stop();
			var intent = new Intent ();
			intent.SetClass (this, typeof(ActLogin));
			StartActivity (intent);
		}
	}
}

