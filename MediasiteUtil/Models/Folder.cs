using System;
using System.Collections.Generic;

namespace MediasiteUtil.Models
{
	public class Folder
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Owner { get; set; }
		public string Description { get; set; }
		public DateTime CreationDate { get; set; }
		public DateTime LastModified { get; set; }
		public string ParentFolderId { get; set; }
		public bool Recycled { get; set; }
		public string Type { get; set; }
		public bool IsShared { get; set; }
		public bool IsCopyDestination { get; set; }
		public bool IsReviewEditApproveEnabled { get; set; }

		/// <summary>
		/// Flattened name showing depth of folder hierarchy - not part of Mediasite schema
		/// </summary>
		public string FlatName { get; set; }

		public List<Folder> ChildFolders { get; set; }

		public Folder(Folder obj)
		{
			this.Id = obj.Id;
			this.Name = obj.Name;
			this.Owner = obj.Owner;
			this.Description = obj.Description;
			this.CreationDate = obj.CreationDate;
			this.LastModified = obj.LastModified;
			this.ParentFolderId = obj.ParentFolderId;
			this.Recycled = obj.Recycled;
			this.Type = obj.Type;
			this.IsShared = obj.IsShared;
			this.IsCopyDestination = obj.IsCopyDestination;
			this.IsReviewEditApproveEnabled = obj.IsReviewEditApproveEnabled;
		}

		public Folder()
		{
		}
	}
}