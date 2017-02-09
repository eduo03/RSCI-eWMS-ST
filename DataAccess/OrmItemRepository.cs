using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Debenhams.Models;
using Debenhams.DataAccess;
using SQLite;
using Android.Graphics;
using System;

namespace Debenhams.DataAccess
{
	public class OrmItemRepository : IItemRepository
	{
		static object locker = new object ();

		public OrmItemRepository(Context context)
		{

		}

		#region > Receiving Header <

		public tblConnectionURL GetConnectionURL()
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblConnectionURL> ().SingleOrDefault ();
			}
		}

		public long UpdateConnectionURL(tblConnectionURL item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}
		#endregion
	}
}