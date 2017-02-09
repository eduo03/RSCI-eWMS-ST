using System;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Debenhams.DataAccess
{
	public class SQLiteHelper
	{
		public static string GetDB()
		{
			string result = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "PaylessDB");
			return result;
		}

		public void ExecuteNonQuery(string cmd)
		{
			SqliteConnection con = new SqliteConnection ("Data Source=" + GetDB());
			SqliteCommand sqlcmd = new SqliteCommand (cmd, con);

			sqlcmd.CommandType = CommandType.Text;
			if (con.State != ConnectionState.Closed) {
				con.Close ();
			}
			con.Open ();
			sqlcmd.ExecuteNonQuery ();
			con.Close ();
		}
		public void ExecuteNonQueryAsync(string cmd)
		{
			SqliteConnection con = new SqliteConnection ("Data Source=" + GetDB());
			SqliteCommand sqlcmd = new SqliteCommand (cmd, con);

			sqlcmd.CommandType = CommandType.Text;
			if (con.State != ConnectionState.Closed) {
				con.Close ();
			}
			con.Open ();
			sqlcmd.ExecuteNonQueryAsync ();
			con.Close ();
		}
		public DataTable ExecuteDataTable(string Command)
		{
			DataTable result = new DataTable();
			SqliteConnection con = new SqliteConnection ("Data Source=" + GetDB());
			SqliteCommand command = new SqliteCommand (Command, con);
			con.Open ();
			SqliteDataReader reader = command.ExecuteReader ();
			var len = reader.FieldCount;

			for (int i = 0; i < len; i++)
				result.Columns.Add (reader.GetName (i), reader.GetFieldType (i));

			result.BeginLoadData ();

			var values = new object[len];

			// Add data rows
			while (reader.Read ()) {
				for (int i = 0; i < len; i++)
					values[i] = reader[i];

				result.Rows.Add (values);
			}

			result.EndLoadData ();

			con.Close ();
			reader.Close ();
			reader.Dispose ();

			return result;
		}
	}
}

