using NSch;
using Sharpen;

namespace NSch
{
	public interface ForwardedTCPIPDaemon : Runnable
	{
		void SetChannel(ChannelForwardedTCPIP channel, InputStream @in, OutputStream @out
			);

		void SetArg(object[] arg);
	}
}
