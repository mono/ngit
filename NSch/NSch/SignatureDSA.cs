using Sharpen;

namespace NSch
{
	public interface SignatureDSA
	{
		/// <exception cref="System.Exception"></exception>
		void Init();

		/// <exception cref="System.Exception"></exception>
		void SetPubKey(byte[] y, byte[] p, byte[] q, byte[] g);

		/// <exception cref="System.Exception"></exception>
		void SetPrvKey(byte[] x, byte[] p, byte[] q, byte[] g);

		/// <exception cref="System.Exception"></exception>
		void Update(byte[] H);

		/// <exception cref="System.Exception"></exception>
		bool Verify(byte[] sig);

		/// <exception cref="System.Exception"></exception>
		byte[] Sign();
	}
}
