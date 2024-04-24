using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace KursovayaRabota
{
    public partial class AddProducts : Form
    {
        public AddProducts()
        {
            InitializeComponent();
        }

        private void AddProducts_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string productName = textBox1.Text;
            string productPrice = textBox2.Text;

            if (string.IsNullOrEmpty(productName) || string.IsNullOrEmpty(productPrice))
            {
                MessageBox.Show("Обязательно заполните все поля!");
            }
            else if (!Regex.IsMatch(productName, "^[а-яА-Яa-zA-Z -]+$"))
            {
                MessageBox.Show("Пожалуйста введите название продукта корректно.");
            }
            else if (!Regex.IsMatch(productPrice, @"^\d+(\.\d{1,2})?$"))
            {
                MessageBox.Show("Пожалуйста введите корректную цену продукта.");
            }
            else
            {
                SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db");
                conn.Open();

                string query = "INSERT INTO Products (Name, Price) VALUES (@Name, @Price)";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", productName);
                    cmd.Parameters.AddWithValue("@Price", Convert.ToDecimal(productPrice));

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Данные успешно добавлены в базу данных!");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении данных в базу данных!");
                    }
                }

                conn.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
