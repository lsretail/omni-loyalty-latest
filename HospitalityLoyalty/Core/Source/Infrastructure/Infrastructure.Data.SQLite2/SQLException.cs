using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Data.SQLite2
{
	public class SQLException : System.Exception
	{
		public SQLStatusCode StatusCode { get; set; }

		public SQLException()
		{ }

		public SQLException(SQLStatusCode statusCode, Exception innerException = null) : base(string.Format("SQLException, code {0}", statusCode.ToString()), innerException)
		{
			this.StatusCode = statusCode;
		}
	}
}
