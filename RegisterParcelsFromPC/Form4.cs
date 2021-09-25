using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RegisterParcelsFromPC
{
    public partial class Form4 : Form
    {
        string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=parcels;UID=sa;PWD=kumano";
        
        void register_new_ryosei()
        {           
            string new_room_name = textBox1.Text;
            string new_name = textBox2.Text;
            string new_name_kana = textBox3.Text;
            MakeSQLCommand sqlstr = new MakeSQLCommand();
            sqlstr.room_name = new_room_name;
            sqlstr.ryosei_name = new_name;
            sqlstr.ryosei_name_kana = new_name_kana;
            string block_id_new = new_room_name.Substring(1,1);
            sqlstr.block_id = int.Parse(block_id_new);
            string aSqlstr = sqlstr.Register_new_ryosei_table();
            Operation ope = new Operation(connStr);
            ope.execute_sql(aSqlstr);
            MessageBox.Show("登録されました。");            
        }
        
        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            register_new_ryosei();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
