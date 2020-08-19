using System;

namespace MediasiteUtil.Models
{
	public static class RecorderStates
	{
		public static string Unknown = "Unknown";
		public static string Idle = "Idle";
		public static string Busy = "Busy";
		public static string RecordStart = "RecordStart";
		public static string Recording = "Recording";
		public static string RecordEnd = "RecordEnd";
		public static string Pausing = "Pausing";
		public static string Paused = "Paused";
		public static string Resuming = "Resuming";
		public static string OpeningSession = "OpeningSession";
		public static string ConfiguringDevices = "ConfiguringDevices";
	}
}
