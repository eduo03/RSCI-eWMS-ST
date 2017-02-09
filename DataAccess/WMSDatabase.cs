using System;
using System.IO;
using SQLite;

namespace Debenhams.DataAccess
{
	public class WMSDatabase: SQLiteConnection 
	{
		protected static string dbLocation;

		public WMSDatabase (string path): base (path)
		{
		}

		public static WMSDatabase NewConnection ()
		{
			return new WMSDatabase (DatabaseFilePath);
		}

		public static string DatabaseFilePath {
			get 
			{ 
				string result = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "DebenhamsDB");
				//string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
				//string libraryPath = Path.Combine (documentsPath, "../Library/");
				//var path = Path.Combine (libraryPath, "Buildaform.db3"); 

				return result;	
			}
		}
	}
}

