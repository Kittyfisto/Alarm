using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using IsabelDb;
using log4net;

namespace Alarm.BusinessLogic
{
	public sealed class Storage
		: IDisposable
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
				try
				{
					var databasePath = Path.Combine(Constants.ApplicationData, "Storage.db");
					Log.InfoFormat("Opening alarm database '{0}'...", databasePath);

					Directory.CreateDirectory(Constants.ApplicationData);
					_database = Database.OpenOrCreate(databasePath, new[] {typeof(Alarm)});
					_alarms = _database.GetDictionary<Guid, Alarm>("Alarms");

					Log.InfoFormat("Alarm database opened!");
				}
				catch (Exception e)
				{
					Log.FatalFormat("Unable to open alarm database: {0}", e);
				}
			});
		}

		public Task<IEnumerable<KeyValuePair<Guid, Alarm>>> GetAllAlarms()
		{
			return _scheduler.StartNew(() => _alarms.GetAll());
		}

		public Guid Add(Alarm alarm)
		{
			var id = Guid.NewGuid();
			_scheduler.StartNew(() =>
			{
				try
				{
					_alarms.Put(id, alarm);
				}
				catch (Exception e)
				{
					Log.ErrorFormat("Caught unexpected exception: {0}", e);
				}
			});
			return id;
		}

		public void Remove(Guid alarmId)
		{
			_scheduler.StartNew(() =>
			{
				try
				{
					_alarms.Remove(alarmId);
				}
				catch (Exception e)
				{
					Log.ErrorFormat("Caught unexpected exception: {0}", e);
				}
			});
		}

		public void Update(Guid id, Alarm alarm)
		{
			var clone = alarm.Clone();
			Log.DebugFormat("Storing alarm '{0}': {1}...", id, clone);
			_scheduler.StartNew(() =>
			{
				try
				{
					_alarms.Put(id, clone);
					Log.DebugFormat("Alarm stored");
				}
				catch (Exception e)
				{
					Log.ErrorFormat("Caught unexpected exception: {0}", e);
				}
			});
		}
	}
}