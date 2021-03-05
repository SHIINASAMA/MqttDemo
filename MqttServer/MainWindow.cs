using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MqttServer
{
    public partial class MainWindow : Form
    {
        private Server server = new();

        public MainWindow()
        {
            InitializeComponent();
            this.button1.Enabled = false;
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
            server.Publish(this.textBox2.Text);
            this.textBox2.Clear();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            server.Init();
        }
    }
}