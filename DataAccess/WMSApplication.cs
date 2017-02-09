using System;
using Android.App;
using Android.Runtime;
using Debenhams.DataAccess;

namespace Debenhams.DataAccess
{
	[Application]

	public class WMSApplication : Application
	{
		public IItemRepository ItemRepository { get; set; }

		public WMSApplication(IntPtr handle, JniHandleOwnership transfer)
			: base(handle, transfer)
		{
		}

		public override void OnCreate()
		{
			base.OnCreate();
			ItemRepository = new OrmItemRepository(this);
		}
	}

	public class GlobalVariables : Application 
	{
		public static string GlobalUrl = "";
		public static string GlobalUserid = "";
		public static string GlobalMessage = "RSCI(Store)";
		public static string Globallogin = "";


	}
}