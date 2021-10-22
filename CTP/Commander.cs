using System;

namespace CTP
{
    public enum Command : byte
    {
        NewMessage = 1,
        NewFile,
        NewFrame,
        NewCrypt
    }

    public enum Errors : byte
    {
        succes = 1,
        mispacket
    }
    public static class Commander
    {
        public static byte[] generateCommandNewFile(string fileName, int coutRepeat)
        {
            byte[] size = BitConverter.GetBytes(coutRepeat);
            byte[] ansver = Settings.Crypto.encoding.GetBytes(Settings.Crypto.encoding.GetString(new byte[] { 1, 1, 1, 1, 1 }) + fileName);

            ansver[0] = (byte)Command.NewFile;
            ansver[1] = size[0]; ansver[2] = size[1]; ansver[3] = size[2]; ansver[4] = size[3];

            return ansver;
        }

        public static byte[] generateCommandNewCrypt(byte typeOfCrypt, in byte[] key)
        {
            byte[] ansver = new byte[2 + key.Length];
            ansver[0] = (byte)Command.NewCrypt;
            ansver[1] = typeOfCrypt;
            for (int i = 2; i < ansver.Length; i++)
                ansver[i] = key[i - 2];

            return ansver;
        }

        public static void waitSynchronization()
        {
            System.Threading.Thread.Sleep(15);
        }
    }
}