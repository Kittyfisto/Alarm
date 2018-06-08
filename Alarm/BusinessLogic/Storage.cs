using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IsabelDb;

namespace Alarm.BusinessLogic
{
	public sealed class Storage
		: IDisposable
	{
		private readonly SerialTaskScheduler _scheduler;
		private IsabelDb.IDictionary<Guid, Alarm> _alarms;
		private IDatabase _database;

		public Storage()
		{
			_scheduler = new SerialTaskScheduler();
			OpenAsync();
		}

		#region IDisposable

		public void Dispose()
		{
			_database?.Dispose();
			_scheduler.Dispose();
		}

		#endregion

		public Task OpenAsync()
		{
			return _scheduler.StartNew(() =>
			{
				Directory.CreateDirectory(Constants.ApplicationData);
				var databasePath = Path.Combine(Constants.ApplicationData, "Storage.db");
				_database = Database.OpenOrCreate(databasePath, new[] {typeof(Alarm)});
				_alarms = _database.GetDictionary<Guid, Alarm>("Alarms");
			});
		}

		public Task<IEnumerable<KeyValuePair<Guid, Alarm>>> GetAllAlarms()
		{
			return _scheduler.StartNew(() => _alarms.GetAll());
		}

		public Guid Add(Alarm alarm)
		{
			var id = Guid.NewGuid();
			_scheduler.StartNew(() => { _alarms.Put(id, alarm); });
			return id;
		}

		public void Remove(Guid alarmId)
		{
			_scheduler.StartNew(() => { _alarms.Remove(alarmId); });
		}
	}
}