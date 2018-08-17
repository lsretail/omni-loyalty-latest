using System;
using System.Collections.Generic;

namespace Presentation.Screens
{
	public class HomeScreenCollectionSource : CardCollectionSource
	{
		public HomeScreen controller;

		public HomeScreenCollectionSource (HomeScreen controller)
		{
			this.controller = controller;

			BuildHeaderTemplates ();
			BuildCellTemplates();
		}

		public override void BuildCellTemplates()
		{
			CellTemplate cellTemplate;
			for (int id = 1; id <= 6; id++)
			{
				cellTemplate = new CellTemplate ();
				cellTemplate.Id = id;
				if (id == 1 || id == 4) 
				{
					cellTemplate.Size = this.controller.CellSize;
					cellTemplate.ImageId = "Vinafata.png";
					cellTemplate.Title = "Vinafata";
				}
				else
				{
					cellTemplate.Size = this.controller.CellSize;
					cellTemplate.ImageId = "zinger_demo.png";
					cellTemplate.Title = "Tower Zinger";
				}

				cellTemplate.ObjectToDisplay = null;

				cellTemplate.OnSelected = (x) => {
					controller.CellSelected (x);
				};

				cellTemplate.LocalImage = true;

				this.cellTemplateList.Add (cellTemplate);
			}
		}

		public override void BuildHeaderTemplates()
		{
			// Don't do anything here if you don't want a header

			/*this.headerTemplateList = new List<HeaderTemplate> ();

			HeaderTemplate headerTemplate = new HeaderTemplate ();
			headerTemplate.Id = 1;
			headerTemplate.ShowTitle = true;
			headerTemplate.Title = "Offertext";
			headerTemplate.ImageFileName = "Vinafata.png";
			headerTemplate.onSelected =  (x) => {
				controller.HeaderSelected (x);
			};

			this.headerTemplateList.Add (headerTemplate);*/
		}
	}
}

