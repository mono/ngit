using NGit;
using NGit.Events;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Events
{
	public class ConfigChangeEventTest : RepositoryTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestFileRepository_ChangeEventsOnlyOnSave()
		{
			ConfigChangedEvent[] events = new ConfigChangedEvent[1];
			db.Listeners.AddConfigChangedListener(new _ConfigChangedListener_52(events));
			FileBasedConfig config = ((FileBasedConfig)db.GetConfig());
			NUnit.Framework.Assert.IsNull(events[0]);
			// set a value to some arbitrary key
			config.SetString("test", "section", "event", "value");
			// no changes until we save
			NUnit.Framework.Assert.IsNull(events[0]);
			config.Save();
			NUnit.Framework.Assert.IsNotNull(events[0]);
			// correct repository?
			NUnit.Framework.Assert.AreEqual(events[0].GetRepository(), db);
			// reset for the next test
			events[0] = null;
			// unset the value we have just set above
			config.Unset("test", "section", "event");
			// no changes until we save
			NUnit.Framework.Assert.IsNull(events[0]);
			config.Save();
			NUnit.Framework.Assert.IsNotNull(events[0]);
			// correct repository?
			NUnit.Framework.Assert.AreEqual(events[0].GetRepository(), db);
		}

		private sealed class _ConfigChangedListener_52 : ConfigChangedListener
		{
			public _ConfigChangedListener_52(ConfigChangedEvent[] events)
			{
				this.events = events;
			}

			public void OnConfigChanged(ConfigChangedEvent @event)
			{
				events[0] = @event;
			}

			private readonly ConfigChangedEvent[] events;
		}
	}
}
