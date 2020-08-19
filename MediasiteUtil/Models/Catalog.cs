using System;

namespace MediasiteUtil.Models
{
	public class Catalog
	{
		public string Id { get; set; }
		public string LinkedFolderId { get; set; }
		public string Name { get; set; }
		public string FriendlyName { get; set; }
		public string Description { get; set; }
		public bool IsChannel { get; set; }
		public string CatalogUrl { get; set; }
		public bool Recycled { get; set; }
		public string Owner { get; set; }
		public DateTime CreationDate { get; set; }
		public DateTime LastModified { get; set; }
		public bool IsListedInShowcase { get; set; }
		public bool IsMyMediasiteChannel { get; set; }
		public bool IsSearchBased { get; set; }
		public bool IsSpotlightChannel { get; set; }
		public bool LimitSearchToCatalog { get; set; }
		public bool IncludeSubFolders { get; set; }
	}
}