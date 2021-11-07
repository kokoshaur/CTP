using Crypter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CTP
{
    class Friend
    {
        protected internal string Id { get; private set; }
        ICrypto crypterFrom;
        ICrypto crypterTo;
        public string name { get; private set; }
        TcpClient socket;
        protected internal NetworkStream Stream { get; private set; }

        Queue<Tuple<byte, byte[]>> sendQueue = new Queue<Tuple<byte, byte[]>>();
        bool isStopTransmit = false;
        Task startSendQueue;

        public Friend(TcpClient socket)
        {
            Id = Guid.NewGuid().ToString();
            this.socket = socket;
            startSendQueue = new Task(newStartSendQueue);
            startSendQueue.Start();
        }

        public void connectTo(List<Friend> friends, Type type)
        {
            Stream = socket.GetStream();

            Task.Run(() =>
            {
                createCryptoTo(type);
                createCryptoFrom();

                friends.Add(this);
            });
        }

        public void connectFrom(List<Friend> friends, Action<string> show)
        {
            Stream = socket.GetStream();

            Task.Run(() =>
            {
                Type type = createCryptoFrom();
                createCryptoTo(type);

                friends.Add(this);
                show(name + " " + Language.Connect.connect);
            });
        }

        public void send(string message)
        {
            addToQueue(Command.NewMessage, Settings.Crypto.encoding.GetBytes(message));
        }

        public void send(byte[] message)
        {
            addToQueue(Command.NewFrame, message);
        }

        public void send(FileStream file)
        {
            isStopTransmit = true;
            if (startSendQueue.IsCompleted)
                startSendQueue.Wait();

            int coutRepeat = (int)Math.Ceiling((double)file.Length / (double)crypterTo.sizeOfReadCluster);
            Stream.Write(Commander.generateCommandNewFile(Path.GetFileName(file.Name), coutRepeat));
            if (Stream.ReadByte() != (byte)Errors.succes)
                return;

            BinaryReader reader = new BinaryReader(file);
            byte[] data = new byte[crypterTo.sizeOfWriteCluster];
            for (int i = 0; i < coutRepeat; i++)
            {
                data = reader.ReadBytes(crypterTo.sizeOfReadCluster);
                data = crypterTo.encryptBlock(data);
                Stream.Write(data);
                if (Stream.ReadByte() != (byte)Errors.succes)
                    return;
            }

            reader.Close();
            file.Close();

            isStopTransmit = false;
        }

        public void refreshCrypto(byte type)
        {
            pauseTransmit();

            Stream.Write(new byte[] { (byte)Command.NewCrypt, 0 });
            Stream.Flush();
            refreshCryptoTo((Type)type);
            refreshCryptoFrom();

            continueTransmit();
        }

        public bool getMessage(ref Command command, ref string report, ref byte[] data)
        {
            if (Stream.DataAvailable)
            {
                data = readMessage(out command);
                report = parseMessage(command, ref data);
                return true;
            }
            else
                return false;
        }

        private void refreshCryptoTo(Type type)
        {
            Stream.WriteByte((byte)type);
            Stream.Flush();
            byte[] data = new byte[4096];
            int bytes = Stream.Read(data, 0, data.Length);

            byte[] openkey = new byte[bytes];
            Array.Copy(data, openkey, bytes);

            crypterTo = CryptersFactory.newCrypter((CryptersFactory.CryptoType)type, crypterFrom.decryptBlock(openkey));
        }

        private Type refreshCryptoFrom()
        {
            CryptersFactory.CryptoType type = (CryptersFactory.CryptoType)Stream.ReadByte();
            crypterFrom = CryptersFactory.newCrypter(type, Settings.Crypto.sizeBlock);

            Stream.Write(crypterTo.encryptBlock(crypterFrom.getOpenKey()));
            Stream.Flush();

            return (Type)type;
        }

        private void addToQueue(Command command, in byte[] message)
        {
            sendQueue.Enqueue(new Tuple<byte, byte[]>((byte)command, crypterTo.encryptBlock(message)));
        }

        private void pauseTransmit()
        {
            isStopTransmit = true;
            while(sendQueue.Count != 0)
                Commander.sleepForOptimization();
            return;
        }

        private void continueTransmit()
        {
            isStopTransmit = false;
        }

        private void newStartSendQueue()
        {
            while (true)
            {
                if (sendQueue.Count != 0 && !isStopTransmit)
                {
                    Tuple<byte, byte[]> message = sendQueue.Peek();
                    Stream.WriteByte(message.Item1);
                    Stream.Write(message.Item2);
                    Stream.Flush();
                    if (Stream.ReadByte() == (byte)Errors.succes)
                        sendQueue.Dequeue();
                }
                Commander.sleepForOptimization();
            }
        }

        private string parseMessage(Command command, ref byte[] data)
        {
            switch (command)
            {
                case Command.NewMessage:
                    acceptPacket();
                    return name + ":" + Settings.Crypto.encoding.GetString(crypterFrom.decryptBlock(data));

                case Command.NewFile:
                    int accessory = BitConverter.ToInt32(new byte[] { data[0], data[1], data[2], data[3] });
                    string fileName = Settings.Crypto.encoding.GetString(data, 4, data.Length - 4);
                    acceptPacket();
                    saveFile(fileName, accessory);

                    return name + ": " + Language.Connect.newFile + " " + Settings.Crypto.encoding.GetString(data, 4, data.Length - 4);

                case Command.NewFrame:
                    data = crypterFrom.decryptBlock(data);
                    acceptPacket();
                    return Language.Connect.accept + ": " + (data.Length - 1) + Language.Connect.bytes;

                case Command.NewCrypt:
                    pauseTransmit();
                    Type type = refreshCryptoFrom();
                    refreshCryptoTo(type);
                    continueTransmit();
                    return Language.Connect.updateCrypter + (CryptersFactory.CryptoType)type;

                default:
                    acceptPacket();
                    return name + ":" + Language.Connect.lostPacket;
            }

        }

        private void acceptPacket()
        {
            Stream.WriteByte((byte)Errors.succes);
            Stream.Flush();
        }

        private void saveFile(string name, int coutRepeat)
        {
            pauseTransmit();

            FileStream file = new FileStream(Settings.Main.pathToSave + name, FileMode.Create, FileAccess.Write, FileShare.Write);
            BinaryWriter writer = new BinaryWriter(file);

            Commander.waitSynchronization();

            byte[] data = new byte[crypterFrom.sizeOfWriteCluster];
            int bytes = 0;
            for (int i = 0; i < coutRepeat; i++)
            {
                bytes = Stream.Read(data, 0, data.Length);
                if (bytes < data.Length)
                {
                    byte[] crypto = new byte[bytes];
                    Array.Copy(data, crypto, bytes);
                    crypto = crypterFrom.decryptBlock(crypto);
                    writer.Write(crypto);
                }
                else
                {
                    data = crypterFrom.decryptBlock(data);
                    writer.Write(data);
                }
                Stream.WriteByte((byte)Errors.succes);
            }

            writer.Flush();
            writer.Close();
            file.Close();

            continueTransmit();
        }

        public void disconnect()
        {
            socket.Close();
        }

        private byte[] readMessage(out Command command, int sizeOfBuffer = 2048, int maxSize = 9999999)
        {
            Commander.waitSynchronization();
            byte[] data;
            List<byte[]> crypto = new List<byte[]>();
            int bytes = 0; int size = 0;
            command = (Command)Stream.ReadByte();
            do
            {
                data = new byte[sizeOfBuffer];
                bytes = Stream.Read(data, 0, data.Length);
                crypto.Add(data);
                size++;
            }
            while (Stream.DataAvailable && (size < maxSize));

            crypto[crypto.Count - 1] = new byte[bytes];
            Array.Copy(data, crypto[crypto.Count - 1], bytes);

            return listToArray(crypto, sizeOfBuffer);
        }

        private void createCryptoTo(Type type)
        {
            byte[] data = Settings.Crypto.encoding.GetBytes(Settings.Crypto.encoding.GetString(new byte[] { 1 }) + Settings.Connect.Name);
            if (data.Length > 63)
                data = Settings.Crypto.encoding.GetBytes(Settings.Crypto.encoding.GetString(new byte[] { 1 }) + Settings.Connect.Name, 0, 63);

            data[0] = (byte)type;

            Stream.Write(data);
            Stream.Flush();

            data = new byte[4096];
            int bytes = Stream.Read(data, 0, data.Length);

            byte[] openkey = new byte[bytes];
            Array.Copy(data, openkey, bytes);

            crypterTo = CryptersFactory.newCrypter((CryptersFactory.CryptoType)type, openkey);
        }

        private Type createCryptoFrom()
        {
            byte[] data = new byte[64];
            int bytes = Stream.Read(data, 0, data.Length);
            crypterFrom = CryptersFactory.newCrypter((CryptersFactory.CryptoType)data[0], Settings.Crypto.sizeBlock);

            name = Settings.Crypto.encoding.GetString(data, 1, bytes - 1);

            Stream.Write(crypterFrom.getOpenKey());
            Stream.Flush();

            return (Type)data[0];
        }

        private byte[] listToArray(List<byte[]> list, int size)
        {
            byte[] ansver = new byte[(list.Count - 1) * size + list.Last().Length];

            int i = 0;
            foreach (byte[] subj in list)
            {
                Array.Copy(subj, 0, ansver, i, subj.Length);
                i += subj.Length;
            }

            return ansver;
        }
    }
}