using CTP;
using System;

namespace Example
{
    class Program
    {
        static string IP = "Ur IP";    //В примере оба ip - это внешний ip данной машины
        static void Main(string[] args)
        {
            Console.WriteLine("Проверка открытия портов");
            Settings.Connect.Name = "user1";
            FakeUser user1 = new FakeUser(IP, 8888, "1");
            Console.ReadLine();
            Settings.Connect.Name = "user2";
            FakeUser user2 = new FakeUser(IP, 7777, "2");
            Console.ReadLine();

            Console.WriteLine("Проверка подключения");
            user1.connect(user2.myPort);
            Console.ReadLine();

            Console.WriteLine("Проверка отправки текстовых сообщений");
            user1.send("Hello, world!");
            Console.ReadLine();

            Console.WriteLine("Проверка смены режима шифрования");
            user1.refreshCrypto("user2", CTP.Type.XOR);
            Console.ReadLine();
            user1.send("Hello, world!");
            Console.ReadLine();
            user2.send("Hello, world!");
            Console.ReadLine();

            Console.WriteLine("Проверка отправки файла");
            Settings.Main.pathToSave = "C:\\";
            user1.sendFile("C:\\Users\\admin\\Desktop\\2.png");
            Console.ReadLine();

            Console.WriteLine("Проверка отправки файла с тяжеловесными шифрованиями");
            user1.refreshCrypto("user2", CTP.Type.RSA);
            Console.ReadLine();
            Settings.Main.pathToSave = "C:\\";
            user1.sendFile("D:\\Subjs\\1.png");
            Console.ReadLine();

            Console.WriteLine("Проверка отправки непрерывного потока данных (видео)");
            user2.refreshCrypto("user1", CTP.Type.XOR);
            Console.ReadLine();
            user1.refreshCrypto("user2", CTP.Type.XOR);
            Console.ReadLine();
            user1.startSendStrim(20);
            Console.WriteLine("Отправлено 100 пакетов");
            Console.ReadLine();

            Console.WriteLine("Проверка закрытия портов");
            user1.stop();
            user2.stop();
        }
    }
}