using System;
using System.IO;
using System.Xml.Serialization;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup.SpecialCase;
using LSRetail.Omni.Domain.Services.Loyalty.Devices;

#if USE_CSHARP_SQLITE
using Community.CsharpSqlite.SQLiteClient;
#else
using Mono.Data.Sqlite;
#endif
using Newtonsoft.Json;

namespace Infrastructure.Data.SQLite2.Devices
{
    public class DeviceRepository : IDeviceLocalRepository
	{
		private static readonly string sqlGetDeviceData = "Select * FROM DEVICEDATA";
		private static readonly string sqlInsertDeviceData = @"INSERT OR REPLACE INTO DEVICEDATA VALUES (@ID, @DEVICEDATAXML)";
		private static readonly string sqlDeleteDeviceData = @"DELETE FROM DEVICEDATA";

		public Device GetDevice()
		{
			Device device = null;

			try
			{
				var conn = SqliteHelper.DatabaseConnection();

				//XmlSerializer serializer = new XmlSerializer(typeof(Device));

				SqliteCommand cmd = new SqliteCommand(sqlGetDeviceData, conn);

				SqliteDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					var json = reader["DEVICEDATAXML"].ToString();

                    JsonConvert.DeserializeObject<Device>(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
				}


				reader.Close();

				cmd.Dispose();

				SqliteHelper.CloseDBConnection(conn);

				if(device == null)
				{
					return new UnknownDevice();
				}
				else
				{
					if(device.UserLoggedOnToDevice != null && device.UserLoggedOnToDevice.LoggedOnToDevice == null)
						device.UserLoggedOnToDevice.LoggedOnToDevice = device;

					return device;
				}
			}
			catch (Exception x)
			{
				throw new SQLException(SQLStatusCode.GetDeviceError, x);
			}
		}

		public void SaveDevice(Device device)
		{
			try
			{
				using (var conn = new SqliteConnection(SqliteHelper.DatabaseConnectionString()))
				{
					conn.Open();
                  

					using (SqliteTransaction dbTrans = (SqliteTransaction)conn.BeginTransaction())
					{
						using (SqliteCommand cmd = (SqliteCommand)conn.CreateCommand())
						{
							cmd.CommandText = sqlDeleteDeviceData;
							cmd.ExecuteNonQuery();
						}

						using (SqliteCommand cmd = (SqliteCommand)conn.CreateCommand())
						{
							cmd.CommandText = sqlInsertDeviceData;

							SqliteParameter ID = new SqliteParameter("@ID", System.Data.DbType.String);
							SqliteParameter DEVICEDATAXML = new SqliteParameter("@DEVICEDATAXML", System.Data.DbType.String);

							cmd.Parameters.Add(ID);
							cmd.Parameters.Add(DEVICEDATAXML);

                            var deviceXml = JsonConvert.SerializeObject(device, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

							ID.Value = device.Id;
							DEVICEDATAXML.Value = deviceXml;

							cmd.ExecuteNonQuery();
						}

						dbTrans.Commit();
					}

					conn.Close();
				}
			}
			catch (Exception ex)
			{
				throw new SQLException(SQLStatusCode.SaveDeviceError, ex);
			}
		}
	}
}

