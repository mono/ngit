using Sharpen;

namespace NSch.Jce
{
	public class KeyPairGenRSA : NSch.KeyPairGenRSA
	{
		internal byte[] d;

		internal byte[] e;

		internal byte[] n;

		internal byte[] c;

		internal byte[] ep;

		internal byte[] eq;

		internal byte[] p;

		internal byte[] q;

		// private
		// public
		//  coefficient
		// exponent p
		// exponent q
		// prime p
		// prime q
		/// <exception cref="System.Exception"></exception>
		public virtual void Init(int key_size)
		{
			KeyPairGenerator keyGen = KeyPairGenerator.GetInstance("RSA");
			keyGen.Initialize(key_size, new SecureRandom());
			Sharpen.KeyPair pair = keyGen.GenerateKeyPair();
			PublicKey pubKey = pair.GetPublic();
			PrivateKey prvKey = pair.GetPrivate();
			d = ((RSAPrivateKey)prvKey).GetPrivateExponent().GetBytes();
			e = ((RSAPublicKey)pubKey).GetPublicExponent().GetBytes();
			n = ((RSAPrivateKey)prvKey).GetModulus().GetBytes();
			c = ((RSAPrivateCrtKey)prvKey).GetCrtCoefficient().GetBytes();
			ep = ((RSAPrivateCrtKey)prvKey).GetPrimeExponentP().GetBytes();
			eq = ((RSAPrivateCrtKey)prvKey).GetPrimeExponentQ().GetBytes();
			p = ((RSAPrivateCrtKey)prvKey).GetPrimeP().GetBytes();
			q = ((RSAPrivateCrtKey)prvKey).GetPrimeQ().GetBytes();
		}

		public virtual byte[] GetD()
		{
			return d;
		}

		public virtual byte[] GetE()
		{
			return e;
		}

		public virtual byte[] GetN()
		{
			return n;
		}

		public virtual byte[] GetC()
		{
			return c;
		}

		public virtual byte[] GetEP()
		{
			return ep;
		}

		public virtual byte[] GetEQ()
		{
			return eq;
		}

		public virtual byte[] GetP()
		{
			return p;
		}

		public virtual byte[] GetQ()
		{
			return q;
		}
	}
}
