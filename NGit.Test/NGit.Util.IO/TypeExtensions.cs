using System;
using System.IO;
using System.Text;
using NGit.Util.IO;
using Sharpen;

namespace NGit.Test.NGit.Util.IO
{
    public static class TypeExtensions
    {
        public static InputStream GetResourceAsStream(this Type type, string name, bool useCrlf = false)
        {
            string str2 = type.Assembly.GetName().Name + ".resources";
            string[] textArray1 = new string[] { str2, ".", type.Namespace, ".", name };
            string str = string.Concat(textArray1);
            Stream manifestResourceStream = type.Assembly.GetManifestResourceStream(str);
            if (manifestResourceStream == null)
            {
                return null;
            }

            return useCrlf
                ? UseCrlf(manifestResourceStream)
                : new EolCanonicalizingInputStream(InputStream.Wrap(manifestResourceStream), false);
        }

        private static Stream UseCrlf(Stream inputStream)
        {
            using (inputStream)
            {
                var newStream = new MemoryStream();
                byte thisByte;
                for (byte? previousByte = null; inputStream.Position < inputStream.Length; previousByte = thisByte)
                {
                    thisByte = (byte) inputStream.ReadByte();

                    if (previousByte != '\r' && thisByte == '\n')
                    {
                        newStream.WriteByte((byte) '\r');
                    }
                    newStream.WriteByte(thisByte);
                }
                newStream.Position = 0;
                return newStream;
            }
        }
    }
}
