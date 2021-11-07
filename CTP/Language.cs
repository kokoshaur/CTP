namespace CTP
{
    public static class Language
    {
        public static class Connect
        {
            public static string successfulOpenPort = "Порт успешно открыт";
            public static string successfulLaunchServer = "Сервер запущен. Ожидание подключений...";
            public static string goodbye = "Всем пока!";
            public static string connect = "Подключился";
            public static string newMessage = "Принято 1 сообщение";
            public static string newFile = "Принят 1 файл: ";
            public static string newBytes = "Принят поток байт";
            public static string accept = "Получено";
            public static string bytes = "байт";
            public static string updateCrypter = "Режим шифрования успешно сменён на ";
            public static string lostPacket = "Потеря пакета";
        }

        public static class Error
        {
            public static string errorConnect = "Неудачная попытка подключения";
            public static string errorOpenPort = "Роутер не поддерживает UPnP";
        }
    }
}