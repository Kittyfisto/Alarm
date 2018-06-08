using System;
using System.Media;
using System.Reflection;
using log4net;

namespace Alarm.BusinessLogic
{
	public sealed class AlarmSound
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly SoundPlayer _player;
		private bool _isPlaying;

		public AlarmSound(string path)
		{
			_player = new SoundPlayer(path);
		}

		public void Play()
		{
			if (!_isPlaying)
			{
				try
				{
					_player.PlayLooping();
				}
				catch (Exception e)
				{
					Log.ErrorFormat("Caught unexpected exception: {0}", e);
				}
				_isPlaying = true;
			}
		}

		public void Stop()
		{
			_player.Stop();
			_isPlaying = false;
		}
	}
}