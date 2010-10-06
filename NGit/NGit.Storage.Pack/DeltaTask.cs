using NGit;
using NGit.Storage.Pack;
using Sharpen;

namespace NGit.Storage.Pack
{
	internal sealed class DeltaTask : Callable<object>
	{
		private readonly PackConfig config;

		private readonly ObjectReader templateReader;

		private readonly DeltaCache dc;

		private readonly ProgressMonitor pm;

		private readonly int batchSize;

		private readonly int start;

		private readonly ObjectToPack[] list;

		internal DeltaTask(PackConfig config, ObjectReader reader, DeltaCache dc, ProgressMonitor
			 pm, int batchSize, int start, ObjectToPack[] list)
		{
			this.config = config;
			this.templateReader = reader;
			this.dc = dc;
			this.pm = pm;
			this.batchSize = batchSize;
			this.start = start;
			this.list = list;
		}

		/// <exception cref="System.Exception"></exception>
		public object Call()
		{
			ObjectReader or = templateReader.NewReader();
			try
			{
				DeltaWindow dw;
				dw = new DeltaWindow(config, dc, or);
				dw.Search(pm, list, start, batchSize);
			}
			finally
			{
				or.Release();
			}
			return null;
		}
	}
}
