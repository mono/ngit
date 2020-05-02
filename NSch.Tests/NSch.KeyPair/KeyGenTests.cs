using System;
using System.IO;
using System.Text;

namespace NSch.Tests.NSch.KeyPair
{
    [NUnit.Framework.TestFixture]
    public class KeyGenTests
    {
        [NUnit.Framework.Test]
        public void KeyPairGenerate_RSA1024_GeneratesKeys()
        {
            var jsch = new global::NSch.JSch();
            var keyPair = global::NSch.KeyPair.GenKeyPair(jsch, global::NSch.KeyPair.RSA, 1024);

            string privateKey;
            using (var keyStream = new MemoryStream())
            {
                keyPair.WritePrivateKey(keyStream);
                privateKey = Encoding.UTF8.GetString(keyStream.ToArray());
            }
            string publicKey;
            using (var keyStream = new MemoryStream())
            {
                keyPair.WritePublicKey(keyStream, "Test Comment");
                publicKey = Encoding.UTF8.GetString(keyStream.ToArray());
            }
            NUnit.Framework.Assert.IsTrue(!String.IsNullOrEmpty(privateKey));
            NUnit.Framework.Assert.IsTrue(!String.IsNullOrEmpty(publicKey));
        }
    }
}
