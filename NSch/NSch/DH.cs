using Sharpen;

namespace NSch
{
	public interface DH
	{
		/// <exception cref="System.Exception"></exception>
		void Init();

		void SetP(byte[] p);

		void SetG(byte[] g);

		/// <exception cref="System.Exception"></exception>
		byte[] GetE();

		void SetF(byte[] f);

		/// <exception cref="System.Exception"></exception>
		byte[] GetK();
	}
}
