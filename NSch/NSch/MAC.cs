using NSch;
using Sharpen;

namespace NSch
{
	public interface MAC
	{
		string GetName();

		int GetBlockSize();

		/// <exception cref="System.Exception"></exception>
		void Init(byte[] key);

		void Update(byte[] foo, int start, int len);

		void Update(int foo);

		void DoFinal(byte[] buf, int offset);
	}
}
