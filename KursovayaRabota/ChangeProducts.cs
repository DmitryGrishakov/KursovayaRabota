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
    public partial class ChangeProducts : Form
    {
        public ChangeProducts()
        {
            InitializeComponent();
        }

        private void ChangeProducts_Load(object sender, EventArgs e)
        {

        }

        private string OldName;
        private decimal OldPrice;

        public ChangeProducts(string name, decimal price)
        {
            InitializeComponent();
            OldName = name;
            OldPrice = price;
            textBox1.Text = name;
            textBox2.Text = price.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string editedName = textBox1.Text;
            string editedPrice = textBox2.Text;

            if (!Regex.IsMatch(editedName, "^[а-яА-Яa-zA-Z -]+$"))
            {
                MessageBox.Show("Пожалуйста введите название продукта корректно.");
            }
            else if (!Regex.IsMatch(editedPrice, @"^\d+(\.\d{1,2})?$"))
            {
                MessageBox.Show("Пожалуйста введите корректную цену продукта.");
            }
            else
            {
                if (editedName != OldName || Convert.ToDecimal(editedPrice) != OldPrice)
                {
                    using (SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db"))
                    {
                        conn.Open();
                        string query = "UPDATE Products SET Name = @NewName, Price = @NewPrice WHERE Name = @OldName AND Price = @OldPrice";

                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@NewName", editedName);
                            cmd.Parameters.AddWithValue("@NewPrice", Convert.ToDecimal(editedPrice));

                            cmd.Parameters.AddWithValue("@OldName", OldName);
                            cmd.Parameters.AddWithValue("@OldPrice", OldPrice);

                            int result = cmd.ExecuteNonQuery();
                            if (result > 0)
                            {
                                MessageBox.Show("Данные успешно обновлены в базе данных!");
                            }
                            else
                            {
                                MessageBox.Show("Ошибка при обновлении данных в базе данных!");
                            }
                        }
                        conn.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Данные не были изменены.");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
