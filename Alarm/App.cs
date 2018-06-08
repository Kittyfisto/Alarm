using System.Windows;
using Alarm.BusinessLogic;
using Alarm.UI;

namespace Alarm
{
	public sealed class App
		: Application
	{
		public new static int Run()
		{
			using (var storage = new Storage())
			{
				var devices = new Devices();
				var soundAlarm = new AlarmSound(Constants.AlarmSoundFile);

				var app = new App();
				var viewModel = new MainWindowViewModel(app.Dispatcher, storage, devices, soundAlarm);
				var window = new MainWindow(viewModel);
				window.Show();
				return ((Application) app).Run();
			}
		}
	}
}
