using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MqttClient
{
    public partial class MainWindow : Form
    {
        private Client client = new Client();

        public MainWindow()
        {
            //Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            button1.Enabled = false;

            client.Init();
        }

        public void Log(string str)
        {
            this.Invoke((EventHandler)(delegate
            {
                this.textBox1.Text += str + "\r\n";
            }));
        }

        public void EnableButton(bool ops)
        {
            this.Invoke((EventHandler)(delegate
            {
                this.button1.Enabled = ops;
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.Send(this.textBox2.Text);
        }
    }
}