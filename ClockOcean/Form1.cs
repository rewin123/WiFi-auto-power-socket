using System;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace ClockOcean
{
    public partial class Form1 : Form
    {
        string data = "config.txt";
        DateTime up;
        DateTime down;
        bool visible = true;
        public Form1()
        {
            InitializeComponent();

            Hide();
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;

            dateTimePicker1.Format = DateTimePickerFormat.Time;
            dateTimePicker2.Format = DateTimePickerFormat.Time;

            if (File.Exists(data))
            {
                string[] lines = File.ReadAllLines(data);
                up = DateTime.Parse(lines[0]);
                down = DateTime.Parse(lines[1]);

                dateTimePicker1.Value = up;
                dateTimePicker2.Value = down;
            }
            else
            {
                up = new DateTime(2017, 1, 1, 9, 0, 0);
                down = new DateTime(2017, 1, 1, 22, 0, 0);

                dateTimePicker1.Value = up;
                dateTimePicker2.Value = down;

                File.WriteAllLines(data, new string[] { up.ToLongTimeString(), down.ToLongTimeString() });
            }

            Send(visible, 10);

            timer1.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            visible = !visible;
            string set = visible ? "0" : "1";


            HttpWebRequest request = HttpWebRequest.CreateHttp("http://192.168.1.23/D2/" + set);
            request.GetResponse();
            request.Abort();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            up = dateTimePicker1.Value;

            File.WriteAllLines(data, new string[] { up.ToLongTimeString(), down.ToLongTimeString() });
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            down = dateTimePicker2.Value;

            File.WriteAllLines(data, new string[] { up.ToLongTimeString(), down.ToLongTimeString() });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            if (visible)
            {
                if((hour * 60 + minute) > (down.Hour * 60 + down.Minute))
                {
                    if (Send(false))
                        visible = false;
                }
                else if((hour * 60 + minute) < (up.Hour * 60 + up.Minute))
                {
                    if (Send(false))
                        visible = false;
                }
            }
            else
            {
                if ((hour * 60 + minute) > (up.Hour * 60 + up.Minute) && (hour * 60 + minute) < (down.Hour * 60 + down.Minute))
                {
                    if (Send(true))
                        visible = true;
                }

            }
        }

        bool Send(bool rele, int interactions = 5)
        {
            string set = rele ? "0" : "1";

            for (int i = 0; i < interactions; i++)
            {
                try
                {
                    HttpWebRequest request = HttpWebRequest.CreateHttp("http://192.168.1.10/D2/" + set);
                    request.GetResponse();
                    request.Abort();
                    return true;
                }
                catch
                {

                }
            }

            return false;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible == false)
            {
                Show();
                ShowInTaskbar = true;
                WindowState = FormWindowState.Normal;
            }
            else
            {
                Hide();
                ShowInTaskbar = false;
            }
        }
    }
}
