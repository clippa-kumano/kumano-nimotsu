using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RegisterParcelsFromPC
{
    public partial class Form1 : Form
    {
        string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=parcels;UID=sa;PWD=kumano";
        int ryoseiTable_block = 1;
  



        public Form1()
        {
            InitializeComponent();
 

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ShowCellInformation(sender, e, "CellContentClick");
        }

        void ShowCellInformation(object sender, DataGridViewCellEventArgs args, string boxTitle)
        {

            DataGridView g = sender as DataGridView;

            if (g != null)
            {
                int col = args.ColumnIndex;
                int row = args.RowIndex;

                //
                // クリックがヘッダー部分などの場合はインデックスが-1となります。
                //現状はcol 0:部屋番号、1:氏名、2:荷物数、3:登録、4:受取
                //
                DialogResult result;
                if (row >= 0 && col >=0&col<=2)
                {
                    result =MessageBox.Show(string.Format("行：{0}, 列：{1}, 値：{2}", row, col, g[col, row].Value), boxTitle, MessageBoxButtons.OK);
                }
                if (row >= 0 && col ==3)//登録
                {
                    result = MessageBox.Show(g[1, row].Value+"さんの荷物を登録します。", boxTitle, MessageBoxButtons.OKCancel);
                    if (col == 3 && result == DialogResult.OK)
                    {
                        register(1, g[1, row].Value.ToString(), g[0, row].Value.ToString(),1);
                    }
                }
                if (row >= 0 && col == 4)//受取
                {
                    result = MessageBox.Show(string.Format("行：{0}, 列：{1}, 値：{2}", row, col, g[col, row].Value), boxTitle, MessageBoxButtons.OKCancel);
                }


                
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            show_parcels_eventTable();
        }

        void show_ryoseiTable()
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT [room_name] as '部屋番号',[ryosei_name] as '氏名',[parcels_current_count] as '荷物数', '登録' as '登録','受取' as '受取' FROM [parcels].[dbo].[ryosei] where block_id='" + ryoseiTable_block+"'";
                var sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            dataGridView1.DataSource = dt;
            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }
        void show_parcels_eventTable()
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT top(50) case [event_type] when 1 then '登録' when 2 then '受取' else 'その他' end  as '操作種類',  [room_name] as '部屋番号',[ryosei_name] as '氏名　　　',[created_at] as '操作時刻',[note] as '特記事項', parcel_uid from parcel_event order by created_at desc";
                var sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            dataGridView2.DataSource = dt;
            this.dataGridView2.Columns["操作時刻"].DefaultCellStyle.Format = "MM/dd HH:mm:ss";
            this.dataGridView2.Columns["parcel_uid"].Visible = false;

            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ryosei_id"></param>
        /// <param name="ryosei_name"></param>
        /// <param name="event_type">1:登録、2:受取</param>
        void register(int ryosei_id, string ryosei_name, string room_name, int event_type)
        {

            //参考：ttps://www.ipentec.com/document/csharp-sql-server-connect-exec-sql
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                //parcelテーブル用
                string v_owner_room_name, v_owner_ryosei_name, v_register_datatime, v_register_staff_id, v_register_staff_room_name,v_register_staff_ryosei_name, v_is_fridge;
                
                v_owner_room_name = room_name;
                v_owner_ryosei_name = ryosei_name;
                DateTime dt = DateTime.Now;
                v_register_datatime = dt.ToString();
                v_register_staff_id = "1";
                v_register_staff_room_name =textBox1.Text;
                v_register_staff_ryosei_name =textBox2.Text;
                v_is_fridge = "0";
                string query_insert_parcels = "insert into [parcels] (owner_room_name,owner_ryosei_name,register_datatime, register_staff_id, register_staff_room_name,register_staff_ryosei_name,is_fridge) values ('"+v_owner_room_name+"','" +v_owner_ryosei_name + "','" + v_register_datatime + "'," + v_register_staff_id + ",'" +v_register_staff_room_name+"','"+ v_register_staff_ryosei_name + "'," + v_is_fridge+")";
                SqlCommand command_parcels = new SqlCommand(query_insert_parcels, conn);
                command_parcels.ExecuteNonQuery();

                //eventテーブル用
                string query_get_parcel_uid = "select top(1) * from parcels order by uid desc";
                SqlCommand command_parcels_event_1 = new SqlCommand(query_get_parcel_uid, conn);
                SqlDataReader sdr1 = command_parcels_event_1.ExecuteReader();
                sdr1.Read();
                DateTime _confirm_register_datatime = (DateTime)sdr1["register_datatime"];
                int _parcel_uid = (int)sdr1["uid"];
                if (_confirm_register_datatime.ToString() == v_register_datatime)
                {
                    //先頭の行が実行後のやつかどうか不安なのでチェックしたかった
                }
                sdr1.Close();
                command_parcels_event_1.Dispose();

                string v_created_at, v_event_type, v_parcel_uid, v_room_name, v_ryosei_name;
                v_created_at = v_register_datatime;
                v_event_type = event_type.ToString();
                v_parcel_uid = _parcel_uid.ToString();
                v_room_name = room_name;
                v_ryosei_name = ryosei_name;

                string query_insert_event = "insert into [parcel_event] (created_at,event_type,parcel_uid,room_name,ryosei_name) values ('"+v_created_at+"',"+v_event_type + "," + v_parcel_uid + ",'" + v_room_name + "','" + v_ryosei_name+"')";
                SqlCommand command_parcels_event = new SqlCommand(query_insert_event, conn);
                command_parcels_event.ExecuteNonQuery();

                //ryoseiテーブル用
                string query_get_ryosei ="select * from ryosei where ryosei_name='"+v_owner_ryosei_name+"'";
                SqlCommand command_ryosei_1 = new SqlCommand(query_get_ryosei, conn);
                SqlDataReader sdr2 = command_ryosei_1.ExecuteReader();
                sdr2.Read();
                int _parcels_current_count = (int)sdr2["parcels_current_count"];
                int _parcels_total_count = (int)sdr2["parcels_total_count"];
                sdr2.Close();
                command_ryosei_1.Dispose();

                string v_parcels_current_count, v_parcels_total_count, v_last_event_datatime;
                v_parcels_current_count = (_parcels_current_count + 1).ToString();
                v_parcels_total_count = (_parcels_total_count + 1).ToString();
                v_last_event_datatime = v_register_datatime;
                string query_update_ryosei = "update ryosei set parcels_current_count=" + v_parcels_current_count + ",parcels_total_count=" + v_parcels_total_count + ",last_event_datatime='" + v_last_event_datatime + "' where ryosei_name='"+ryosei_name+"'";
                SqlCommand command_ryosei_2 = new SqlCommand(query_update_ryosei, conn);
                command_ryosei_2.ExecuteNonQuery();
                conn.Close();
            }
            show_parcels_eventTable();
            show_ryoseiTable();


        }
        void change_blockTabImage(int n)
        {           
            
            //リソースマネージャーを使う
            System.Reflection.Assembly assembly;

            assembly = System.Reflection.Assembly.GetExecutingAssembly();

            //Project1はプロジェクト名
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager
            ("RegisterParcelsFromPC.Properties.Resources", assembly);

            //指定されたリソースにインポートした画像を読み込む
            //JANは画像名　Januaryの一月やね。

            Bitmap A1 = (Bitmap)rm.GetObject("tab_a1");
            Bitmap A2 = (Bitmap)rm.GetObject("tab_a2");
            Bitmap A1_B = (Bitmap)rm.GetObject("tab_a1_B");
            Bitmap A2_B = (Bitmap)rm.GetObject("tab_a2_B");


            pictureBox1.Image = A1;
            pictureBox2.Image = A2;
            /*
            pictureBox3.Image = A3;
            pictureBox4.Image = A4;
            pictureBox5.Image = B12;
            */
            if (n == 1) pictureBox1.Image = A1_B;
            if (n == 2) pictureBox2.Image = A2_B;



        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ryoseiTable_block = 1;
            show_ryoseiTable();
            change_blockTabImage(1);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ryoseiTable_block = 2;
            show_ryoseiTable();
            change_blockTabImage(2);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            ryoseiTable_block = 3;
            show_ryoseiTable();
            change_blockTabImage(0);
        }
         
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            ryoseiTable_block = 4;
            show_ryoseiTable();
            change_blockTabImage(0);
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            ryoseiTable_block = 5;
            show_ryoseiTable();
            change_blockTabImage(0);
        }


        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
