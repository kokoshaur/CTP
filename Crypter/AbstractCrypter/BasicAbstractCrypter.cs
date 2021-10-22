namespace Crypter
{
    public abstract class BasicAbstractCrypter : ICrypto
    {
        public OpenKey openkey { get; protected set; }
        public abstract int sizeOfReadCluster { get; }
        public abstract int sizeOfWriteCluster { get; }

        public BasicAbstractCrypter(OpenKey openKey)
        {
            openkey = openKey;
        }

        public BasicAbstractCrypter(byte[] openKey)
        {
            openkey = new OpenKey(openKey);
        }
        public BasicAbstractCrypter(int sizeBlock = 64) //sizeBlock - колличество байт, из которых бует сформировано число.
        {                                                                     //У обоих пользователей оно должно быть одинаковым в одной сессии.                                         //При незначительном увеличении ЗНАЧИТЕЛЬНО падает скорость шифрования.
            generateKey(sizeBlock);
        }


        public abstract byte[] decryptBlock(in byte[] inpute);
        public abstract byte[] encryptBlock(in byte[] inpute);
        public abstract void generateKey(int sizeBlock);
        public byte[] getOpenKey()
        {
            return openkey.toByte();
        }
    }
}