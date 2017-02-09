using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Debenhams.Models;
using Debenhams.DataAccess;
using Debenhams;

namespace Debenhams.Adapter
{
	public class AdpLoadListDetail : BaseAdapter
	{
		private List<tblTLListDetail> _items;
		private Activity _context;

		public AdpLoadListDetail(Activity context, List<tblTLListDetail> items)
		{
			_context = context;
			_items = items;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			LinearLayout view;
			tblTLListDetail item;
			item = _items.ElementAt (position);

			view = (convertView
				?? _context.LayoutInflater.Inflate (Resource.Layout.LayLoadListBoxs, parent, false)
			) as LinearLayout;

			view.FindViewById<TextView>(Resource.Id.txtload ).Text = item.box_code;
			view.FindViewById<TextView>(Resource.Id.txtstatus ).Text = item.status;

			return view;
		}

		public override int Count
		{
			get 
			{ 
				return _items.Count();
			}
		}

		public tblTLListDetail GetItemDetail(int position)
		{
			return _items.ElementAt (position);
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return null;

		}

		public override long GetItemId(int position)
		{
			return position;
		}
	}
}