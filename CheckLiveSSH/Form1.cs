using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckLiveSSH
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        DataTable table;
        public DataTable Convert(string File, string TableName, string delimiter)
        {
            table = new DataTable();

            table.Columns.Add("ipaddress", typeof(string));
            table.Columns.Add("username", typeof(string));
            table.Columns.Add("password", typeof(string));
            table.Columns.Add("location", typeof(string));
            table.Columns.Add("pos", typeof(string));
            table.Columns.Add("city", typeof(string));
            table.Columns.Add("status", typeof(string));

            StreamReader s = new StreamReader(File);

            string AllData = s.ReadToEnd();
            string[] stringSeparators = new string[] { "" };
            string[] rows = AllData.Split(stringSeparators, StringSplitOptions.None);
            foreach (string r in rows)
            {
                string[] items = r.Split(delimiter.ToCharArray());
                table.Rows.Add(items);
            }
            return table;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text files (*.txt) | *.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string url = ofd.FileName;
                txt_url.Text = url;
                DataTable dt = Convert(url, "test", "|");
                gv_data.DataSource = dt;

            }
        }
        int somay = 0;
        private int _soMayChon;
        private int _mayChamXong;
        private int somayChamXong
        {
            set
            {
                lock (gv_data)
                {
                    _mayChamXong = value;
                    if (_mayChamXong >= _soMayChon)
                    {

                        //progressBarControl1.BeginInvoke(new Action(() =>
                        //{
                        //    Double percent = 100;
                        //    progressBarControl1.EditValue = percent;
                        //}));
                    }
                    else
                    {
                        //progressBarControl1.BeginInvoke(new Action(() =>
                        //{
                        //    Double percent = Convert.ToDouble(_mayChamXong) / Convert.ToDouble(_soMayChon) * 100.0;
                        //    progressBarControl1.EditValue = percent;
                        //    Load_DuLieu();
                        //}));
                    }
                }
            }
            get
            {
                lock (gv_data)
                {
                    return _mayChamXong;
                }
            }
        }
        private void TinhTrangKetNoi(int rowHandle, string msg)
        {
            gv_data.BeginInvoke((Action)(() =>
            {
                gv_data[6, rowHandle].Value = msg;
            }));
        }
        private void btn_start_Click(object sender, EventArgs e)
        {
            _soMayChon = table.Rows.Count;
            somayChamXong = 0;
            //int i = 0;

            var ipList = new List<KeyValuePair<int, string>>();

            for (var r = 0; r <= table.Rows.Count - 1; r++)
            {
                somay++;

                ipList.Add(new KeyValuePair<int, string>(r, table.Rows[r]["ipaddress"].ToString()));
            }

            if (somay > 0)
            {


                foreach (var ipInfo in ipList)
                {
                    Task.Run(async () =>
                    {
                        TinhTrangKetNoi(ipInfo.Key, "Đang kiểm tra...");
                        MySSH ssh = new MySSH();
                        int r = ipInfo.Key;
                        string username = table.Rows[r]["username"].ToString();
                        string password = table.Rows[r]["password"].ToString();
                        string result = await ssh.checkSSH(ipInfo.Value, username, password, "google.com", "15");
                        TinhTrangKetNoi(ipInfo.Key, result);
                    });
                };

            }
        }
    }
}
