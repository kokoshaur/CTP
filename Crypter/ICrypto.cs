namespace Crypter
{
    public interface ICrypto
    {
        public int sizeOfReadCluster { get; }
        public int sizeOfWriteCluster { get; }
        public byte[] getOpenKey();
        public byte[] decryptBlock(in byte[] inpute); //Дешифрование блока байт
        public byte[] encryptBlock(in byte[] inpute); //Шифрование блока байт
    }
}