using System.Text;
using NGit;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>Write progress messages out to the sideband channel.</summary>
	/// <remarks>Write progress messages out to the sideband channel.</remarks>
	internal class SideBandProgressMonitor : ProgressMonitor
	{
		private PrintWriter @out;

		private bool output;

		private long taskBeganAt;

		private long lastOutput;

		private string msg;

		private int lastWorked;

		private int totalWork;

		internal SideBandProgressMonitor(OutputStream os)
		{
			@out = new PrintWriter(new OutputStreamWriter(os, Constants.CHARSET));
		}

		public override void Start(int totalTasks)
		{
			// Ignore the number of tasks.
			taskBeganAt = Runtime.CurrentTimeMillis();
			lastOutput = taskBeganAt;
		}

		public override void BeginTask(string title, int total)
		{
			EndTask();
			msg = title;
			lastWorked = 0;
			totalWork = total;
		}

		public override void Update(int completed)
		{
			if (msg == null)
			{
				return;
			}
			int cmp = lastWorked + completed;
			long now = Runtime.CurrentTimeMillis();
			if (!output && now - taskBeganAt < 500)
			{
				return;
			}
			if (totalWork == UNKNOWN)
			{
				if (now - lastOutput >= 500)
				{
					Display(cmp, null);
					lastOutput = now;
				}
			}
			else
			{
				if ((cmp * 100 / totalWork) != (lastWorked * 100) / totalWork || now - lastOutput
					 >= 500)
				{
					Display(cmp, null);
					lastOutput = now;
				}
			}
			lastWorked = cmp;
			output = true;
		}

		private void Display(int cmp, string eol)
		{
			StringBuilder m = new StringBuilder();
			m.Append(msg);
			m.Append(": ");
			if (totalWork == UNKNOWN)
			{
				m.Append(cmp);
			}
			else
			{
				int pcnt = (cmp * 100 / totalWork);
				if (pcnt < 100)
				{
					m.Append(' ');
				}
				if (pcnt < 10)
				{
					m.Append(' ');
				}
				m.Append(pcnt);
				m.Append("% (");
				m.Append(cmp);
				m.Append("/");
				m.Append(totalWork);
				m.Append(")");
			}
			if (eol != null)
			{
				m.Append(eol);
			}
			else
			{
				m.Append("   \r");
			}
			@out.Write(m);
			@out.Flush();
		}

		public override bool IsCancelled()
		{
			return false;
		}

		public override void EndTask()
		{
			if (output)
			{
				if (totalWork == UNKNOWN)
				{
					Display(lastWorked, ", done\n");
				}
				else
				{
					Display(totalWork, "\n");
				}
			}
			output = false;
			msg = null;
		}
	}
}
