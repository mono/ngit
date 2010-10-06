using Sharpen;

namespace NSch.Jce
{
	public class KeyPairGenDSA : NSch.KeyPairGenDSA
	{
		internal byte[] x;

		internal byte[] y;

		internal byte[] p;

		internal byte[] q;

		internal byte[] g;

		// private
		// public
		/// <exception cref="System.Exception"></exception>
		public virtual void Init(int key_size)
		{
			KeyPairGenerator keyGen = KeyPairGenerator.GetInstance("DSA");
			keyGen.Initialize(key_size, new SecureRandom());
			Sharpen.KeyPair pair = keyGen.GenerateKeyPair();
			PublicKey pubKey = pair.GetPublic();
			PrivateKey prvKey = pair.GetPrivate();
			x = ((DSAPrivateKey)prvKey).GetX().GetBytes();
			y = ((DSAPublicKey)pubKey).GetY().GetBytes();
			DSAParams @params = ((DSAKey)prvKey).GetParams();
			p = @params.GetP().GetBytes();
			q = @params.GetQ().GetBytes();
			g = @params.GetG().GetBytes();
		}

		public virtual byte[] GetX()
		{
			return x;
		}

		public virtual byte[] GetY()
		{
			return y;
		}

		public virtual byte[] GetP()
		{
			return p;
		}

		public virtual byte[] GetQ()
		{
			return q;
		}

		public virtual byte[] GetG()
		{
			return g;
		}
	}
}
