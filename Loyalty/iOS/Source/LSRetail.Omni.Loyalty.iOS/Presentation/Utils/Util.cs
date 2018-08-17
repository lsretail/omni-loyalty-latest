using System;
using UIKit;
using Security;
using Foundation;
using ObjCRuntime;
using System.Runtime.InteropServices;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation.Utils
{
    public static class Util
	{
		public static AppDelegate AppDelegate
		{
			get
			{
				return (UIApplication.SharedApplication.Delegate as AppDelegate);
			}
		}

		public static string AssemblyVersion
		{
			get
			{
				return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		public static string PhoneId 
		{
			get
			{
				var query = new SecRecord (SecKind.GenericPassword);
				query.Service = NSBundle.MainBundle.BundleIdentifier;
				query.Account = "PhoneId";

				// get the phoneId
				NSData phoneId = SecKeyChain.QueryAsData (query);

				// if the phoneId doesn't exist, we create it
				if(phoneId == null)
				{
					string model = UIDevice.CurrentDevice.Model; //iPhone  iPad

					if (string.IsNullOrWhiteSpace(model))
					{
						model = "i?";
					}
					else
					{
						if(model.Length > 8)
							model = model.Substring(0,8);
					}

					model = model + "-iOS" + Utils.Util.GetOSVersion().Major.ToString() + "-";

					query.ValueData = model + NSData.FromString (System.Guid.NewGuid ().ToString ());
					var result = SecKeyChain.Add (query);
					if ((result != SecStatusCode.Success) && (result != SecStatusCode.DuplicateItem))
						throw new Exception ("Cannot store PhoneId");

					Console.WriteLine(query.ValueData.Length.ToString());

					return query.ValueData.ToString ();
				}
				else
				{
					return phoneId.ToString ();
				}
			}
		}

		public static Version GetOSVersion()
		{
			string versionString = UIDevice.CurrentDevice.SystemVersion.Replace(",", ".");
			Version osVersion;

			try
			{
				osVersion = new Version(versionString);
			}
			catch (Exception) 
			{
				osVersion = new Version("0.0.0.0");
			}

			return osVersion;
		}

		public static void FillDeviceInfo(Device device)
		{
			device.Manufacturer = "Apple";
			device.Platform = "iOS";
			device.OsVersion = UIDevice.CurrentDevice.SystemVersion;
			device.Model = getHardwareVersion();
			device.DeviceFriendlyName = device.Manufacturer + " " + device.Model;
		}

		[DllImport(Constants.SystemLibrary)]
		static internal extern int sysctlbyname ([MarshalAs(UnmanagedType.LPStr)] string property, IntPtr output, IntPtr oldLen, IntPtr newp, uint newlen);

		private static string getHardwareVersion()
		{
			string HardwareProperty = "hw.machine";

			// get the length of the string that will be returned
			var pLen = Marshal.AllocHGlobal (sizeof(int));
			sysctlbyname (HardwareProperty, IntPtr.Zero, pLen, IntPtr.Zero, 0);

			var length = Marshal.ReadInt32 (pLen);

			// check to see if we got a length
			if (length == 0) {
				Marshal.FreeHGlobal (pLen);
				return "";
			}

			// get the hardware string
			var pStr = Marshal.AllocHGlobal (length);
			sysctlbyname (HardwareProperty, pStr, pLen, IntPtr.Zero, 0);

			// convert the native string into a C# string
			var hardwareStr = Marshal.PtrToStringAnsi (pStr);

			// cleanup
			Marshal.FreeHGlobal (pLen);
			Marshal.FreeHGlobal (pStr);

			return hardwareStr;
		}

		#region Location
		/*
		internal class MyCLLocationManagerDelegate : CLLocationManagerDelegate {
			Action<CLLocation> callback;

			public MyCLLocationManagerDelegate (Action<CLLocation> callback)
			{
				this.callback = callback;
			}

			public override void UpdatedLocation (CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation)
			{
				manager.StopUpdatingLocation ();
				locationManager = null;
				callback (newLocation);
			}

			public override void Failed (CLLocationManager manager, NSError error)
			{
				callback (null);
			}

		}

		static CLLocationManager locationManager;
		static public void RequestLocation (Action<CLLocation> callback)
		{
			locationManager = new CLLocationManager () {
				DesiredAccuracy = CLLocation.AccuracyBest,
				Delegate = new MyCLLocationManagerDelegate (callback),
				DistanceFilter = 1000f
			};
			if (CLLocationManager.LocationServicesEnabled)
				locationManager.StartUpdatingLocation ();
		}   
		*/
		#endregion

		/// <summary>
		/// Finds the first responder recursively within the given view and it's subviews
		/// </summary>
		public static UIView FindFirstResponder(UIView view)
		{
			if (view.IsFirstResponder)
			{
				return view;
			}

			foreach (UIView subView in view.Subviews)
			{
				var firstResponder = FindFirstResponder(subView);
				if (firstResponder != null)
					return firstResponder;
			}

			return null;
		}

		public static bool IsModalController(UIViewController controller)
		{			
			if (controller.NavigationController != null 
				&& controller.NavigationController.ViewControllers.Length > 1
				&& controller.NavigationController.TopViewController == controller)
				return false;	

			if (controller.PresentingViewController != null 
				&& controller.PresentingViewController.PresentedViewController == controller)
				return true;

			if (controller.NavigationController != null 
				&& controller.NavigationController.PresentingViewController != null 
				&& controller.NavigationController.PresentingViewController.PresentedViewController == controller.NavigationController)
				return true;		

			return false;
		}

		public static int GetStringLineCount(string str)
		{
			return str.Split('\n').Length;
		}

		public static DateTime NSDateToDateTime(NSDate date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime( 
				new DateTime(2001, 1, 1, 0, 0, 0) );
			return reference.AddSeconds(date.SecondsSinceReferenceDate);
		}

		public static NSDate DateTimeToNSDate(DateTime date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(
				new DateTime(2001, 1, 1, 0, 0, 0) );
			return NSDate.FromTimeIntervalSinceReferenceDate(
				(date - reference).TotalSeconds);
		}

		public static string GetISBN13(string ISBN)
		{
			string isbn10 = "978" + ISBN.Substring(0, 9);
			int isbn10_1 = Convert.ToInt32(isbn10.Substring(0, 1));
			int isbn10_2 = Convert.ToInt32(Convert.ToInt32(isbn10.Substring(1, 1)) * 3);
			int isbn10_3 = Convert.ToInt32(isbn10.Substring(2, 1));
			int isbn10_4 = Convert.ToInt32(Convert.ToInt32(isbn10.Substring(3, 1)) * 3);
			int isbn10_5 = Convert.ToInt32(isbn10.Substring(4, 1));
			int isbn10_6 = Convert.ToInt32(Convert.ToInt32(isbn10.Substring(5, 1)) * 3);
			int isbn10_7 = Convert.ToInt32(isbn10.Substring(6, 1));
			int isbn10_8 = Convert.ToInt32(Convert.ToInt32(isbn10.Substring(7, 1)) * 3);
			int isbn10_9 = Convert.ToInt32(isbn10.Substring(8, 1));
			int isbn10_10 = Convert.ToInt32(Convert.ToInt32(isbn10.Substring(9, 1)) * 3);
			int isbn10_11 = Convert.ToInt32(isbn10.Substring(10, 1));
			int isbn10_12 = Convert.ToInt32(Convert.ToInt32(isbn10.Substring(11, 1)) * 3);
			//int k = (isbn10_1 + isbn10_2 + isbn10_3 + isbn10_4 + isbn10_5 + isbn10_6 + isbn10_7 + isbn10_8 + isbn10_9 + isbn10_10 + isbn10_11 + isbn10_12);
			int checkdigit = 10 - ((isbn10_1 + isbn10_2 + isbn10_3 + isbn10_4 + isbn10_5 + isbn10_6 + isbn10_7 + isbn10_8 + isbn10_9 + isbn10_10 + isbn10_11 + isbn10_12) % 10);
			if (checkdigit == 10)
				checkdigit = 0;
			return isbn10 + checkdigit.ToString();
		}
	}
}

