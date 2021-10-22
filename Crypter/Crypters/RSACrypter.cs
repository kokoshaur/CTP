using System.Numerics;
using FieldMath;

/// <summary>
/// Универсальный алгоритм со средним уровнем криптостойкости. Подходит для шифрования как малых сообщений, так и больших файлов. 
/// Скорость инициализации: низкая. Скорость шифрования\дешифрования: высокая. Расходы памяти: средние. Избыточность шифротекста: низкая.
/// 
/// Несложно заметить, что при минимальном значении sizeBlock = 1 (значения меньше не имеют смысла и вызовут исключение) общий объём данных после шифрования увеличится в 1.4 раза.
/// То есть, если подать на вход массив из 10000 байт, то на выходе (в худшем случае) будет массив, длинной 14000.
/// При увеличении sizeBlock общий объём данных после шифрования будет стремиться к общему объёму данных до шифрования (но никогда не достигнет его).
/// 
/// Значения sizeBlock выше 500 не рекомендуются, ввиду изнурительно долгой инициализации ключа. Рекомендуемое значение sizeBlock = 64.
/// 
/// При вводе массива из нулей, результатом будем пустой массив (вне зависимости от длинны исходного массива).
/// 
/// Реализовано в соответствии с документацией https://ee.stanford.edu/~hellman/publications/24.pdf
/// </summary>

namespace Crypter.Crypters
{
    public class RSACrypter : BasicAbstractCrypter
    {
        BigInteger secretKey;
        int coutBytes;
        public RSACrypter(OpenKey openKey) : base(openKey)
        { coutBytes = openkey.mod.coutBytes(); }

        public RSACrypter(byte[] openKey) : base(openKey)
        { coutBytes = openkey.mod.coutBytes(); }

        public RSACrypter(int sizeBlock = 64) : base(sizeBlock)
        { coutBytes = openkey.mod.coutBytes(); }

        public override int sizeOfReadCluster => 40000;

        public override int sizeOfWriteCluster => 56000;

        public override byte[] decryptBlock(in byte[] inpute)
        {
            BigInteger[] inBigInt1 = inpute.byteToBigInt(openkey.sizeOfCluster);
            BigInteger[] ansver = new BigInteger[inBigInt1.Length];

            for (int i = 0; i < inBigInt1.Length; i++)
                ansver[i] = BigInteger.ModPow(inBigInt1[i], secretKey, openkey.mod);

            return ansver.bigIntToByte(coutBytes, true).deleteNul();
        }

        public override byte[] encryptBlock(in byte[] inpute)
        {
            BigInteger[] inBigInt = inpute.byteToBigInt(coutBytes, true);
            BigInteger[] ansver = new BigInteger[inBigInt.Length];

            for (int i = 0; i < inBigInt.Length; i++)
                ansver[i] = BigInteger.ModPow(inBigInt[i], openkey.e, openkey.mod);

            return ansver.bigIntToByte(openkey.sizeOfCluster);
        }

        public override void generateKey(int sizeBlock)
        {
            openkey = new OpenKey(sizeBlock);

            BigInteger p = Math.bigRandomTrevial(Math.vanilaPow(256, openkey.sizeOfCluster));
            BigInteger q = Math.bigRandomTrevial(Math.vanilaPow(256, openkey.sizeOfCluster));

            openkey.n = p * q;
            BigInteger f = (p - 1) * (q - 1);

            do
                openkey.e = Math.bigRandomTrevial(100);
            while (openkey.e >= f);

            openkey.mod = openkey.n;

            secretKey = Math.nodByEvclide(openkey.e, f);

            openkey.sizeOfCluster = openkey.mod.ToByteArray().Length;
        }

    }
}