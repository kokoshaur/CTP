using Open.Nat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CTP   //Cryptographic transmission protocol
{
    public enum Type : byte
    {
        RSA = Crypter.CryptersFactory.CryptoType.RSA,
        GAMAL = Crypter.CryptersFactory.CryptoType.GAMAL,
        XOR = Crypter.CryptersFactory.CryptoType.XOR
    }
    public class CTPManager
    {
        public bool allowNewConnection { get; private set; } = true;
        public bool allowAcceptMessage { get; private set; } = true;

        TcpListener tcpListener;
        public int port { private set; get; }

        List<Friend> friends = new List<Friend>();
        public Action<string> showMessage;
        public Action<string, byte[]> showFrame;

        Task listenNewFriends;
        Task listenNewMessage;

        public CTPManager(Action<string> showMessage, Action<string, byte[]> showFrame = null)
        {
            this.showMessage = showMessage;
            this.showFrame = showFrame;

            listenNewFriends = new Task(listenFriends);
            listenNewMessage = new Task(listenMessage);
        }

        public async void openPortAsync(int port, Action afterOpen)
        {
            try
            {
                this.port = port;
                var discoverer = new NatDiscoverer();
                var cts = new CancellationTokenSource(10000);
                var device = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);

                await device.CreatePortMapAsync(new Mapping(Protocol.Tcp, port, port, Settings.Connect.Name));
                showMessage(Language.Connect.successfulOpenPort);
                afterOpen();
            }
            catch (Exception)
            {
                showMessage(Language.Error.errorOpenPort);
            }
        }

        public async void closePortAsync(int port, Action afterClose)
        {
            try
            {
                var discoverer = new NatDiscoverer();
                var cts = new CancellationTokenSource(10000);
                var device = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);

                await device.DeletePortMapAsync(new Mapping(Protocol.Tcp, port, port, Settings.Connect.Name));
                afterClose();
            }
            catch (Exception)
            {
                showMessage(Language.Error.errorOpenPort);
            }
        }

        public bool addFriend(string ip, int port, Type type = Type.RSA)
        {
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(ip, port);
                new Friend(client).connectTo(friends, type);
                showMessage(Language.Connect.connect);
            }
            catch (Exception ex)
            {
                showMessage(ex.Message);
                return false;
            }

            return true;
        }

        public void startListenNewFriends()
        {
            allowNewConnection = true;
            listenNewFriends.Start();
        }

        public void startListenMessage()
        {
            allowAcceptMessage = true;
            listenNewMessage.Start();
        }

        public void stopListenNewFriends()
        {
            allowNewConnection = false;
        }

        public void stopListenMessage()
        {
            allowAcceptMessage = false;
        }

        public void refreshCrypto(string nameFriend, Type type)
        {
            Task.Run(() =>
            {
                foreach (Friend friend in friends.ToArray())
                    if (friend.name.Equals(nameFriend))
                        friend.refreshCrypto((byte)type);
            });
        }

        public void sendToAll(string message)
        {
            Task.Run(() =>
            {
                foreach (Friend friend in friends.ToArray())
                    friend.send(message);
            });
        }

        public void sendToAll(byte[] message)
        {
            Task.Run(() =>
            {
                foreach (Friend friend in friends.ToArray())
                    friend.send(message);
            });
        }

        public void sendToAll(FileStream file)
        {
            Task.Run(() =>
            {
                foreach (Friend friend in friends.ToArray())
                    friend.send(file);
            });
        }

        public void close()
        {
            stopListenNewFriends();
            stopListenMessage();

            if (Language.Connect.goodbye != "")
                sendToAll(Language.Connect.goodbye);

            foreach (Friend friend in friends)
                friend.disconnect();

            tcpListener.Stop();
        }

        private void listenFriends()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, port);
                tcpListener.Start();
                showMessage(Language.Connect.successfulLaunchServer);

                while (allowNewConnection)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    Friend friend = new Friend(tcpClient);
                    friend.connectFrom(friends, showMessage);
                }
            }
            catch (Exception ex)
            {
                showMessage(ex.Message);
                close();
            }
        }

        private void listenMessage()
        {
            Command command = 0;
            string message = "";
            byte[] data = null;

            while (allowAcceptMessage)
            {
                foreach (Friend friend in friends.ToArray())
                    if (friend.getMessage(ref command, ref message, ref data))
                        processResult(command, message, data);
                Thread.Sleep(Settings.Connect.checkMessageDelay);
            }
        }

        private void processResult(Command command, in string message, in byte[] data)
        {
            if ((command == Command.NewFrame) && (showFrame != null))
                showFrame(message, data);
            else
                showMessage(message);
        }
    }
}