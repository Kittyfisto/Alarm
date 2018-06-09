using System.Reflection;
using System.Windows;
using Alarm.BusinessLogic;
using Alarm.UI;
using log4net;

namespace Alarm
{
	public sealed class App
		: Application
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public new static int Run()
		{
			Log.InfoFormat("Starting application...");

			using (var storage = new Storage())
			{
				var devices = new Configuration();
				var soundAlarm = new AlarmSound(Constants.AlarmSoundFile);

				var app = new App();
				var viewModel = new MainWindowViewModel(app.Dispatcher, storage, devices, soundAlarm);
				var window = new MainWindow(viewModel);
				window.Show();

				Log.InfoFormat("Application started");

				return ((Application) app).Run();
			}
		}
	}
}
