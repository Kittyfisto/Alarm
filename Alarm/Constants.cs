using System;
using System.IO;
using System.Reflection;

namespace Alarm
{
	public static class Constants
	{
		public static readonly string ApplicationData;
		public static readonly string ApplicationLogFile;
		public static readonly string DevicesFile;
		public static readonly string ProgramFiles;
		public static readonly string AlarmSoundFile;

		static Constants()
		{
			var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			ApplicationData = Path.Combine(appData, "Alarms");
			ApplicationLogFile = Path.Combine(ApplicationData, "Alarm.log");
			DevicesFile = Path.Combine(ApplicationData, "Devices.xml");

			ProgramFiles = AssemblyDirectory;
			AlarmSoundFile = Path.Combine(ProgramFiles, "Alarm.wav");
		}

		public static string AssemblyDirectory
		{
			get
			{
				string codeBase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}
	}
}