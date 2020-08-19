using System;

namespace MediasiteUtil.Models
{
	public class Template
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool PollsEnabled { get; set; }
		public bool ForumsEnabled { get; set; }
		public bool IsLive { get; set; }
		public bool IsUploadAutomatic { get; set; }
		public bool IsOnDemand { get; set; }
		public int TimeZoneId { get; set; }
		public string TimeZoneRegistryKey { get; set; }
		public string Owner { get; set; }
		public string ParentFolderId { get; set; }
		public bool ReviewEditApproveEnabled { get; set; }
		public bool ReplaceAclWithPolicy { get; set; }
	}
}