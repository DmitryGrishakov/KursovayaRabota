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
    public partial class Deliveries : Form
    {
        public Deliveries()
        {
            InitializeComponent();
        }

        private DataTable dataGridViewData; // Переменная для хранения данных из DataGridView

        private void Deliveries_Load(object sender, EventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db");

            conn.Open();

            string query = "SELECT Companies.Name AS CompanyName, Stores.Name AS StoreName, Products.Name AS ProductName, " +
                           "ProductsInDeliveries.Quantity, Deliveries.Cost, Deliveries.Delivery_date " +
                           "FROM Deliveries " +
                           "JOIN Companies ON Deliveries.Company_ID = Companies.Company_ID " +
                           "JOIN Stores ON Deliveries.Store_ID = Stores.Store_ID " +
                           "JOIN ProductsInDeliveries ON Deliveries.Delivery_ID = ProductsInDeliveries.Delivery_ID " +
                           "JOIN Products ON ProductsInDeliveries.Product_ID = Products.Product_ID";

            SQLiteCommand cmd = new SQLiteCommand(query, conn);

            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string companyName = reader["CompanyName"].ToString();
                string storeName = reader["StoreName"].ToString();
                string productName = reader["ProductName"].ToString();
                int quantity = Convert.ToInt32(reader["Quantity"]);
                decimal cost = Convert.ToDecimal(reader["Cost"]);
                DateTime deliveryDate = Convert.ToDateTime(reader["Delivery_date"]);

                // Проверка наличия элемента в списке перед добавлением
                if (!IsItemInDataGridView(dataGridView1, companyName, storeName, productName, quantity, cost, deliveryDate))
                {
                    dataGridView1.Rows.Add(companyName, storeName, productName, quantity, cost, deliveryDate);
                }
            }

            conn.Close();
        }

        private bool IsItemInDataGridView(DataGridView dataGridView, string companyName, string storeName, string productName, int quantity, decimal cost, DateTime deliveryDate)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[0]?.Value?.ToString() == companyName &&
                    row.Cells[1]?.Value?.ToString() == storeName &&
                    row.Cells[2]?.Value?.ToString() == productName &&
                    Convert.ToInt32(row.Cells[3].Value) == quantity &&
                    Convert.ToDecimal(row.Cells[4].Value) == cost &&
                    Convert.ToDateTime(row.Cells[5].Value) == deliveryDate)
                {
                    return true; // Элемент уже существует в DataGridView
                }
            }
            return false; // Элемент не найден
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddDeliveries addDeliveries = new AddDeliveries();
            addDeliveries.ShowDialog(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataGridViewRow selectedRow = dataGridView1.SelectedRows.Count > 0 ? dataGridView1.SelectedRows[0] : null;
            if (selectedRow != null)
            {
                string companyName = selectedRow.Cells["Column1"].Value.ToString();
                string storeName = selectedRow.Cells["Column2"].Value.ToString();
                string productName = selectedRow.Cells["Column6"].Value.ToString();
                int quantity = Convert.ToInt32(selectedRow.Cells["Column3"].Value);
                decimal cost = Convert.ToDecimal(selectedRow.Cells["Column4"].Value);
                DateTime deliveryDate = Convert.ToDateTime(selectedRow.Cells["Column5"].Value);

                // Создаем экземпляр формы редактирования поставок и передаем данные
                ChangeDeliveries changeDeliveries = new ChangeDeliveries(companyName, storeName, productName, quantity, cost, deliveryDate);
                changeDeliveries.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("Выберите поставку для редактирования.");
            }
        }

        private void DeleteDeliveryFromDatabase(int rowIndex)
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db"))
            {
                try
                {
                    conn.Open();

                    // Получаем индекс записи для удаления
                    int selectedRowIndex = rowIndex + 1;

                    // Удаляем связанные записи из таблицы ProductsInDeliveries
                    string deleteProductsQuery = "DELETE FROM ProductsInDeliveries WHERE ROWID = @ROWID";
                    using (SQLiteCommand cmd = new SQLiteCommand(deleteProductsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ROWID", selectedRowIndex);
                        cmd.ExecuteNonQuery();
                    }

                    // Удаляем запись из таблицы Deliveries
                    string deleteQuery = "DELETE FROM Deliveries WHERE ROWID = @ROWID";
                    using (SQLiteCommand cmd = new SQLiteCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ROWID", selectedRowIndex);
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Поставка успешно удалена из базы данных!");
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при удалении поставки из базы данных!");
                            return; // Выходим из метода, так как произошла ошибка
                        }
                    }

                    // Обновляем индексы в базе данных
                     string updateQuery = "UPDATE ProductsInDeliveries SET ROWID = ROWID - 1 WHERE ROWID > @ROWID; " +
                     "UPDATE Deliveries SET ROWID = ROWID - 1 WHERE ROWID > @ROWID;" +
                     "UPDATE ProductsInDeliveries SET Delivery_ID = Delivery_ID - 1 WHERE Delivery_ID > @DeliveryID;";

                    using (SQLiteCommand cmd = new SQLiteCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ROWID", selectedRowIndex);
                        cmd.Parameters.AddWithValue("@DeliveryID", selectedRowIndex);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Товар успешно удален из поставки!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message);
                }
                finally
                {
                    // Закрытие соединения в блоке finally, если оно было открыто успешно
                    if (conn != null && conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
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
                DeleteDeliveryFromDatabase(rowIndex);

                // Удаляем строку из DataGridView
                dataGridView1.Rows.RemoveAt(rowIndex);
            }
            else
            {
                MessageBox.Show("Выберите поставку для удаления.");
            }
        }
    }
}
