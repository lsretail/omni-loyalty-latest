using System.Linq;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation.Screens
{
	public class LocationsCollectionSource : CardCollectionSource
	{
		private LocationsCardCollectionController controller;

		public LocationsCollectionSource(LocationsCardCollectionController controller)
		{
			this.controller = controller;

			BuildHeaderTemplates();
			BuildCellTemplates();
		}

		public override void BuildCellTemplates()
		{
			CellTemplate cellTemplate;
			int cellId = 1;

			// Only show map cell if we have store data
			if (this.controller.Stores.Count > 0)
			{
				// Map cell (map of all stores)
				cellTemplate = new CellTemplate();
				cellTemplate.Id = cellId; // Arbitrary id
				cellTemplate.Size = this.controller.CellSize;
				cellTemplate.ImageId = "/Other/map_cell_background.png";
				cellTemplate.LocalImage = true;
				cellTemplate.Title = LocalizationUtilities.LocalizedString("Locations_MapOfAllLocations", "Map of all locations");
				cellTemplate.ObjectToDisplay = null;
				cellTemplate.OnSelected = (x) =>
				{
					controller.MapCellSelected();
				};
				this.cellTemplateList.Add(cellTemplate);
				cellId++;
			}

			foreach (Store store in this.controller.Stores)
			{
				ImageView imgView = store.Images.FirstOrDefault();

				cellTemplate = new CellTemplate();
				cellTemplate.Id = cellId;
				cellTemplate.Size = this.controller.CellSize;
				cellTemplate.ImageId = (imgView != null ? imgView.Id : string.Empty);
				cellTemplate.ImageColorHex = (imgView != null ? imgView.AvgColor : string.Empty);
				cellTemplate.LocalImage = false;
				cellTemplate.Title = store.Description;

				cellTemplate.ObjectToDisplay = store;

				cellTemplate.OnSelected = (x) =>
				{
					controller.CellSelected(x);
				};

				this.cellTemplateList.Add(cellTemplate);
				cellId++;
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

		public void RefreshCellTemplates()
		{
			this.cellTemplateList.Clear();
			BuildCellTemplates();
		}

		public void RefreshHeaderTemplates()
		{
			this.headerTemplateList.Clear();
			BuildHeaderTemplates();
		}
	}
}

