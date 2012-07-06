using System;
using NUnit.Framework;
using Sharpen;
using System.Threading;

namespace Sharpen.Test
{
	[TestFixture()]
	public class ScheduledThreadPoolExecutorTests
	{
		const int delayDif = 50;

		[Test]
		public void ExecuteTest ()
		{
			ThreadPoolExecutor executor = new ThreadPoolExecutor (10, Executors.DefaultThreadFactory ());
			executor.Execute (new RunnableAction (delegate {
				Console.WriteLine ("Yarrrrr!");
			}));
		}

		[Test()]
		public void DelayedTask ()
		{
			ScheduledThreadPoolExecutor e = new ScheduledThreadPoolExecutor (5, Executors.DefaultThreadFactory ());
			DateTime tim = DateTime.Now;
			var future = e.Schedule (new RunnableAction (delegate {
				Console.WriteLine ("t1");
			}),50, TimeUnit.MILLISECONDS);
			future.Get ();
			double elapsed = (DateTime.Now - tim).TotalMilliseconds;
			Assert.IsTrue (elapsed >= 50, "Elapsed: " + elapsed);
			Assert.IsTrue (elapsed < 60 + delayDif, "Elapsed: " + elapsed);
			e.ShutdownNow ();
		}
		
		[Test()]
		public void InsertDelayedTask ()
		{
			ScheduledThreadPoolExecutor e = new ScheduledThreadPoolExecutor (5, Executors.DefaultThreadFactory ());
			double t1 = 0;
			double t2 = 0;
			
			DateTime tim1 = DateTime.Now;
			e.Schedule (new RunnableAction (delegate {
				t1 = (DateTime.Now - tim1).TotalMilliseconds;
			}),100, TimeUnit.MILLISECONDS);
			
			Thread.Sleep (20);
			
			DateTime tim2 = DateTime.Now;
			e.Schedule (new RunnableAction (delegate {
				t2 = (DateTime.Now - tim2).TotalMilliseconds;
			}),50, TimeUnit.MILLISECONDS);
			
			Thread.Sleep (150);
			
			Assert.IsTrue (t2 >= 50, "Elapsed: " + t2);
			Assert.IsTrue (t2 < 50 + delayDif, "Elapsed: " + t2);
			Assert.IsTrue (t1 >= 100, "Elapsed: " + t1);
			Assert.IsTrue (t1 < 100 + delayDif, "Elapsed: " + t1);
			e.ShutdownNow ();
		}
		
		[Test()]
		public void ShutdownNow ()
		{
			ScheduledThreadPoolExecutor e = new ScheduledThreadPoolExecutor (5, Executors.DefaultThreadFactory ());
			bool run = false;
			e.Schedule (new RunnableAction (delegate {
				run = true;
			}),100, TimeUnit.MILLISECONDS);
			
			Thread.Sleep (50);
			
			var pending = e.ShutdownNow ();
			Assert.AreEqual (0, pending.Count);
			Assert.IsTrue (e.IsShutdown ());
			Assert.IsTrue (e.IsTerminated (), "Terminated");
			Assert.IsFalse (e.IsTerminating (), "Terminating");
			
			Thread.Sleep (100);
			
			Assert.IsFalse (run);
			Assert.IsTrue (e.IsTerminated (), "Terminated");
			Assert.IsFalse (e.IsTerminating (), "Terminating");
		}
		
		[Test()]
		public void Shutdown ()
		{
			ScheduledThreadPoolExecutor e = new ScheduledThreadPoolExecutor (5, Executors.DefaultThreadFactory ());
			bool run = false;
			e.Schedule (new RunnableAction (delegate {
				run = true;
			}),100, TimeUnit.MILLISECONDS);
			
			Thread.Sleep (50);
			
			e.Shutdown ();
			Assert.IsTrue (e.IsShutdown ());
			Assert.IsFalse (e.IsTerminated (), "Terminated");
			Assert.IsTrue (e.IsTerminating (), "Terminating");
			
			Thread.Sleep (100);
			
			Assert.IsTrue (run, "Not run");
			Assert.IsTrue (e.IsTerminated (), "Terminated");
			Assert.IsFalse (e.IsTerminating (), "Terminating");
		}
		
		[Test()]
		public void ShutdownNoContinueExisting ()
		{
			ScheduledThreadPoolExecutor e = new ScheduledThreadPoolExecutor (5, Executors.DefaultThreadFactory ());
			e.SetExecuteExistingDelayedTasksAfterShutdownPolicy (false);
			
			bool run = false;
			e.Schedule (new RunnableAction (delegate {
				run = true;
			}),100, TimeUnit.MILLISECONDS);
			
			Thread.Sleep (50);
			
			e.Shutdown ();
			Assert.IsTrue (e.IsShutdown ());
			Assert.IsTrue (e.IsTerminated (), "Terminated");
			Assert.IsFalse (e.IsTerminating (), "Terminating");
			
			Thread.Sleep (100);
			
			Assert.IsFalse (run);
			Assert.IsTrue (e.IsTerminated (), "Terminated");
			Assert.IsFalse (e.IsTerminating (), "Terminating");
		}
	}
}
