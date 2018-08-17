using System;
using System.Collections.Generic;
using UIKit;
using Foundation;

namespace Presentation
{
	public class AboutUsTableSource : UITableViewSource
	{
		public List<AboutUsItem> items;
		private NSString cellIdentifier = (NSString)"TableCell";

		public delegate void StringEventHandler (string text);
		public StringEventHandler SetInfoText;
		public StringEventHandler PhoneNumberLinePressed;
		public StringEventHandler EmailLinePressed;
		public StringEventHandler WebsiteLinePressed;

		public AboutUsTableSource(string contactUsInfoString)
		{
			this.items = new List<AboutUsItem>();

			ParseContactUsInfoString(contactUsInfoString);
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return this.items.Count;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			AboutUsItem itemPressed = items[indexPath.Row];

			if (itemPressed.Action != null)
				itemPressed.Action();

			tableView.DeselectRow(indexPath, true);
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			AboutUsItem item = this.items[indexPath.Row];

			AboutUsTableCell cell = new AboutUsTableCell(cellIdentifier, UITableViewCellStyle.Subtitle);
			cell.SetValues(item.Key, item.Value);

			return cell;
		}

		public void ParseContactUsInfoString(string infoString)
		{
			// TODO Better parsing (more resilient/flexible) 
			// (find a third party parser? check this one: http://www.codeproject.com/Articles/57176/Parsing-HTML-Tags-in-C)


			string originalString = infoString;
			int startPos;
			int endPos;

			// Paragraphs - assume that the paragraphs don't contain attributes
			string paragraph;
			List<string> paragraphs = new List<string>();
			while (infoString.IndexOf("<p>") != -1 && infoString.IndexOf("</p>") != -1)
			{
				startPos = infoString.IndexOf("<p>") + "<p>".Length;
				endPos = infoString.IndexOf("</p>");
				paragraph = infoString.Substring(startPos, endPos - startPos);
				paragraphs.Add(paragraph);
				infoString = infoString.Substring(endPos + "</p>".Length);
			}

			// Put paragraphs in text view
			string paragraphText = string.Empty;
			foreach (string p in paragraphs)
			{
				paragraphText += p + System.Environment.NewLine;
			}

			if (SetInfoText != null)
			{
				SetInfoText (paragraphText);
			}
			//this.ctrl.SetInfoText(paragraphText);

			// Anchors - can and will contain attributes
			string anchor;
			List<string> anchors = new List<string>();
			infoString = originalString;
			while (infoString.IndexOf("<a") != -1 && infoString.IndexOf("</a>") != -1)
			{
				startPos = infoString.IndexOf("<a");
				endPos = infoString.IndexOf("</a>") + "</a>".Length;
				anchor = infoString.Substring(startPos, endPos - startPos);
				anchors.Add(anchor);    // entire html node, including tags
				infoString = infoString.Substring(endPos);
			}

			// Map anchors to ContactUsItems - build ContactUsItems list
			string link;
			string linkText;
			foreach (string a in anchors)
			{
				// link
				startPos = a.IndexOf("href=") + "href=".Length;
				endPos = a.IndexOf(">");
				link = a.Substring(startPos, endPos - startPos).Replace("\"", string.Empty).Replace("'", string.Empty);

				// linktext
				startPos = a.IndexOf(">") + 1;
				endPos = a.Substring(startPos).IndexOf("<") + startPos;
				linkText = a.Substring(startPos, endPos - startPos);

				AboutUsItem contactItem = new AboutUsItem();
				contactItem.Key = linkText;
				contactItem.Value = link.Substring(link.IndexOf(":") + 1);
				if (link.StartsWith("tel:"))
				{
					contactItem.Action = new Action(() => {
						if (PhoneNumberLinePressed != null)
						{
							PhoneNumberLinePressed (contactItem.Value);
						}
						//this.ctrl.PhoneNumberLinePressed(contactItem.Value); 
					});
				}
				else if (link.StartsWith("mail:"))
				{
					contactItem.Action = new Action(() => {
						if (EmailLinePressed != null)
						{
							EmailLinePressed (contactItem.Value);
						}

						//this.ctrl.EmailLinePressed(contactItem.Value); 
					});
				}
				else if (link.StartsWith("url:"))
				{
					contactItem.Action = new Action(() => {
						if (WebsiteLinePressed != null)
						{
							WebsiteLinePressed (contactItem.Value);
						}

						//this.ctrl.WebsiteLinePressed(contactItem.Value); 
					});
				}
				else
				{
					contactItem.Action = new Action(() => {});
				}

				this.items.Add(contactItem);
			}
		}

		public class AboutUsItem
		{
			public string Key { get; set; }
			public string Value { get; set; }
			public Action Action { get; set; }
		}
	}
}



