using Mono.Math;
using Sharpen;

namespace NSch.Jce
{
	public class SignatureRSA : NSch.SignatureRSA
	{
		internal Signature signature;

		internal KeyFactory keyFactory;

		/// <exception cref="System.Exception"></exception>
		public virtual void Init()
		{
			signature = Signature.GetInstance("SHA1withRSA");
			keyFactory = KeyFactory.GetInstance("RSA");
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void SetPubKey(byte[] e, byte[] n)
		{
			RSAPublicKeySpec rsaPubKeySpec = new RSAPublicKeySpec(new BigInteger(n), new BigInteger
				(e));
			PublicKey pubKey = keyFactory.GeneratePublic(rsaPubKeySpec);
			signature.InitVerify(pubKey);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void SetPrvKey(byte[] d, byte[] n)
		{
			RSAPrivateKeySpec rsaPrivKeySpec = new RSAPrivateKeySpec(new BigInteger(n), new BigInteger
				(d));
			PrivateKey prvKey = keyFactory.GeneratePrivate(rsaPrivKeySpec);
			signature.InitSign(prvKey);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual byte[] Sign()
		{
			byte[] sig = signature.Sign();
			return sig;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Update(byte[] foo)
		{
			signature.Update(foo);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual bool Verify(byte[] sig)
		{
			int i = 0;
			int j = 0;
			byte[] tmp;
			if (sig[0] == 0 && sig[1] == 0 && sig[2] == 0)
			{
				j = ((sig[i++] << 24) & unchecked((int)(0xff000000))) | ((sig[i++] << 16) & unchecked(
					(int)(0x00ff0000))) | ((sig[i++] << 8) & unchecked((int)(0x0000ff00))) | ((sig[i
					++]) & unchecked((int)(0x000000ff)));
				i += j;
				j = ((sig[i++] << 24) & unchecked((int)(0xff000000))) | ((sig[i++] << 16) & unchecked(
					(int)(0x00ff0000))) | ((sig[i++] << 8) & unchecked((int)(0x0000ff00))) | ((sig[i
					++]) & unchecked((int)(0x000000ff)));
				tmp = new byte[j];
				System.Array.Copy(sig, i, tmp, 0, j);
				sig = tmp;
			}
			//System.err.println("j="+j+" "+Integer.toHexString(sig[0]&0xff));
			return signature.Verify(sig);
		}
	}
}
