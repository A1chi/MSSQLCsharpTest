using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
namespace MSSQLCsharp
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null;    
        private SqlConnection nordConnection = null;
        private List<string[]> rows;
        private List<string[]> filtrList = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            nordConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["NordDB"].ConnectionString);
            nordConnection.Open();
            //------------------------DataGridView
            SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Products", nordConnection);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            dataGridView2.DataSource = ds.Tables[0];

            SqlDataReader dataReader = null;
            rows = new List<string[]>();
            string[] row = null;

            try
            {
                SqlCommand sqlCommand = new SqlCommand("SELECT ProductName, QuantityPerUnit, UnitPrice FROM Products", nordConnection);

                dataReader = sqlCommand.ExecuteReader();

                while(dataReader.Read())
                {
                    row = new string[]
                    {
                        Convert.ToString(dataReader["ProductName"]),
                        Convert.ToString(dataReader["QuantityPerUnit"]),
                        Convert.ToString(dataReader["UnitPrice"])
                    };
                    rows.Add(row);
                }
            }
            catch(Exception ex)
            { MessageBox.Show(ex.Message); }
            finally
            {
                if(dataReader != null && !dataReader.IsClosed)
                { dataReader.Close(); }
            }
            RefreshList(rows);
        }

        private void RefreshList(List<string[]> list)
        {
            listView2.Items.Clear();
            foreach (string[] s in list)
            {
                listView2.Items.Add(new ListViewItem (s));
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand(
                $"INSERT INTO [Products] (ProductName, SupplierID, QuantityPerUnit, UnitPrice, UnitsInStock, CategoryID) VALUES (@ProductName, @SupplierID, @QuantityPerUnit, @UnitPrice, @UnitsInStock, @CategoryID)",
                nordConnection);
            
            command.Parameters.AddWithValue("ProductName",textBox1.Text);
            command.Parameters.AddWithValue("SupplierID", textBox2.Text);
            command.Parameters.AddWithValue("QuantityPerUnit", textBox3.Text);
            command.Parameters.AddWithValue("UnitPrice", double.Parse(textBox4.Text));
            command.Parameters.AddWithValue("UnitsInStock", int.Parse(textBox5.Text));
            command.Parameters.AddWithValue("CategoryID", textBox6.Text);
            MessageBox.Show($"Add product {command.ExecuteNonQuery().ToString()}");
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

            SqlDataAdapter dataAdapter = new SqlDataAdapter(textBox7.Text,
                nordConnection);

            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            dataGridView1.DataSource = dataSet.Tables[0];
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            SqlDataReader sqlreader = null;
            try
            {
                SqlCommand sqlCommand = new SqlCommand("SELECT ProductName,QuantityPerUnit,UnitPrice FROM Products",nordConnection);
                sqlreader = sqlCommand.ExecuteReader();
                ListViewItem item = null;
                while (sqlreader.Read())
                {
                    item = new ListViewItem(new string[] { Convert.ToString(sqlreader["ProductName"]),
                        Convert.ToString(sqlreader["QuantityPerUnit"]),
                        Convert.ToString(sqlreader["UnitPrice"]) });
                    listView1.Items.Add(item);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sqlreader != null && !sqlreader.IsClosed)
                {
                    sqlreader.Close();
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = $"UnitsInStock <= 10";
                    break;
                case 1:
                    (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = $"UnitsInStock >= 10 AND UnitsInStock <= 50";
                    break;
                case 2:
                    (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = $"UnitsInStock >= 50";
                    break;
                case 3:
                    (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = $"";
                    break;


            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = $"ProductName LIKE '%{textBox9.Text}%'";
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            filtrList = rows.Where((x) =>
            x[0].ToLower().Contains(textBox10.Text.ToLower())).ToList();

            RefreshList(filtrList);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    filtrList = rows.Where((x) =>
                    Double.Parse(x[2]) <= 10).ToList();
                    RefreshList(filtrList);
                    break;
                case 1:
                    filtrList = rows.Where((x) =>
                    Double.Parse(x[2]) > 10 && Double.Parse(x[2]) <= 100).ToList();
                    RefreshList(filtrList);
                    break;
                case 2:
                    filtrList = rows.Where((x) =>
                    Double.Parse(x[2]) > 100).ToList();
                    RefreshList(filtrList);
                    break;
                case 3:
                    RefreshList(rows);
                    break;
                    
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Products", nordConnection);
            DataSet ds = new DataSet();
            dataAdapter.Fill(ds);
            dataGridView2.DataSource = ds.Tables[0];

        }
    }
}
