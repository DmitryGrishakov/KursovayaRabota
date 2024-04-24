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

namespace KursovayaRabota
{
    public partial class Stores : Form
    {
        public Stores()
        {
            InitializeComponent();
        }

        private void Stores_Load(object sender, EventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db");

            conn.Open();

            string query = "SELECT Name, Phone, Address FROM Stores";

            SQLiteCommand cmd = new SQLiteCommand(query, conn);

            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                dataGridView1.Rows.Add(reader["Name"], reader["Phone"], reader["Address"]);
            }

            conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddStores addStores = new AddStores();
            addStores.ShowDialog(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = dataGridView1.SelectedRows.Count > 0 ? dataGridView1.SelectedRows[0] : null;
            if (selectedRow != null)
            {
                string name = selectedRow.Cells["Column1"].Value.ToString();
                string phone = selectedRow.Cells["Column2"].Value.ToString();
                string address = selectedRow.Cells["Column3"].Value.ToString();

                ChangeStores changeStores = new ChangeStores(name, phone, address);
                changeStores.ShowDialog(this);
            }
        }

        private void DeleteRecordFromDatabase(int rowIndex)
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db"))
            {
                conn.Open();

                // Получаем индекс записи для удаления
                int selectedRowIndex = rowIndex + 1;

                // Удаляем запись из базы данных
                string deleteQuery = "DELETE FROM Stores WHERE ROWID = @RowIndex";
                using (SQLiteCommand cmd = new SQLiteCommand(deleteQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@RowIndex", selectedRowIndex);
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Запись успешно удалена из базы данных!");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении записи из базы данных!");
                        return; // Выходим из метода, так как произошла ошибка
                    }
                }

                // Обновляем индексы в базе данных
                string updateQuery = "UPDATE Stores SET ROWID = ROWID - 1 WHERE ROWID > @RowIndex";
                using (SQLiteCommand cmd = new SQLiteCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@RowIndex", selectedRowIndex);
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Получаем индекс выбранной строки
                int rowIndex = selectedRow.Index;

                // Вызываем метод для удаления записи из базы данных
                DeleteRecordFromDatabase(rowIndex);

                // Удаляем строку из DataGridView
                dataGridView1.Rows.RemoveAt(rowIndex);
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.");
            }
        }
    }
}
