using System;

namespace MediasiteUtil.Models
{
	public class Schedule
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string TitleType { get; set; }
		public string FolderId { get; set; }
		public string ScheduleTemplateId { get; set; }
		public bool IsLive { get; set; }
		public bool IsUploadAutomatic { get; set; }
		public string RecorderWebServiceUrl { get; set; }
		public string RecorderEncryptionKey { get; set; }
		public string RecorderId { get; set; }
		public string RecorderName { get; set; }
		public string RecorderUsername { get; set; }
		public string RecorderPassword { get; set; }
		public int AdvanceCreationTime { get; set; }
		public int AdvanceLoadTimeInSeconds { get; set; }
		public string ReceipientsEmailAddresses { get; set; }
		public bool CreatePresentation { get; set; }
		public bool LoadPresentation { get; set; }
		public bool AutoStart { get; set; }
		public bool AutoStop { get; set; }
		public string SendersEmail { get; set; }
		public string CDNPublishingPoint { get; set; }
		public int NextNumberInSchedule { get; set; }
		public bool NotifyPresenter { get; set; }
		public string TimeZoneRegistryKey { get; set; }
		public DateTime LastModified { get; set; }
		public bool DeleteInactive { get; set; }
		public string Description { get; set; }
		public bool IsForumEnabled { get; set; }
		public bool IsOnDemand { get; set; }
		public bool IsPollsEnabled { get; set; }
		public bool ReviewEditApproveEnabled { get; set; }
		public bool ReplaceAclWithPolicy { get; set; }
		public string PlayerId { get; set; }
	}
}