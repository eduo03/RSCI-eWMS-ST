using System.Collections.Generic;
using Debenhams.Models;
using Android.Graphics;
using System;

namespace Debenhams.DataAccess
{
	public interface IItemRepository
	{
		#region     >>>>>Receiving<<<<<
	
		tblConnectionURL GetConnectionURL();
		long UpdateConnectionURL(tblConnectionURL item);

		#endregion
	}	
}