using System;
using RestSharp.Deserializers;

namespace MediasiteUtil.Models
{
	public class PresentationDefaultRepresentation
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public string Status { get; set; }
		[DeserializeAs(Name = "#Play")]
		public TargetClass PlayUrl { get; set; }

		public PresentationDefaultRepresentation(PresentationDefaultRepresentation obj)
		{
			this.Id = obj.Id;
			this.Title = obj.Title;
			this.Status = obj.Status;
		}

		public PresentationDefaultRepresentation() { }
	}


	public class PresentationCardRepresentation : PresentationDefaultRepresentation
	{
		public string Description { get; set; }
		public DateTime? RecordDate { get; set; }
		public DateTime? RecordDateLocal { get; set; }
		public int Duration { get; set; }
		public int NumberOfViews { get; set; }
		public string Owner { get; set; }
		public string PrimaryPresenter { get; set; }
		public string ThumbnailUrl { get; set; }
		public bool IsLive { get; set; }
		public DateTime? CreationDate { get; set; }

		public PresentationCardRepresentation() : base() { }

		public PresentationCardRepresentation(PresentationCardRepresentation obj)
			: base(obj)
		{
			this.Description = obj.Description;
			this.Status = obj.Status;
			this.PrimaryPresenter = obj.PrimaryPresenter;
			this.ThumbnailUrl = obj.ThumbnailUrl;
			this.RecordDate = obj.RecordDate;
			this.RecordDateLocal = obj.RecordDateLocal;
			this.Duration = obj.Duration;
			this.NumberOfViews = obj.NumberOfViews;
			this.Owner = obj.Owner;
			this.CreationDate = obj.CreationDate;
			this.IsLive = obj.IsLive;
		}
	}

	public class PresentationFullRepresentation : PresentationCardRepresentation
	{
		public string RootId { get; set; }
		public string PlayerId { get; set; }
		public string PresentationTemplateId { get; set; }
		public string AlternateName { get; set; }
		public string CopyrightNotice { get; set; }
		public int MaximumConnections { get; set; }
		public string PublishingPointName { get; set; }
		public bool IsUploadAutomatic { get; set; }
		public string TimeZone { get; set; }
		public bool PollsEnabled { get; set; }
		public bool ForumsEnabled { get; set; }
		public bool SharingEnabled { get; set; }
		public bool PlayerLocked { get; set; }
		public bool PollsInternal { get; set; }
		public bool Private { get; set; }
		public bool NotifyOnMetadataChanged { get; set; }
		public string ApprovalState { get; set; }
		public string ApprovalRequiredChangeTypes { get; set; }
		public int ContentRevision { get; set; }
		public string PollLink { get; set; }
		public string ParentFolderName { get; set; }
		public string ParentFolderId { get; set; }
		public DateTime? DisplayRecordDate { get; set; }

		public PresentationFullRepresentation() : base() { }
	}
}