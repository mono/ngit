using Sharpen;

namespace NSch.Jce
{
	public class Random : NSch.Random
	{
		private byte[] tmp = new byte[16];

		private SecureRandom random = null;

		public Random()
		{
			// We hope that 'new SecureRandom()' will use NativePRNG algorithm
			// on Sun's Java5 for GNU/Linux and Solaris.
			// It seems NativePRNG refers to /dev/urandom and it must not be blocked,
			// but NativePRNG is slower than SHA1PRNG ;-<
			// TIPS: By adding option '-Djava.security.egd=file:/dev/./urandom'
			//       SHA1PRNG will be used instead of NativePRNG.
			// On MacOSX, 'new SecureRandom()' will use NativePRNG algorithm and
			// it is also slower than SHA1PRNG.
			// On Windows, 'new SecureRandom()' will use SHA1PRNG algorithm.
			random = new SecureRandom();
		}

		public virtual void Fill(byte[] foo, int start, int len)
		{
			if (len > tmp.Length)
			{
				tmp = new byte[len];
			}
			random.NextBytes(tmp);
			System.Array.Copy(tmp, 0, foo, start, len);
		}
	}
}
