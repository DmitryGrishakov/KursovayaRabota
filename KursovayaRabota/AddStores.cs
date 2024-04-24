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
    public partial class AddStores : Form
    {
        public AddStores()
        {
            InitializeComponent();
        }

        private void AddStores_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string one = textBox1.Text;
            string two = textBox2.Text;
            string three = textBox3.Text;

            if (string.IsNullOrEmpty(one) || string.IsNullOrEmpty(two) || string.IsNullOrEmpty(three))
            {
                MessageBox.Show("Обязательно заполните все поля!");
            }

            else if (!Regex.IsMatch(one, "^[а-яА-Яa-zA-Z -]+$"))
            {
                MessageBox.Show("Пожалуйста введите название магазина корректно.");
            }

            else if (!Regex.IsMatch(two, "^[0-9+ -]+$"))
            {
                MessageBox.Show("Пожалуйста введите телефон магазина корректно.");
            }

            else if (!Regex.IsMatch(three, "^[а-яА-Я.0-9, -]+$"))
            {
                MessageBox.Show("Пожалуйста введите адрес магазина корректно.");
            }
            else
            {
                SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db");
                conn.Open();

                string query = "INSERT INTO Stores (Name, Phone, Address) VALUES (@Name, @Phone, @Address)";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", one);
                    cmd.Parameters.AddWithValue("@Phone", two);
                    cmd.Parameters.AddWithValue("@Address", three);

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
