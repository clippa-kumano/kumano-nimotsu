using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RegisterParcelsFromPC
{
    public partial class Form5 : Form
    {
        string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=parcels;UID=sa;PWD=kumano";

        void RegisterSlack_id()
        {
            string room_name = textBox2.Text;
            string ryosei_name = textBox3.Text;
            string slack_id = textBox4.Text;
            MakeSQLCommand sqlstr = new MakeSQLCommand();
            sqlstr.room_name = room_name;
            sqlstr.ryosei_name = ryosei_name;
            sqlstr.slack_id = slack_id;
            string aSqlstr = sqlstr.toRegister_slack();
            Operation ope = new Operation(connStr);
            ope.execute_sql(aSqlstr);
            MessageBox.Show("登録されました。");

        }

        public Form5()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RegisterSlack_id();
        }
    }
}
