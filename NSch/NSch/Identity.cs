using NSch;
using Sharpen;

namespace NSch
{
	public interface Identity
	{
		/// <exception cref="NSch.JSchException"></exception>
		bool SetPassphrase(byte[] passphrase);

		byte[] GetPublicKeyBlob();

		byte[] GetSignature(byte[] data);

		bool Decrypt();

		string GetAlgName();

		string GetName();

		bool IsEncrypted();

		void Clear();
	}
}
