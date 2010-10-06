using Mono.Math;
using Sharpen;

namespace NSch.Jce
{
	public class DH : NSch.DH
	{
		internal BigInteger p;

		internal BigInteger g;

		internal BigInteger e;

		internal byte[] e_array;

		internal BigInteger f;

		internal BigInteger K;

		internal byte[] K_array;

		private KeyPairGenerator myKpairGen;

		private KeyAgreement myKeyAgree;

		// my public key
		// your public key
		// shared secret key
		/// <exception cref="System.Exception"></exception>
		public virtual void Init()
		{
			myKpairGen = KeyPairGenerator.GetInstance("DH");
			//    myKpairGen=KeyPairGenerator.getInstance("DiffieHellman");
			myKeyAgree = KeyAgreement.GetInstance("DH");
		}

		//    myKeyAgree=KeyAgreement.getInstance("DiffieHellman");
		/// <exception cref="System.Exception"></exception>
		public virtual byte[] GetE()
		{
			if (e == null)
			{
				DHParameterSpec dhSkipParamSpec = new DHParameterSpec(p, g);
				myKpairGen.Initialize(dhSkipParamSpec);
				Sharpen.KeyPair myKpair = myKpairGen.GenerateKeyPair();
				myKeyAgree.Init(myKpair.GetPrivate());
				//    BigInteger x=((javax.crypto.interfaces.DHPrivateKey)(myKpair.getPrivate())).getX();
				e = ((DHPublicKey)(myKpair.GetPublic())).GetY();
				e_array = e.GetBytes();
			}
			return e_array;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual byte[] GetK()
		{
			if (K == null)
			{
				KeyFactory myKeyFac = KeyFactory.GetInstance("DH");
				DHPublicKeySpec keySpec = new DHPublicKeySpec(f, p, g);
				PublicKey yourPubKey = myKeyFac.GeneratePublic(keySpec);
				myKeyAgree.DoPhase(yourPubKey, true);
				byte[] mySharedSecret = myKeyAgree.GenerateSecret();
				K = new BigInteger(mySharedSecret);
				K_array = K.GetBytes();
				//System.err.println("K.signum(): "+K.signum()+
				//		   " "+Integer.toHexString(mySharedSecret[0]&0xff)+
				//		   " "+Integer.toHexString(K_array[0]&0xff));
				K_array = mySharedSecret;
			}
			return K_array;
		}

		public virtual void SetP(byte[] p)
		{
			SetP(new BigInteger(p));
		}

		public virtual void SetG(byte[] g)
		{
			SetG(new BigInteger(g));
		}

		public virtual void SetF(byte[] f)
		{
			SetF(new BigInteger(f));
		}

		internal virtual void SetP(BigInteger p)
		{
			this.p = p;
		}

		internal virtual void SetG(BigInteger g)
		{
			this.g = g;
		}

		internal virtual void SetF(BigInteger f)
		{
			this.f = f;
		}
	}
}
