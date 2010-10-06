using Sharpen;

namespace NSch
{
	public interface SignatureRSA
	{
		/// <exception cref="System.Exception"></exception>
		void Init();

		/// <exception cref="System.Exception"></exception>
		void SetPubKey(byte[] e, byte[] n);

		/// <exception cref="System.Exception"></exception>
		void SetPrvKey(byte[] d, byte[] n);

		/// <exception cref="System.Exception"></exception>
		void Update(byte[] H);

		/// <exception cref="System.Exception"></exception>
		bool Verify(byte[] sig);

		/// <exception cref="System.Exception"></exception>
		byte[] Sign();
	}
}
