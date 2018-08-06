using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Alarm.UI.Programs
{
	public sealed class AddProgramViewModel
		: INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
