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
    public partial class FormResume : Form
    {
        public FormResume()
        {
            InitializeComponent();
        }

        private void LoadResume()
        {
            SqlConnection sqlConnection1 = new System.Data.SqlClient.SqlConnection("Data Source=ERU-ПК\\TESTSRV;Initial Catalog=var74;Persist Security Info=True;User ID=user_var;Password=pass123"); //Вынести в параметры
            sqlConnection1.Open();
            string query = "SELECT Spec,Place, ZP, Url FROM resume";
            SqlDataAdapter dataAdapter = new System.Data.SqlClient.SqlDataAdapter(query, sqlConnection1);
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);

            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.DataSource = ds.Tables[0];
            dataGridView1.Columns[0].HeaderText = "Должность";
            dataGridView1.Columns[1].HeaderText = "Город";
            dataGridView1.Columns[2].HeaderText = "Зарплата";
            dataGridView1.Columns[3].HeaderText = "Ссылка";
            dataGridView1.Columns[3].Width = 386;
            sqlConnection1.Close();
        }

        private string Filter = "";
        private string place = "", dolz = "";
        private int min, max;

        //Установка фильтра
        private void SetFilter()
        {
            if (min > 0 && max > 0)
            {
                Filter = String.Format("Spec like '%{0}%' and Place like '%{1}%' and ZP >={2} and ZP <={3}", dolz, place, min, max);
            }
            else
            {
                if (min > 0 && max <= 0)
                {
                    Filter = String.Format("Spec like '%{0}%' and Place like '%{1}%' and ZP >={2}", dolz, place, min);
                }
                if (min <= 0 && max > 0)
                {
                    Filter = String.Format("Spec like '%{0}%' and Place like '%{1}%' and ZP <={2}", dolz, place, max);
                }
                if (min <= 0 && max <= 0)
                {
                    Filter = String.Format("Spec like '%{0}%' and Place like '%{1}%'", dolz, place);
                }
            }
            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = Filter;
        }

        private void ClearFilter()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            Filter = place = dolz = "";
            min = max = 0;
            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = "";
        }

        private void FormResume_Load(object sender, EventArgs e)
        {
            LoadResume();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadResume();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClearFilter();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dolz = textBox1.Text;
            SetFilter();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            place = textBox2.Text;
            SetFilter();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(textBox3.Text, out min);
            SetFilter();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(textBox4.Text, out max);
            SetFilter();
        }
    }
}
