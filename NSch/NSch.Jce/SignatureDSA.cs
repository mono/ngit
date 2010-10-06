using Mono.Math;
using Sharpen;

namespace NSch.Jce
{
	public class SignatureDSA : NSch.SignatureDSA
	{
		internal Signature signature;

		internal KeyFactory keyFactory;

		/// <exception cref="System.Exception"></exception>
		public virtual void Init()
		{
			signature = Signature.GetInstance("SHA1withDSA");
			keyFactory = KeyFactory.GetInstance("DSA");
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void SetPubKey(byte[] y, byte[] p, byte[] q, byte[] g)
		{
			DSAPublicKeySpec dsaPubKeySpec = new DSAPublicKeySpec(new BigInteger(y), new BigInteger
				(p), new BigInteger(q), new BigInteger(g));
			PublicKey pubKey = keyFactory.GeneratePublic(dsaPubKeySpec);
			signature.InitVerify(pubKey);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void SetPrvKey(byte[] x, byte[] p, byte[] q, byte[] g)
		{
			DSAPrivateKeySpec dsaPrivKeySpec = new DSAPrivateKeySpec(new BigInteger(x), new BigInteger
				(p), new BigInteger(q), new BigInteger(g));
			PrivateKey prvKey = keyFactory.GeneratePrivate(dsaPrivKeySpec);
			signature.InitSign(prvKey);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual byte[] Sign()
		{
			byte[] sig = signature.Sign();
			// sig is in ASN.1
			// SEQUENCE::={ r INTEGER, s INTEGER }
			int len = 0;
			int index = 3;
			len = sig[index++] & unchecked((int)(0xff));
			//System.err.println("! len="+len);
			byte[] r = new byte[len];
			System.Array.Copy(sig, index, r, 0, r.Length);
			index = index + len + 1;
			len = sig[index++] & unchecked((int)(0xff));
			//System.err.println("!! len="+len);
			byte[] s = new byte[len];
			System.Array.Copy(sig, index, s, 0, s.Length);
			byte[] result = new byte[40];
			// result must be 40 bytes, but length of r and s may not be 20 bytes  
			System.Array.Copy(r, (r.Length > 20) ? 1 : 0, result, (r.Length > 20) ? 0 : 20 - 
				r.Length, (r.Length > 20) ? 20 : r.Length);
			System.Array.Copy(s, (s.Length > 20) ? 1 : 0, result, (s.Length > 20) ? 20 : 40 -
				 s.Length, (s.Length > 20) ? 20 : s.Length);
			//  System.arraycopy(sig, (sig[3]==20?4:5), result, 0, 20);
			//  System.arraycopy(sig, sig.length-20, result, 20, 20);
			return result;
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
			// ASN.1
			int frst = ((sig[0] & unchecked((int)(0x80))) != 0 ? 1 : 0);
			int scnd = ((sig[20] & unchecked((int)(0x80))) != 0 ? 1 : 0);
			//System.err.println("frst: "+frst+", scnd: "+scnd);
			int length = sig.Length + 6 + frst + scnd;
			tmp = new byte[length];
			tmp[0] = unchecked((byte)unchecked((int)(0x30)));
			tmp[1] = unchecked((byte)unchecked((int)(0x2c)));
			tmp[1] += (byte)frst;
			tmp[1] += (byte)scnd;
			tmp[2] = unchecked((byte)unchecked((int)(0x02)));
			tmp[3] = unchecked((byte)unchecked((int)(0x14)));
			tmp[3] += (byte)frst;
			System.Array.Copy(sig, 0, tmp, 4 + frst, 20);
			tmp[4 + tmp[3]] = unchecked((byte)unchecked((int)(0x02)));
			tmp[5 + tmp[3]] = unchecked((byte)unchecked((int)(0x14)));
			tmp[5 + tmp[3]] += (byte)scnd;
			System.Array.Copy(sig, 20, tmp, 6 + tmp[3] + scnd, 20);
			sig = tmp;
			return signature.Verify(sig);
		}
	}
}
