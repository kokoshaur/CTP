namespace Crypter
{
    public abstract class TrevialAbstractCrypter : ICrypto
    {
        public byte[] key { get; protected set; }
        public abstract int sizeOfReadCluster { get; }
        public abstract int sizeOfWriteCluster { get; }

        public TrevialAbstractCrypter(int sizeBlock = 64) //sizeBlock - колличество байт, из которых бует сформировано число.
        {                                                                     //У обоих пользователей оно должно быть одинаковым в одной сессии.                                         //При незначительном увеличении ЗНАЧИТЕЛЬНО падает скорость шифрования.
            key = new byte[sizeBlock];
            System.Random rand = new System.Random();
            rand.NextBytes(key);
        }
        public TrevialAbstractCrypter(byte[] key)
        {
            this.key = key;
        }

        public abstract byte[] decryptBlock(in byte[] inpute);
        public abstract byte[] encryptBlock(in byte[] inpute);
        public byte[] getOpenKey()
        {
            return key;
        }
    }
}