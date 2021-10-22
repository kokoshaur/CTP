namespace Crypter
{
    public static class CryptersFactory
    {
        public enum CryptoType : byte
        {
            RSA = 1,
            GAMAL,
            XOR
        }

        public static ICrypto newCrypter(CryptoType type, int sizeBlock)
        {
            switch (type)
            {
                case CryptoType.RSA:
                    return new Crypters.RSACrypter(sizeBlock);

                case CryptoType.XOR:
                    return new Crypters.XORCrypter(sizeBlock);

                default:
                    return new Crypters.RSACrypter(sizeBlock);
            }
        }

        public static ICrypto newCrypter(CryptoType type, byte[] openkey)
        {
            switch (type)
            {
                case CryptoType.RSA:
                    return new Crypters.RSACrypter(openkey);

                case CryptoType.XOR:
                    return new Crypters.XORCrypter(openkey);

                default:
                    return new Crypters.RSACrypter(openkey);
            }
        }
    }
}