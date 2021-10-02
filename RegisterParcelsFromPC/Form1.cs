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
        int staff_uid = 0;
        int night_duty_mode = 0;
        string staff_ryosei_name = "", staff_ryosei_room = "";


        Color color_register = Color.FromArgb(218, 255, 245);
        Color color_release = Color.FromArgb(217, 255, 218);
        Color color_delete = Color.FromArgb(255, 216, 216);
        Color color_deletable = Color.FromArgb(255, 240, 240);

        Color color_night_duty_mode = Color.Lavender;

        List<int> deletable_event_uid;
        int deletable_seconds = 300;

        int test = 0;

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
                //event table  col 0:イベント種類、1:uid、2:部屋番号, 3:氏名、4:時刻、5:note、6:parcel_uid,7:ryosei_uid,8:is_finished("True"もしくは"False"で渡される）
                if (boxTitle == "left_side")
                {
                    string room_name = g[0, row].Value.ToString();
                    string ryosei_name = g[1, row].Value.ToString();
                    int current_parcel_count = int.Parse(g[2, row].Value.ToString());
                    string slack_id = g[5, row].Value.ToString();
                    int ryosei_uid = int.Parse(g[6, row].Value.ToString());

                    if (row >= 0 && col == 1)//事務当の登録
                    {
                        change_staff(ryosei_uid, room_name, ryosei_name);
                    }
                    if (row >= 0 && col == 3)//荷物の登録
                    {

                        register(ryosei_uid, staff_uid, ryosei_name, room_name, slack_id);

                    }
                    if (row >= 0 && col == 4 && int.Parse(g[2, row].Value.ToString()) > 0)//受取
                    {
                        release(ryosei_uid, staff_uid, ryosei_name, room_name);
                        //result = MessageBox.Show(string.Format("行：{0}, 列：{1}, 値：{2}", row, col, g[col, row].Value), boxTitle, MessageBoxButtons.OKCancel);
                    }
                }


                if (row >= 0 && col >= 0 && boxTitle == "right_top_side")
                {//イベント削除のイベント。分岐がややこしいので、関数の中で分岐するように変更したい。

                    string event_type = g[0, row].Value.ToString();
                    int event_uid = int.Parse(g[1, row].Value.ToString());
                    string room_name = g[2, row].Value.ToString();
                    string ryosei_name = g[3, row].Value.ToString();
                    int parcel_uid = int.Parse(g[6, row].Value.ToString());
                    int ryosei_uid = int.Parse(g[7, row].Value.ToString());
                    string is_finished = g[8, row].Value.ToString();//TrueまたはFalse


                    delete(event_type, event_uid, ryosei_uid, parcel_uid, room_name, ryosei_name, is_finished);

                    
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
                if (night_duty_mode == 0)
                {
                    cmd.CommandText = sqlstr.forShow_ryosei_table();
                }
                else
                {
                    cmd.CommandText = sqlstr.forShow_ryosei_table_night_duty_mode();
                }

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
            for (int row = 0; row < dataGridView2.Rows.Count - 1; row++)
            {
                string event_type = dataGridView2.Rows[row].Cells[0].Value.ToString();
                string is_finished = dataGridView2.Rows[row].Cells[8].Value.ToString();
                if (event_type == "登録")
                {
                    dataGridView2.Rows[row].Cells[0].Style.BackColor = color_register;
                }
                if (event_type == "受取")
                {
                    dataGridView2.Rows[row].Cells[0].Style.BackColor = color_release;
                }
                if (is_finished == "False" && (event_type == "登録" || event_type == "受取"))
                {
                    dataGridView2.Rows[row].Cells[1].Style.BackColor = color_deletable;

                }
                if (event_type == "削除")
                {
                    dataGridView2.Rows[row].Cells[0].Style.BackColor = color_delete;
                }
                if (event_type == "モード解除" || event_type == "モード開始")
                {
                    dataGridView2.Rows[row].Cells[0].Style.BackColor = color_night_duty_mode;
                }
            }

        }


        void register(int owner_uid, int staff_uid, string ryosei_name, string room_name, string slack_id)
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

            string sqlstr_get_all_current_parcel = sqlstr.toRelease_get_all_parcels();
            Operation ope = new Operation(connStr);
            List<int> CurrentParcels = ope.get_all_uid(sqlstr_get_all_current_parcel);
            //現状はその人名義の荷物をすべて取得している
            //ここを書き換えれば、荷物を選択とかできると思うけど、今のままにしておいてすべて受け取らせて必要があればイベント削除、とかの運用のほうが良いと思う。

            sqlstr.release_datetime = dt.ToString();
            sqlstr.release_staff_uid = staff_uid;
            sqlstr.parcels_total_waittime = ope.calculate_registered_time(CurrentParcels, dt, owner_uid);
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
        void confirm(string event_type, string event_uid_str, string ryosei_uid_str, string parcel_uid_str, string room_name, string ryosei_name)
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
                    if (event_type == "登録") makeSQLCommand.event_type = 1;
                    if (event_type == "受取") makeSQLCommand.event_type = 2;

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

        void delete(string event_type, int event_uid,int ryosei_uid, int parcel_uid, string room_name, string ryosei_name, string is_finished)
        {
            //when 1 then '登録' when 2 then '受取' when 3 then '削除' when 10 then '当番交代' when 11 then 'モード開始' when 12 then 'モード解除'  else 'その他'
            //MakaSQLCommandのforShow_event_table()部分を参照する
            //enumとかを使ってエレガントにしたさもある

            //削除ができるのは、受取がされていない登録(eventのis_finished=0のもの）と、受取イベント。
            //過去に受取が一度されたが削除された場合→現在は登録イベントを取り消せないようにしておく
            if (event_type == "当番交代") return;
            if (event_type == "モード開始") return;
            if (event_type == "モード終了") return;
            if (event_type == "削除") return;


            //ここで5分以内かどうかを判定



            //ダイアログを出す（一度目）
            DialogResult result;
            string msgbox_str = $@"#{event_uid} の操作を取り消しますか？";
            result = MessageBox.Show(msgbox_str, "boxTitle", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                //ここで、登録イベントであり過去に受取がされていれば処理を終了する

                if (event_type == "登録" && is_finished == "True")
                {
                    MessageBox.Show("この荷物は過去に受取されているため、イベントを削除できません", "boxTitle", MessageBoxButtons.OK);
                    return;
                }

                //ダイアログを出す（二度目）
                DialogResult result2;
                string msgbox_str2 = $@"一度取り消した操作は元に戻すことは出来ません。
#{event_uid} の操作を取り消します。
よろしいですか？";
                result2 = MessageBox.Show(msgbox_str2, "boxTitle", MessageBoxButtons.OKCancel);

                if (result2 == DialogResult.OK)
                { 

                    MakeSQLCommand makeSQLCommand = new MakeSQLCommand();
                    if (event_type == "登録") makeSQLCommand.event_type = 1;
                    if (event_type == "受取") makeSQLCommand.event_type = 2;

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

        void periodical_check()
        {
            //10秒に一度呼ばれる
            //deletable_eventを更新する
            //具体的に言うと、登録もしくは受取イベントで、is_finishedが0であり、5分以上経過しているイベントのis_finishedを1にする

            MakeSQLCommand sqlstr = new MakeSQLCommand();
            DateTime dt = DateTime.Now;
            dt=dt.AddSeconds(-1 * deletable_seconds);
            string base_time = dt.ToString("yyyy-MM-dd HH:mm:ss");
            sqlstr.created_at = base_time;
            string getlist_periodicCheck = sqlstr.toGetList_PeriodicCheck();//更新対象の一覧を取得
            string sqlstr_periodicCheck = sqlstr.toPeriodicCheck();//実際のupdate文

            Operation ope = new Operation(connStr);
            List<int> target_uid=ope.get_all_uid(getlist_periodicCheck);//先に更新対象の一覧を種痘
            ope.execute_sql(sqlstr_periodicCheck);//update文をかます
            //更新対象があったときのみ、イベントテーブルを再描画
            if (target_uid.Count > 0)
            {
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

        void Change_Mode(int event_type)
        {
            MakeSQLCommand makeSQLCommand = new MakeSQLCommand();
            makeSQLCommand.event_type = event_type;
            makeSQLCommand.created_at = DateTime.Now.ToString();
            string sqlstr = makeSQLCommand.toChangeMode();
            Operation ope = new Operation(connStr);
            ope.execute_sql(sqlstr);
            show_parcels_eventTable();

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

        private void button2_Click(object sender, EventArgs e)
        {
            periodical_check();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (night_duty_mode == 0)
            {
                night_duty_mode = 1;
                MessageBox.Show("泊事務当確認モード");
                Change_Mode(11);
            }
            else
            {
                night_duty_mode = 0;
                MessageBox.Show("モード解除");
                Change_Mode(12);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            periodical_check();
            show_parcels_eventTable();
            test++;
            textBox4.Text = test.ToString();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
