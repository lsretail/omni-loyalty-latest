using System;
using UIKit;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base;

namespace Presentation.Models
{
	public class BaseModel
	{
		public BaseModel ()
		{
		}

		public void HandleException(Exception ex, string method, bool displayAlert = true)
		{
			if (ex is AggregateException)
			{
				foreach (Exception innerException in (ex as AggregateException).InnerExceptions)
					HandleException(innerException, method, displayAlert);
			} 
			else
			{
				HandleUIException(ex, method, displayAlert);
			}
		}

		public static string GetErrorMessageString(LSOmniException ex)
		{
			switch (ex.StatusCode)
			{
				case StatusCode.AuthFailed:
					return LocalizationUtilities.LocalizedString("Model_AuthenticationFailedException", "Incorrect user name or password. Please try again.");

				case StatusCode.CommunicationFailure:
					return LocalizationUtilities.LocalizedString("Model_NetworkException", "Network error");

				case StatusCode.DeviceIsBlocked:
					return LocalizationUtilities.LocalizedString("Model_DeviceBlocked", "This device has been blocked");

				case StatusCode.AccessNotAllowed:
					return LocalizationUtilities.LocalizedString("Model_AccessNotAllowed", "Access denied");

				case StatusCode.EmailExists:
					return LocalizationUtilities.LocalizedString("Model_EmailExists", "Email does not exist");

				case StatusCode.SecurityTokenInvalid:
					return LocalizationUtilities.LocalizedString("Model_SecurityToken", "Access denied");

				case StatusCode.PasswordInvalid:
					return LocalizationUtilities.LocalizedString("Model_InvalidPassword", "Invalid password");

				case StatusCode.UserNameInvalid:
					return LocalizationUtilities.LocalizedString("Model_InvalidUsername", "Invalid username");

				case StatusCode.PasswordOldInvalid:
					return LocalizationUtilities.LocalizedString("Model_InvalidOldPassword", "Incorrect old password");

				case StatusCode.ItemNotFound:
					return LocalizationUtilities.LocalizedString("Model_ItemNotFound", "Item not found");

				case StatusCode.EmailInvalid:
					return LocalizationUtilities.LocalizedString("Model_EmailInvalidException", "Email invalid");

				default:
					return LocalizationUtilities.LocalizedString("Model_RequestError", "Could not process request");
			}
		}


		private void HandleUIException(Exception ex, string method, bool displayAlert = true) 
		{
			System.Diagnostics.Debug.WriteLine ("{0}: {1} {2} - {3}", method, ex.GetType().ToString(), ex.Message, ex.StackTrace);

			string msg = LocalizationUtilities.LocalizedString("Model_GenericException", "Error, please try again");


			if(ex is LSOmniException)
			{
				msg = GetErrorMessageString((LSOmniException)ex);	
			} else {
				msg = ex.Message;
			}


			if (displayAlert)
			{
				UIApplication.SharedApplication.InvokeOnMainThread(async () => {
					await AlertView.ShowAlert(
					    null,
						msg,
						string.Empty,
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);
				});
			}
		}
	}
}

