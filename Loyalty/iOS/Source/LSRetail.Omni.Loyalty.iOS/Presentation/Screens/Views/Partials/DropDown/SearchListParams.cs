using System;
namespace Presentation
{
	public class SearchListParams
	{
		public SearchListParams()
		{
		}

		public SearchListParams(string id, string description)
		{
			Id = id;
			Description = description;
		}

		public string Id { get; set; }
		public string Description { get; set; }
	}
}

