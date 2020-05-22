using System;
using System.Windows.Forms;
using System.Data.SQLite;

namespace 分货小助手
{
    public partial class Form1 : Form
    {
        SQLiteConnection m_dbConnection;
        public Form1()
        {
            InitializeComponent();
        }

        void connectToDatabase()
        {
            m_dbConnection = new SQLiteConnection("Data Source=devicesend.db;Version=3;");
            m_dbConnection.Open();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            connectToDatabase();
            string sql = "select cityname,cityid,citycode from city order by cityid";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                dataGridView1.Rows.Add(reader["cityname"].ToString(), "0", reader["cityid"].ToString(), reader["citycode"].ToString());
            }
                

            sql = "select devname from device";
            command = new SQLiteCommand(sql, m_dbConnection);
            reader = command.ExecuteReader();
            while (reader.Read())
                comboBox1.Items.Add(reader["devname"].ToString());
        }


        public static String getTimeMillisFromZero()
        {
            int datenow = DateTime.Now.Millisecond+ DateTime.Now.Second*1000+ DateTime.Now.Minute*1000*60+ DateTime.Now.Hour * 1000 * 60*60;
            return String.Format("{0:D8}",datenow);
        }

        public void Add_Move_Repeat(string str, ComboBox comboBox)
        {
            //MessageBox.Show(comboBox.Items[i].ToString());
            if (comboBox.Items.Count > 0)
            {
                int i = 0;
                for (; i < comboBox.Items.Count; i++)
                {
                    if (comboBox.Items[i].ToString().Equals(str))
                    {
                        break;
                    }
                }
                if (i == comboBox.Items.Count)
                    comboBox.Items.Add(str);
            }
            else
            {
                comboBox.Items.Add(str);
            }
        }

        public void Self_Move_Repeat(ComboBox comboBox)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                for (int j = i + 1; j < comboBox.Items.Count; j++)
                {
                    if (comboBox.Items[i].Equals(comboBox.Items[j]))
                        comboBox.Items.Remove(comboBox.Items[j]);
                }
            }
        }

        private void ListBox1_Click(object sender, EventArgs e)
        {
            ListBox eObj = sender as ListBox;
            String info = eObj.SelectedItem as string;
            comboBox1.Text = info;
            comboBox1.Select(comboBox1.Text.Length, 1); //光标定位到最后
        }

        private void ComboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            listBox1.Items.Clear();
            ComboBox eObj = sender as ComboBox; //事件源对象
            comboBox1 = eObj; //当前事件出发对象
            String key = comboBox1.Text;
            string sql = "select devname from device where devname like '%"+key+"%'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                listBox1.Items.Add(reader["devname"].ToString());
            comboBox1.Select(comboBox1.Text.Length, 1); //光标定位到文本框最后
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            string MATERIELCODE = null;
            String devcode = comboBox1.Text;
            string lendOrderNo = null;
            string sql1 = null;
            string sql2 = null;
            string sql = "select devcode from device where devname = '"+devcode+"'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                MATERIELCODE = reader["devcode"].ToString();
            if(MATERIELCODE==null|| MATERIELCODE.Equals(""))
            {
                MessageBox.Show("请输入正确的设备型号");
            }
            else
            {
                for (int i = 0; i < 17; i++)
                {
                    if (dataGridView1.Rows[i].Cells["Column2"].Value.Equals("0"))
                    {
                        continue;
                    }
                    lendOrderNo = "LPO" + DateTime.Now.ToString("yyyyMMdd") + getTimeMillisFromZero();
                    sql1 = "insert into sd_lend_order(LENDORDERNO, LENDSTATUS, CREATETIME, CITYCODE, GODOWNCODE, CREATOR, REMARK, MATERIELCODE, ORDERTYPE)" +
                        "values ('" + lendOrderNo + "','1',sysdate,'" + dataGridView1.Rows[i].Cells["Column3"].Value
                        + "','" + dataGridView1.Rows[i].Cells["Column4"].Value + "','guyingtao','" + textBox1.Text + "','" + MATERIELCODE + "','1');";
                    sql2 = "insert into SD_DELIVERY_ORDER_CONFIRM(DELIVERYID, DELIVERYNUM, CONFIRMNUM, MINUSNUM, DEVCODE) " +
                        "values('"+ lendOrderNo + "', '"+ dataGridView1.Rows[i].Cells["Column2"].Value + "', '0', '"+ dataGridView1.Rows[i].Cells["Column2"].Value + "', '100');";
                    textBox2.Text += sql1;
                    textBox2.Text += "\r\n";
                    textBox2.Text += sql2;
                    textBox2.Text += "\r\n";

                }
            }
        }
    }
}
