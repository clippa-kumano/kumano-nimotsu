﻿using System;
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
        int staff_uid=0;
        string staff_ryosei_name="", staff_ryosei_room="";


        Color color_register = Color.FromArgb(218, 255, 245);
        Color color_release = Color.FromArgb(217, 255, 218);
        Color color_delete = Color.FromArgb(255, 216, 216);

        public Form1()
        {
            InitializeComponent();
            dataGridView1.RowTemplate.Height = 60;
            dataGridView2.RowTemplate.Height = 60;

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ShowCellInformation(sender, e, "left_side");
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ShowCellInformation(sender, e, "right_top_side");
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
                //ryosei table col 0:部屋番号、1:氏名、2:荷物数、3:登録、4:受取、5:slack_id, 6:ryosei_uid
                //event table  col 0:イベント種類、1:uid、2:部屋番号, 3:氏名、4:時刻、5:note、6:parcel_uid,7:ryosei_uid,8:is_finished
                if (row >= 0 && col == 1 && boxTitle == "left_side")//登録
                {
                    change_staff(int.Parse(g[6, row].Value.ToString()), g[0, row].Value.ToString(), g[1, row].Value.ToString());
                }
                if (row >= 0 && col == 3&& boxTitle=="left_side")//登録
                {

                    register(int.Parse(g[6, row].Value.ToString()),staff_uid,g[1, row].Value.ToString(), g[0, row].Value.ToString(), g[5, row].Value.ToString());
                    
                }
                if (row >= 0 && col == 4&& boxTitle == "left_side"&& int.Parse(g[2, row].Value.ToString())>0)//受取
                {
                    release(int.Parse(g[6, row].Value.ToString()),staff_uid, g[1, row].Value.ToString(), g[0, row].Value.ToString());
                    //result = MessageBox.Show(string.Format("行：{0}, 列：{1}, 値：{2}", row, col, g[col, row].Value), boxTitle, MessageBoxButtons.OKCancel);
                }
                if (row >= 0 && col >=0 &&boxTitle == "right_top_side")
                {
                    string test = g[8, row].Value.ToString();
                        if (g[8, row].Value.ToString() != "True" &&  g[0, row].Value.ToString() != "その他")//既に受け取られた登録イベントは削除できない/事務当登録イベントは削除できない
                    {
                        confirm(g[1, row].Value.ToString(), g[7, row].Value.ToString(), g[6, row].Value.ToString(), g[2, row].Value.ToString(), g[3, row].Value.ToString());

                    }
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
                MakeSQLCommand sqlstr = new MakeSQLCommand();
                sqlstr.block_id = ryoseiTable_block;
                cmd.CommandText = sqlstr.forShow_ryosei_table();
                var sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            dataGridView1.DataSource = dt;
            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.RowTemplate.Height = 60;
            this.dataGridView1.Columns["slack_id"].Visible = false;
            this.dataGridView1.Columns["uid"].Visible = false;

            //各行に色を付ける処理
            //dataGridView2.Rows.Countとやると、常に51になる？？
            //よくわからんがバグの温床っぽい
            dataGridView1.Columns[3].DefaultCellStyle.BackColor = color_register;
            dataGridView1.Columns[4].DefaultCellStyle.BackColor = color_release;
            this.dataGridView1.CurrentCell = null;


        }
        void show_parcels_eventTable()
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                var cmd = conn.CreateCommand();
                MakeSQLCommand sqlstr = new MakeSQLCommand();
                cmd.CommandText = sqlstr.forShow_event_table();
                var sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            dataGridView2.DataSource = dt;
            this.dataGridView2.Columns["操作時刻"].DefaultCellStyle.Format = "MM/dd HH:mm:ss";
            //this.dataGridView2.Columns["uid"].Visible = false;
            this.dataGridView2.Columns["parcel_uid"].Visible = false;
            this.dataGridView2.Columns["ryosei_uid"].Visible = false;
            this.dataGridView2.Columns["is_finished"].Visible = false;

            this.dataGridView2.CurrentCell = null;

            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView2.RowTemplate.Height = 60;


            //各行に色を付ける処理
               //Rows.Count-1が大事
            for (int row = 0; row < dataGridView2.Rows.Count-1; row++)
            {
                string col = dataGridView2.Rows[row].Cells[0].Value.ToString();
                if (col == "登録")
                {
                    dataGridView2.Rows[row].Cells[0].Style.BackColor = color_register;
                }
                if (col == "受取")
                {
                    dataGridView2.Rows[row].Cells[0].Style.BackColor = color_release;
                }
                if (col == "削除")
                {
                    dataGridView2.Rows[row].Cells[0].Style.BackColor = color_delete;
                }
            }

        }


        void register(int owner_uid, int staff_uid, string ryosei_name, string room_name,string slack_id)
        {
            if (staff_uid == 0)
            {
                MessageBox.Show("事務当を登録してください", "boxTitle", MessageBoxButtons.OK);
                return;
            }
            DialogResult result;
            result = MessageBox.Show(room_name + " " + ryosei_name + "さんの荷物を登録します。", "boxTitle", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {


                //SQL文の作成
                MakeSQLCommand sqlstr = new MakeSQLCommand();
                //---------------parcels {owner_room_name}','{owner_ryosei_name}','{register_datetime}','{register_staff_room_name}','{register_staff_ryosei_name}',{placement}
                sqlstr.owner_uid = owner_uid;
                sqlstr.register_staff_uid = staff_uid;
                //sqlstr.owner_room_name = room_name;
                //sqlstr.owner_ryosei_name = ryosei_name;
                sqlstr.register_datetime = DateTime.Now.ToString();
                //sqlstr.register_staff_room_name = textBox1.Text;
                //sqlstr.register_staff_ryosei_name = textBox2.Text;
                sqlstr.placement = 0;//暫定
                //----------------event created_at,event_type,room_name,ryosei_name,parcel_uid
                sqlstr.event_type = 1; //登録は1
                //parcel_uid => SQL文で自動取得
                //---------------ryoseiテーブルに必要なデータはすべて上で網羅されている
                string aSqlStr = "";
                aSqlStr += sqlstr.toRegister_parcels_table();
                aSqlStr += sqlstr.toRegister_parcelevent_table();//parcelsテーブルの更新よりも後に行う（parcel_uidをSQL文で取得しているため）
                aSqlStr += sqlstr.toRegister_ryosei_table();


                //実際に書き換え
                //参考：ttps://www.ipentec.com/document/csharp-sql-server-connect-exec-sql
                Operation ope = new Operation(connStr);
                ope.execute_sql(aSqlStr);
                show_parcels_eventTable();
                show_ryoseiTable();

                //slackでの通知
                if (slack_id != "")//登録していない人はDBではNULLとなっており、ここには""(空のstring)の形で来る
                {
                    Httppost httppost = new Httppost();
                    httppost.user_code = slack_id;
                    httppost.message_str = $"{sqlstr.register_datetime} に荷物が登録されました。";
                    httppost.posting_DM();
                }

            }


        }
        void release(int owner_uid, int staff_uid, string ryosei_name, string room_name)
        {
            if (staff_uid == 0)
            {
                MessageBox.Show("事務当を登録してください", "boxTitle", MessageBoxButtons.OK);
                return;
            }


            DateTime dt = DateTime.Now;//total_wait_timeの計算にも使用している。
            //参考：ttps://www.ipentec.com/document/csharp-sql-server-connect-exec-sql
            MakeSQLCommand sqlstr = new MakeSQLCommand();
            sqlstr.owner_uid = owner_uid;

            string sqlstr_get_all_current_parcel= sqlstr.toRelease_get_all_parcels();
            Operation ope = new Operation(connStr);
            List<int> CurrentParcels = ope.get_all_current_parcel(sqlstr_get_all_current_parcel);
            //現状はその人名義の荷物をすべて取得している
            //ここを書き換えれば、荷物を選択とかできると思うけど、今のままにしておいてすべて受け取らせて必要があればイベント削除、とかの運用のほうが良いと思う。

            sqlstr.release_datetime = dt.ToString();
            sqlstr.release_staff_uid = staff_uid;
            sqlstr.parcels_total_waittime = ope.calculate_registered_time(CurrentParcels, dt,owner_uid);
            string aSqlStr = "";
            aSqlStr += sqlstr.toRelease_parcels_table(CurrentParcels);
            aSqlStr += sqlstr.toRelease_parcelevent_table(CurrentParcels);
            aSqlStr += sqlstr.toRelease_ryosei_table(CurrentParcels);


            DialogResult result;
            string msgbox_str = $@"{room_name} {ryosei_name} さんの荷物は、現在{CurrentParcels.Count.ToString()}個登録されています。
荷物をすべて受け取りますか？
";
            result = MessageBox.Show(msgbox_str, "boxTitle", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                ope.execute_sql(aSqlStr);
                show_parcels_eventTable();
                show_ryoseiTable();
            }

        }
        void confirm(string event_uid_str, string ryosei_uid_str, string parcel_uid_str,string room_name, string ryosei_name)
        {
            DialogResult result;
            string msgbox_str = $@"#{event_uid_str} の操作を取り消しますか？";
            result = MessageBox.Show(msgbox_str, "boxTitle", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                DialogResult result2;
                string msgbox_str2 = $@"一度取り消した操作は元に戻すことは出来ません。
#{event_uid_str} の操作を取り消します。
よろしいですか？";
                result2 = MessageBox.Show(msgbox_str2, "boxTitle", MessageBoxButtons.OKCancel);

                if (result2 == DialogResult.OK)
                {
                    int event_uid, ryosei_uid, parcel_uid;
                    int.TryParse(event_uid_str, out event_uid);
                    int.TryParse(ryosei_uid_str, out ryosei_uid);
                    int.TryParse(parcel_uid_str, out parcel_uid);

                    MakeSQLCommand makeSQLCommand = new MakeSQLCommand();
                    //{created_at}',{event_type},{parcel_uid},'{owner_room_name}','{owner_ryosei_name}
                    makeSQLCommand.created_at = DateTime.Now.ToString();
                    makeSQLCommand.event_uid = event_uid;
                    makeSQLCommand.parcel_uid = parcel_uid;
                    makeSQLCommand.owner_uid = ryosei_uid;

                    string sqlstr = "";
                    sqlstr += makeSQLCommand.toDeleteLogically_event_table();
                    sqlstr += makeSQLCommand.toDeleteLoogically_ryosei_table();
                    sqlstr += makeSQLCommand.toDeleteLogically_parcels_table();
                    Operation ope = new Operation(connStr);
                    ope.execute_sql(sqlstr);

                    show_parcels_eventTable();
                    show_ryoseiTable();
                }
            }
        }

        void change_staff(int ryosei_uid, string room_name, string ryosei_name)
        {
            DialogResult result;
            result = MessageBox.Show("事務当を交代します\r\n次の事務当は" + room_name + " " + ryosei_name + "さんです。", "boxTitle", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                textBox1.Text = room_name;
                textBox2.Text = ryosei_name;
                staff_uid = ryosei_uid;
                staff_ryosei_room = room_name;
                staff_ryosei_name = ryosei_name;

                MakeSQLCommand makeSQLCommand = new MakeSQLCommand();
                //{created_at}',{event_type},{parcel_uid},'{owner_room_name}','{owner_ryosei_name}
                makeSQLCommand.ryosei_uid = ryosei_uid;
                makeSQLCommand.created_at = DateTime.Now.ToString();
                string sqlstr = makeSQLCommand.toChangeStaff_event_table();
                Operation ope = new Operation(connStr);
                ope.execute_sql(sqlstr);

                show_parcels_eventTable();
            }

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



        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}