using System.Text;
using NGit;
using Sharpen;

namespace NGit
{
	/// <summary>A simple progress reporter printing on stderr</summary>
	public class TextProgressMonitor : ProgressMonitor
	{
		private bool output;

		private long taskBeganAt;

		private string msg;

		private int lastWorked;

		private int totalWork;

		/// <summary>Initialize a new progress monitor.</summary>
		/// <remarks>Initialize a new progress monitor.</remarks>
		public TextProgressMonitor()
		{
			taskBeganAt = Runtime.CurrentTimeMillis();
		}

		public override void Start(int totalTasks)
		{
			// Ignore the number of tasks.
			taskBeganAt = Runtime.CurrentTimeMillis();
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
			if (!output && Runtime.CurrentTimeMillis() - taskBeganAt < 500)
			{
				return;
			}
			if (totalWork == UNKNOWN)
			{
				Display(cmp);
				System.Console.Error.Flush();
			}
			else
			{
				if ((cmp * 100 / totalWork) != (lastWorked * 100) / totalWork)
				{
					Display(cmp);
					System.Console.Error.Flush();
				}
			}
			lastWorked = cmp;
			output = true;
		}

		private void Display(int cmp)
		{
			StringBuilder m = new StringBuilder();
			m.Append('\r');
			m.Append(msg);
			m.Append(": ");
			while (m.Length < 25)
			{
				m.Append(' ');
			}
			if (totalWork == UNKNOWN)
			{
				m.Append(cmp);
			}
			else
			{
				string twstr = totalWork.ToString();
				string cmpstr = cmp.ToString();
				while (cmpstr.Length < twstr.Length)
				{
					cmpstr = " " + cmpstr;
				}
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
				m.Append(cmpstr);
				m.Append("/");
				m.Append(twstr);
				m.Append(")");
			}
			System.Console.Error.Write(m);
		}

		public override bool IsCancelled()
		{
			return false;
		}

		public override void EndTask()
		{
			if (output)
			{
				if (totalWork != UNKNOWN)
				{
					Display(totalWork);
				}
				System.Console.Error.WriteLine();
			}
			output = false;
			msg = null;
		}
	}
}
