using System.Text;

namespace CTP
{
    public class Settings
    {
        public static class Crypto
        {
            public static Encoding encoding = Encoding.UTF8;
            public static int sizeBlock = 5;
        }
        public static class Connect
        {
            public static string Name = "mda";
            public static int connectTimeout = 100;
            public static int maxQueueSize = 100;
            public static int checkMessageDelay = 10;
        }

        public static class Main
        {
            public static string pathToSave = "C:\\";
        }
    }
}