using System;
using System.IO;
using CTP;

namespace Example
{
    class FakeUser
    {
        string IP;
        public int myPort { private set; get; }
        CTPManager me;
        string name;
        int coutFrame = 0;
        public FakeUser(string ip, int portIn, string name)
        {
            IP = ip;
            myPort = portIn;
            this.name = name;
            me = new CTPManager(show, showFrame);
            start();
        }

        public void show(string message)
        {
            Console.WriteLine(name + " " + message);
        }

        public void showFrame(string message, byte[] data)
        {
            if (data[0] == 123 && data[data.Length - 1] == 123 && data.Length == 8000)
                Console.WriteLine(name + " " + "Принят новый фрейм " + data.Length + " (" + coutFrame++ + ")");
            else
                Console.WriteLine("Потеря пакета" + coutFrame++);
        }

        public void connect(int portOut)
        {
            me.addFriend(IP, portOut);
        }

        public void send(string message)
        {
            me.sendToAll(message);
        }
        public void startSendStrim(int coutRepeat)
        {
            byte[] message = new byte[2000];
            for (int i = 0; i < message.Length; i++)
                message[i] = 123;

            for (int i = 0; i < coutRepeat; i++)
                me.sendToAll(message);
        }

        public void sendFile(string path)
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            me.sendToAll(file);
        }

        public void refreshCrypto(string name, CTP.Type type)
        {
            me.refreshCrypto(name, type);
        }

        public void start()
        {
            me.openPortAsync(myPort, up);
        }

        public void stop()
        {
            me.closePortAsync(myPort, down);
        }

        private void up()
        {
            me.startListenNewFriends();
            me.startListenMessage();
        }

        private void down()
        {
            me.close();
        }
    }
}