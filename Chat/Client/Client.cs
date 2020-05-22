using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TCPNetwork;

namespace Client {
    class Client {
        public delegate void Log(string s);
        public event Log ClientLog;
        private string username;
        private String serverHost;
        private Socket cSocket;
        private int port = 8034;
        private NetMessaging net;
        private string msgToSend = null;
        Thread messaging;
        Thread communicate;
        public bool isConnected { get; private set; }
        public Client(String serverHost, string username) {
            this.serverHost = serverHost;
            this.username = username;
            isConnected = false;
        }
        public void Start() {
            isConnected = false;
            try {
                ClientLog.Invoke(String.Format("[Server]: Подключение к {0}", this.serverHost));
                cSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                cSocket.Connect(this.serverHost, port);
                net = new NetMessaging(cSocket);
                
                net.LoginCmdReceived += OnLogin;
                net.UserListCmdReceived += OnUserList;
                net.StartCmdReceived += OnStart;
                net.MessageCmdReceived += OnMessage;

                communicate = new Thread(() => {
                    try {
                        net.Communicate();
                    } catch (Exception) {
                        ClientLog.Invoke("[Server]: Не удалось получить данные. Завершение соединения...");
                    }
                });
                communicate.Start();
            } catch (Exception) {
                ClientLog.Invoke("[Server]: Что-то пошло не так... :(");
            }
        }

        public void SendMessage(string s) {
            msgToSend = s;
        }
        private void OnMessage(string command, string data) {
            if(isConnected)
                ClientLog.Invoke(String.Format("{0}", data));
        }

        private void OnStart(string command, string data) {
            if (isConnected) {
                ClientLog.Invoke("[Server]: Вы можете писать сообщения!");
                GoMessaging();
            }   
        }

        private void OnUserList(string command, string data) {
            var us = data.Split(',');
            isConnected = true;
            foreach (var cl in us) {
                if (cl == username) {
                    ClientLog.Invoke("[Server]: Имя пользователя занято");
                    isConnected = false;
                    return;
                }     
            }
            ClientLog.Invoke("[Server]: Список подключенных клиентов:");
            if (us.Length == 0 || (us.Length == 1 && us[0].Length == 0))
                ClientLog?.Invoke("[Server]: Вы единственный пользователь на сервере");
            foreach (var cl in us) {
                if (cl.Length != 0)
                    ClientLog.Invoke(">" + cl);
            }
            ClientLog.Invoke("-----------------------------");
        }

        private void GoMessaging() {
            messaging = new Thread(() => {
                while (true) {

                    if (msgToSend != null && isConnected) {
                        net.SendData("MESSAGE", msgToSend);
                        msgToSend = null;
                    }


                }
            }
            );
            messaging.Start();
        }

        void OnLogin(string c, string d) {
            isConnected = true;
            net.SendData("LOGIN", username);
        }
        public void Stop() {
            
            if (cSocket != null) cSocket.Close();
            if (communicate != null) communicate.Abort();
            if (messaging != null) messaging.Abort();
        }

    }
}
