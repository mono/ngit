using Sharpen;

namespace NSch
{
	public interface KeyPairGenRSA
	{
		/// <exception cref="System.Exception"></exception>
		void Init(int key_size);

		byte[] GetD();

		byte[] GetE();

		byte[] GetN();

		byte[] GetC();

		byte[] GetEP();

		byte[] GetEQ();

		byte[] GetP();

		byte[] GetQ();
	}
}
