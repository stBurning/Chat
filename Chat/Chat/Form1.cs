using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Chat {
    public partial class Form1 : Form {

        private Server server;
        Thread serverThread;
        public Form1() {
            InitializeComponent();
            server = new Server();
            server.ServerLog += Server_ServerLog;
            
        }

        private void Server_ServerLog(string s) {
            if (!richTextBox1.InvokeRequired) {
                richTextBox1.AppendText(s + '\n');
            } else
                Invoke(new Server.Log(Server_ServerLog), s);
        }

        private void button1_Click(object sender, EventArgs e) {
            
            button1.Enabled = false;
            serverThread = new Thread(() => {
                server.Start();
            });
            serverThread.Start();
              

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            
            server.Stop();
            if (serverThread != null) {
                serverThread.Interrupt();
                Environment.Exit(0); 
            }

        }
    }

    
}
