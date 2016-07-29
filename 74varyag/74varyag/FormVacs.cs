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
namespace _74varyag
{
    public partial class FormVacs : Form
    {

        
        public FormVacs()
        {
            InitializeComponent();
        }

        private void LoadVacs()
        {
            SqlConnection sqlConnection1 = new System.Data.SqlClient.SqlConnection("Data Source=ERU-ПК\\TESTSRV;Initial Catalog=var74;Persist Security Info=True;User ID=user_var;Password=pass123"); //Вынести в параметры
            sqlConnection1.Open();
            string query = "SELECT Dolzh,Org,Ob,Treb,Usl,minZP, maxZP, Url FROM vacs";
            SqlDataAdapter dataAdapter = new System.Data.SqlClient.SqlDataAdapter(query,sqlConnection1);
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            
            this.dataGridView1.ReadOnly = true; 
            this.dataGridView1.DataSource = ds.Tables[0];
            dataGridView1.Columns[0].HeaderText = "Должность";
            dataGridView1.Columns[1].HeaderText = "Организация";
            dataGridView1.Columns[2].HeaderText = "Обязанности";
            dataGridView1.Columns[3].HeaderText = "Требования";
            dataGridView1.Columns[4].HeaderText = "Условия";
            dataGridView1.Columns[5].HeaderText = "мин Зарплата";
            dataGridView1.Columns[6].HeaderText = "макс Зарплата";
            dataGridView1.Columns[7].HeaderText = "Ссылка";
            sqlConnection1.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            LoadVacs();
        }

        private void FormVacs_Load(object sender, EventArgs e)
        {
         LoadVacs();
        }

        private string Filter = "";
        private string org = "", dolz = "";
        private int min ,max;
       
        //Установка фильтра
        private void SetFilter()
        {
           if(min>0 && max>0)
            {
               Filter = String.Format("Dolzh like '%{0}%' and Org like '%{1}%' and minZP >={2} and maxZp <={3}", dolz, org, min, max);
            }
            else
            {
                if (min > 0 && max <= 0)
                {
                    Filter = String.Format("Dolzh like '%{0}%' and Org like '%{1}%' and minZP >={2}", dolz, org, min);
                }
                if(min<=0 && max>0)
               {
                    Filter = String.Format("Dolzh like '%{0}%' and Org like '%{1}%' and maxZP <={2}", dolz, org, max);
               }
                if(min<=0 && max<=0)
                {
                    Filter = String.Format("Dolzh like '%{0}%' and Org like '%{1}%'", dolz, org);
                }
            }
            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = Filter;
        }

        //Очистка фильтра
        private void ClearFilter()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            Filter = org = dolz = "";
            min = max = 0;
            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = "";
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dolz = textBox1.Text;
            SetFilter();
            //(dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format("Dolzh like '%{0}%'", textBox1.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            org = textBox2.Text;
            SetFilter();
            //(dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format("Org like '%{0}%'", textBox2.Text);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(textBox3.Text, out min);
            SetFilter();
           //(dataGridView1.DataSource as DataTable).DefaultView.RowFilter = "minZP >=" + textBox3.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(textBox4.Text, out max);
            SetFilter();
            //(dataGridView1.DataSource as DataTable).DefaultView.RowFilter = "maxZP <=" + textBox4.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClearFilter();
        }
    }
}
