using System;
using UIKit;
using AdSupport;
using Security;
using Foundation;
using ObjCRuntime;
using CoreLocation;
using System.Linq;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;

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
				var query = new SecRecord(SecKind.GenericPassword);
				query.Service = NSBundle.MainBundle.BundleIdentifier;
				query.Account = "PhoneId";

				// get the phoneId
				NSData phoneId = SecKeyChain.QueryAsData(query);

				// if the phoneId doesn't exist, we create it
				if (phoneId == null)
				{
					string model = UIDevice.CurrentDevice.Model; //iPhone  iPad

					if (string.IsNullOrWhiteSpace(model))
					{
						model = "i?";
					}
					else
					{
						if (model.Length > 8)
							model = model.Substring(0, 8);
					}

					model = model + "-iOS" + Util.GetOSVersion().Major.ToString() + "-";

					query.ValueData = model + NSData.FromString(Guid.NewGuid().ToString());
					var result = SecKeyChain.Add(query);
					if ((result != SecStatusCode.Success) && (result != SecStatusCode.DuplicateItem))
						throw new Exception("Cannot store PhoneId");

					Console.WriteLine(query.ValueData.Length.ToString());

					return query.ValueData.ToString();
				}
				else
				{
					return phoneId.ToString();
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

		public static string FormatQty(decimal qty)
		{

			if (qty > 0 && qty % 1 == 0)
				return qty.ToString("#");
			else
				return qty.ToString();
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

		public static int GetStringLineCount(string str)
		{
			return str.Split('\n').Length;
		}

		#region Generate extra info string

		public static string GenerateItemExtraInfo(MenuItem item)
		{
			/*bool blue = true;
			if (blue)
			{
				return "";
			}*/
			var extraInfo = string.Empty;

			if (item is Recipe)
			{
				var recipe = item as Recipe;

				foreach (var ingredient in recipe.Ingredients)
				{
					extraInfo += GenerateIngredientExtraInfo(ingredient);
				}

				//TODO:This is weird
				/*foreach (var recipeLine in recipe.RecipeLines)
				{*/
					foreach (var productModifierGroup in recipe.ProductModifierGroups)
					{
						foreach (var productModifier in productModifierGroup.ProductModifiers)
						{
							extraInfo += GenerateModifierExtraInfo(productModifierGroup, productModifier);
						}
					}
				//}
			}
			else if (item is MenuDeal)
			{
				var deal = item as MenuDeal;

				foreach (var dealLine in deal.DealLines)
				{
					var dealLineItem = dealLine.DealLineItems.FirstOrDefault(x => x.Id == dealLine.SelectedId);

					if (dealLineItem != null)
					{
						extraInfo += dealLineItem.Quantity > 1 ? Utils.Util.FormatQty(dealLineItem.Quantity) + " " + dealLineItem.Description : dealLineItem.Description;

						if (dealLineItem.PriceAdjustment != 0m)
						{
							string formattedPriceAdjustment = AppData.MobileMenu != null ? AppData.MobileMenu.Currency.FormatDecimal(dealLineItem.Quantity * dealLineItem.PriceAdjustment) : dealLineItem.PriceAdjustment.ToString();
							extraInfo += " (+" + formattedPriceAdjustment + ")" + System.Environment.NewLine;
						}
						else
						{
							extraInfo += System.Environment.NewLine;
						}
					}

					if (dealLineItem != null && dealLineItem.MenuItem is Recipe)
					{
						var recipeInfo = GenerateItemExtraInfo(dealLineItem.MenuItem);  // NOTE: Recursion

						if (!string.IsNullOrEmpty(recipeInfo))
						{
							extraInfo += recipeInfo + System.Environment.NewLine;
						}
					}

					foreach (var dealModifierGroup in dealLine.DealModifierGroups)
					{
						foreach (var dealModifier in dealModifierGroup.DealModifiers)
						{
							extraInfo += GenerateModifierExtraInfo(dealModifierGroup, dealModifier);
						}
					}
				}
			}

			return extraInfo.TrimEnd(System.Environment.NewLine.ToCharArray());
		}

		private static string GenerateIngredientExtraInfo(Ingredient ingredient)
		{
			string extraInfo = string.Empty;

			MenuService service = new MenuService();
			IngredientItem item = service.GetItem(AppData.MobileMenu, ingredient.Id);

			if (ingredient.Quantity > ingredient.OriginalQuantity)
			{
				if (string.IsNullOrEmpty(ingredient.UnitOfMeasure))
				{
					extraInfo = string.Format(LocalizationUtilities.LocalizedString("SlideoutBasket_ExtraLineFormatStringPlus", "+ {0} {1}"), ingredient.Quantity - ingredient.OriginalQuantity, item.Description) + System.Environment.NewLine;
				}
				else
				{
					// TODO Show UOM?
					extraInfo = string.Format(LocalizationUtilities.LocalizedString("SlideoutBasket_ExtraLineFormatStringPlus", "+ {0} {1}"), ingredient.Quantity - ingredient.OriginalQuantity, item.Description) + System.Environment.NewLine;
					//extraInfo = string.Format(LocalizationUtilities.LocalizedString("SlideoutBasket_ExtraLineFormatStringPlusUom", "+ {0} {1} {2}"), ingredient.Qty - ingredient.OriginalQty, ingredient.Uom, ingredient.Item.Description) + System.Environment.NewLine;
				}
			}
			else if (ingredient.Quantity < ingredient.OriginalQuantity)
			{
				if (string.IsNullOrEmpty(ingredient.UnitOfMeasure))
				{
					extraInfo = string.Format(LocalizationUtilities.LocalizedString("SlideoutBasket_ExtraLineFormatStringMinus", "- {0} {1}"), ingredient.OriginalQuantity - ingredient.Quantity, item.Description) + System.Environment.NewLine;
				}
				else
				{
					// TODO Show UOM?
					extraInfo = string.Format(LocalizationUtilities.LocalizedString("SlideoutBasket_ExtraLineFormatStringMinus", "- {0} {1}"), ingredient.OriginalQuantity - ingredient.Quantity, item.Description) + System.Environment.NewLine;
					//extraInfo = string.Format(LocalizationUtilities.LocalizedString("SlideoutBasket_ExtraLineFormatStringMinusUom", "- {0} {1} {2}"), ingredient.OriginalQty - ingredient.Qty, ingredient.Uom, ingredient.Item.Description) + System.Environment.NewLine;
				}
			}

			return extraInfo;
		}

		private static string GenerateModifierExtraInfo(ModifierGroup modifierGroup, Modifier modifier)
		{
			string extraInfo = string.Empty;

			if (((modifierGroup.MinimumSelection == modifierGroup.MaximumSelection && modifierGroup.MaximumSelection == 1) || (modifier.MinimumSelection == 0 && modifier.MaximumSelection == 1)) && modifier.Quantity > modifier.OriginalQty)
			{
				if (modifier.Price != 0m)
				{
					string formattedPrice = AppData.MobileMenu != null ? AppData.MobileMenu.Currency.FormatDecimal(modifier.Price) : modifier.Price.ToString();
					extraInfo = string.Format(LocalizationUtilities.LocalizedString("SlideoutBasket_ExtraLineFormatStringModifier", "{0}"), modifier.Description);
					extraInfo += " (+" + formattedPrice + ")" + System.Environment.NewLine;
				}
				else
				{
					extraInfo = string.Format(LocalizationUtilities.LocalizedString("SlideoutBasket_ExtraLineFormatStringModifier", "{0}"), modifier.Description) + System.Environment.NewLine;
				}
			}
			else
			{
				if (modifier.Quantity > modifier.OriginalQty)
				{
					if (string.IsNullOrEmpty(modifier.UnitOfMeasure))
					{
						if (modifier.Price != 0)
						{
							string formattedCurrencyPriceString = AppData.MobileMenu != null ? AppData.MobileMenu.Currency.FormatDecimal((modifier.Quantity - modifier.OriginalQty) * modifier.Price) : ((modifier.Quantity - modifier.OriginalQty) * modifier.Price).ToString();
							extraInfo = string.Format("+ {0} {1} (+{2})", modifier.Quantity - modifier.OriginalQty, modifier.Description, formattedCurrencyPriceString) + System.Environment.NewLine;
						}
						else
						{
							extraInfo = string.Format(LocalizationUtilities.LocalizedString("SlideoutBasket_ExtraLineFormatStringPlus", "+ {0} {1}"), modifier.Quantity - modifier.OriginalQty, modifier.Description) + System.Environment.NewLine;
						}
					}
					else
					{
						// TODO Show UOM?
						if (modifier.Price != 0)
						{
							string formattedCurrencyPriceString = AppData.MobileMenu != null ? AppData.MobileMenu.Currency.FormatDecimal((modifier.Quantity - modifier.OriginalQty) * modifier.Price) : ((modifier.Quantity - modifier.OriginalQty) * modifier.Price).ToString();
							extraInfo = string.Format("+ {0} {1} (+{2})", modifier.Quantity - modifier.OriginalQty, modifier.Description, formattedCurrencyPriceString) + System.Environment.NewLine;

						}
						else
						{
							extraInfo = string.Format(LocalizationUtilities.LocalizedString("SlideoutBasket_ExtraLineFormatStringPlus", "+ {0} {1}"), modifier.Quantity - modifier.OriginalQty, modifier.Description) + System.Environment.NewLine;
						}

						//extraInfo = string.Format(LocalizationUtilities.LocalizedString("SlideoutBasket_ExtraLineFormatStringPlusUom", "+ {0} {1} {2}"), modifier.Qty - modifier.OriginalQty, modifier.Uom, modifier.Description) + System.Environment.NewLine;
					}
				}
				else if (modifier.Quantity < modifier.OriginalQty)
				{
					if (string.IsNullOrEmpty(modifier.UnitOfMeasure))
					{
						extraInfo = string.Format(LocalizationUtilities.LocalizedString("SlideoutBasket_ExtraLineFormatStringMinus", "- {0} {1}"), modifier.OriginalQty - modifier.Quantity, modifier.Description) + System.Environment.NewLine;
					}
					else
					{
						// TODO Show UOM?
						extraInfo = string.Format(LocalizationUtilities.LocalizedString("SlideoutBasket_ExtraLineFormatStringMinus", "- {0} {1}"), modifier.OriginalQty - modifier.Quantity, modifier.Description) + System.Environment.NewLine;
						//extraInfo = string.Format(LocalizationUtilities.LocalizedString("SlideoutBasket_ExtraLineFormatStringMinusUom", "- {0} {1} {2}"), modifier.OriginalQty - modifier.Qty, modifier.Uom, modifier.Description) + System.Environment.NewLine;
					}
				}
			}

			return extraInfo;
		}

		#endregion
	}
}

