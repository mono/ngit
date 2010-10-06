using Sharpen;

namespace NSch
{
	public interface KeyPairGenDSA
	{
		/// <exception cref="System.Exception"></exception>
		void Init(int key_size);

		byte[] GetX();

		byte[] GetY();

		byte[] GetP();

		byte[] GetQ();

		byte[] GetG();
	}
}
