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
    public partial class ChangeStores : Form
    {
        public ChangeStores()
        {
            InitializeComponent();
        }

        private void ChangeStores_Load(object sender, EventArgs e)
        {

        }

        private string OldName;
        private string OldPhone;
        private string OldAddress;

        public ChangeStores(string name, string phone, string address)
        {
            InitializeComponent();
            OldName = name;
            OldPhone = phone;
            OldAddress = address;
            textBox1.Text = name;
            textBox2.Text = phone;
            textBox3.Text = address;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string editedName = textBox1.Text;
            string editedPhone = textBox2.Text;
            string editedAddress = textBox3.Text;

            if (!Regex.IsMatch(editedName, "^[а-яА-Яa-zA-Z -]+$"))
            {
                MessageBox.Show("Пожалуйста введите название магазина корректно.");
            }
            else if (!Regex.IsMatch(editedPhone, "^[0-9+ -]+$"))
            {
                MessageBox.Show("Пожалуйста введите телефон магазина корректно.");
            }
            else if (!Regex.IsMatch(editedAddress, "^[а-яА-Я.0-9, -]+$"))
            {
                MessageBox.Show("Пожалуйста введите адрес магазина корректно.");
            }
            else
            {
                if (editedName != OldName || editedPhone != OldPhone || editedAddress != OldAddress)
                {
                    using (SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db"))
                    {
                        conn.Open();
                        string query = "UPDATE Stores SET Name = @NewName, Phone = @NewPhone, Address = @NewAddress WHERE Name = @OldName AND Phone = @OldPhone AND Address = @OldAddress";

                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@NewName", editedName);
                            cmd.Parameters.AddWithValue("@NewPhone", editedPhone);
                            cmd.Parameters.AddWithValue("@NewAddress", editedAddress);

                            cmd.Parameters.AddWithValue("@OldName", OldName);
                            cmd.Parameters.AddWithValue("@OldPhone", OldPhone);
                            cmd.Parameters.AddWithValue("@OldAddress", OldAddress);

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
