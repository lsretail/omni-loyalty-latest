using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Base.Exceptions
{
	public class PasswordOldInvalidException : Exception
	{
		public PasswordOldInvalidException (string message) : base(message)
		{
		}
	}
}


