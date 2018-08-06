using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace Alarm
{
	public static class Bootstrapper
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		[STAThread]
		public static int Main(string[] args)
		{
			try
			{
				ConfigureLog4Net();
				HandleExceptions();

				return Run();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return -1;
			}
		}

		private static int Run()
		{
			try
			{
				return App.Run();
			}
			catch (Exception e)
			{
				Log.FatalFormat("Caught unhandled exception: {0}", e);
				return -1;
			}
		}

		private static void ConfigureLog4Net()
		{
			var hierarchy = (Hierarchy) LogManager.GetRepository();

			var patternLayout = new PatternLayout
			{
				ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"
			};
			patternLayout.ActivateOptions();

			var fileAppender = new RollingFileAppender
			{
				AppendToFile = false,
				File = Constants.ApplicationLogFile,
				Layout = patternLayout,
				MaxSizeRollBackups = 20,
				MaximumFileSize = "1GB",
				RollingStyle = RollingFileAppender.RollingMode.Size,
				StaticLogFileName = false
			};
			fileAppender.ActivateOptions();
			hierarchy.Root.AddAppender(fileAppender);

			hierarchy.Root.Level = Level.Info;
			hierarchy.Configured = true;
		}

		private static void HandleExceptions()
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
			Dispatcher.CurrentDispatcher.UnhandledException += CurrentDispatcherOnUnhandledException;
			TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
		}

		private static void CurrentDispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Log.ErrorFormat("Caught unexpected exception: {0}", e.Exception);
			e.Handled = true;
		}

		private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Log.ErrorFormat("Caught unexpected exception: {0}", e.ExceptionObject);
		}

		private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			Log.ErrorFormat("Caught unexpected exception: {0}", e.Exception);
			e.SetObserved();
		}
	}
}