using System;
using System.Numerics;

namespace Crypter
{
    public class OpenKey
    {
        public int sizeOfCluster { get; set; }

        public BigInteger n { get; set; }
        public BigInteger e { get; set; }
        public BigInteger mod { get; set; }

        public OpenKey(int sizeBlock)
        {
            sizeOfCluster = sizeBlock;
        }

        public OpenKey(byte[] openkey)
        {
            sizeOfCluster = openkey[0];

            byte[] N = new byte[sizeOfCluster];
            Array.Copy(openkey, 1, N, 0, N.Length);

            byte[] E = new byte[openkey.Length - N.Length - 1];
            Array.Copy(openkey, N.Length + 1, E, 0, E.Length);

            n = new BigInteger(N);
            e = new BigInteger(E);
            mod = n;
        }

        public byte[] toByte()
        {
            byte[] N = n.ToByteArray();
            byte[] E = e.ToByteArray();

            byte[] ansver = new byte[N.Length + E.Length + 1];
            ansver[0] = (byte)sizeOfCluster;

            Array.Copy(N, 0, ansver, 1, N.Length);

            Array.Copy(E, 0, ansver, N.Length + 1, E.Length);

            return ansver;
        }
    }
}