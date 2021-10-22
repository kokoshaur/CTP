namespace Crypter.Crypters
{
    public class XORCrypter : TrevialAbstractCrypter
    {
        public XORCrypter(int sizeBlock = 64) : base(sizeBlock) { }
        public XORCrypter(byte[] key) : base(key) { }

        public override int sizeOfReadCluster => 65535;

        public override int sizeOfWriteCluster => 65535;

        public override byte[] decryptBlock(in byte[] inpute)
        {
            byte[] ansver = new byte[inpute.Length];
            for (int i = 0; i < inpute.Length; i++)
                ansver[i] = (byte)(inpute[i] ^ key[i % key.Length]);

            return ansver;
        }

        public override byte[] encryptBlock(in byte[] inpute)
        {
            byte[] ansver = new byte[inpute.Length];
            for (int i = 0; i < inpute.Length; i++)
                ansver[i] = (byte)(inpute[i] ^ key[i % key.Length]);

            return ansver;
        }
    }
}