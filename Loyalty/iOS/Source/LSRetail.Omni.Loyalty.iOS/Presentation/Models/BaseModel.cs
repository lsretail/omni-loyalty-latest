using System;
using UIKit;
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
					HandleUIException(innerException, method, displayAlert);
			}
			else
			{
				HandleUIException(ex, method, displayAlert);
			}
		}

		private void HandleUIException(Exception ex, string method, bool displayAlert = true) 
		{
			System.Diagnostics.Debug.WriteLine ("{0}: {1} {2} - {3}", method, ex.GetType().ToString(), ex.Message, ex.StackTrace);

			string msg = LocalizationUtilities.LocalizedString("Model_GenericException", "Error, please try again");

            if(ex is LSOmniException)
            {
                if(((LSOmniException)ex).StatusCode == StatusCode.AuthFailed)
                {
                    msg = LocalizationUtilities.LocalizedString("Model_AuthenticationFailedException", "Incorrect user name or password. Please try again.");
                }
				/*if (ex.GetType() == typeof(NetworkException))
				{
					msg = LocalizationUtilities.LocalizedString("Model_NetworkException", "Network error");
				}*/

				else if (((LSOmniException)ex).StatusCode == StatusCode.DeviceIsBlocked)
                {
                    msg = LocalizationUtilities.LocalizedString("Model_DeviceBlocked", "This device has been blocked");
                }
                else if (((LSOmniException)ex).StatusCode == StatusCode.AccessNotAllowed)
                {
                    msg = LocalizationUtilities.LocalizedString("Model_AccessNotAllowed", "Access denied");
                }
				else if (((LSOmniException)ex).StatusCode == StatusCode.EmailExists)
                {
                    msg = LocalizationUtilities.LocalizedString("Model_EmailExists", "Email already exists. Please try again");
                }
                else if (((LSOmniException)ex).StatusCode == StatusCode.SecurityTokenInvalid)
                {
                    msg = LocalizationUtilities.LocalizedString("Model_SecurityToken", "Access denied");
                }
                else if (((LSOmniException)ex).StatusCode == StatusCode.PasswordInvalid)
                {
                    msg = LocalizationUtilities.LocalizedString("Model_InvalidPassword", "Invalid password");
                }
                else if (((LSOmniException)ex).StatusCode == StatusCode.UserNameInvalid)
                {
                   msg = LocalizationUtilities.LocalizedString("Model_InvalidUsername", "Invalid username"); 
                }
				else if (((LSOmniException)ex).StatusCode == StatusCode.UserNameExists)
                {
                    msg = LocalizationUtilities.LocalizedString("Model_UsernameExists", "This email/username already exists. Please try again");
                }
                else if (((LSOmniException)ex).StatusCode == StatusCode.UserNotLoggedIn)
                {
                    msg = LocalizationUtilities.LocalizedString("Model_UserNotLoggedIn", "User not logged in");
                }
                else if (((LSOmniException)ex).StatusCode == StatusCode.PasswordOldInvalid)
                {
                    msg = LocalizationUtilities.LocalizedString("Model_InvalidOldPassword", "Incorrect old password");
                }
                else if (((LSOmniException)ex).StatusCode == StatusCode.ItemNotFound)
                {
                    msg = LocalizationUtilities.LocalizedString("Model_ItemNotFound", "Item not found");
                }
                else if (((LSOmniException)ex).StatusCode == StatusCode.EmailInvalid)
                {
                    msg = LocalizationUtilities.LocalizedString("Model_EmailInvalidException", "Email invalid");
                }
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

