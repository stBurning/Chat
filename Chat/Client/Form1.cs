using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client {
    public partial class Form1 : Form {
        private Client client = null;
        public Form1() {
            InitializeComponent();
            
        }

        private void ClientLog(string s) {
            try {
                if (!messageField.InvokeRequired) {
                    messageField.AppendText(s + '\n');
                } else
                    Invoke(new Client.Log(ClientLog), s);
            } catch (Exception) {

            }
            
        }

        private void button1_Click(object sender, EventArgs e) {
            if (client != null) client.Stop();
            messageField.Text = "";
            if(hostTextBox.Text.Length == 0) {
                MessageBox.Show("Введите адресс или имя сервера");
                return;
            } else {
                try {
                    client = new Client(hostTextBox.Text, usernameTextBox.Text);
                    client.ClientLog += ClientLog;
                    client.Start();                    
                } catch {
                    
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) {    
            if(messageBox.Text.Length != 0)
                client.SendMessage(messageBox.Text);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            client.Stop();
            
        }
    }
}
