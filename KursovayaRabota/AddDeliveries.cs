using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace KursovayaRabota
{
    public partial class AddDeliveries : Form
    {

        private decimal calculatedCost;

        private void AddDeliveries_Load(object sender, EventArgs e)
        {
            LoadCompanies();
            LoadStores();
            LoadProducts();
        }

        public AddDeliveries()
        {
            InitializeComponent();
            
        }

        private void LoadCompanies()
        {
            // Код для загрузки данных о компаниях
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db"))
            {
                conn.Open();

                string query = "SELECT Name FROM Companies";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBox1.Items.Add(reader["Name"]);
                        }
                    }
                }
            }
        }

        private void LoadStores()
        {
            // Код для загрузки данных о магазинах
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db"))
            {
                conn.Open();

                string query = "SELECT Name FROM Stores";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBox2.Items.Add(reader["Name"]);
                        }
                    }
                }
            }
        }

        private void LoadProducts()
        {
            // Код для загрузки данных о продуктах
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db"))
            {
                conn.Open();

                string query = "SELECT Name FROM Products";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBox3.Items.Add(reader["Name"]);
                        }
                    }
                }
            }
        }

        private decimal CalculateTotalCost(string productName, int quantity)
        {
            int productId = GetProductId(productName);

            if (productId != -1)
            {
                decimal unitPrice = GetUnitPrice(productId);
                return unitPrice * quantity;
            }

            return -1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string companyName = comboBox1.Text;
            string storeName = comboBox2.Text;
            string productName = comboBox3.Text;
            string quantityText = textBox1.Text;

            if (string.IsNullOrEmpty(companyName) || string.IsNullOrEmpty(storeName) || string.IsNullOrEmpty(productName) || string.IsNullOrEmpty(quantityText))
    {
                MessageBox.Show("Обязательно заполните все поля!");
                return;
            }

            if (!int.TryParse(quantityText, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите количество товара корректно!");
                return;
            }

            // Вычисление стоимости на основе цены за единицу товара и количества
            decimal calculatedCost = CalculateTotalCost(productName, quantity);
            DateTime deliveryDate = dateTimePicker1.Value;

            // Код сохранения данных
            SaveDeliveryData(companyName, storeName, productName, quantity, calculatedCost, deliveryDate);
        }


        private void SaveDeliveryData(string companyName, string storeName, string productName, int quantity, decimal cost, DateTime deliveryDate)
        {
            int companyId = GetCompanyId(companyName);
            int storeId = GetStoreId(storeName);
            int productId = GetProductId(productName);

            if (companyId != -1 && storeId != -1 && productId != -1)
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db"))
                {
                    conn.Open();

                    // Получение стоимости за единицу товара (Price)
                    decimal unitPrice = GetUnitPrice(productId);

                    // Вычисление общей стоимости поставки
                    decimal totalCost = unitPrice * quantity;

                    string query = "INSERT INTO Deliveries (Delivery_date, Cost, Company_ID, Store_ID) VALUES (@DeliveryDate, @Cost, @CompanyID, @StoreID)";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DeliveryDate", deliveryDate);
                        cmd.Parameters.AddWithValue("@Cost", totalCost); // Используем общую стоимость
                        cmd.Parameters.AddWithValue("@CompanyID", companyId);
                        cmd.Parameters.AddWithValue("@StoreID", storeId);

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Поставка успешно добавлена в базу данных!");
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при добавлении поставки в базу данных!");
                        }
                    }

                    int deliveryId = GetLastInsertedDeliveryId(conn);

                    if (deliveryId != -1)
                    {
                        query = "INSERT INTO ProductsInDeliveries (Delivery_ID, Product_ID, Quantity) VALUES (@DeliveryID, @ProductID, @Quantity)";

                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@DeliveryID", deliveryId);
                            cmd.Parameters.AddWithValue("@ProductID", productId);
                            cmd.Parameters.AddWithValue("@Quantity", quantity);

                            int resultProduct = cmd.ExecuteNonQuery();

                            if (resultProduct > 0)
                            {
                                MessageBox.Show("Товар успешно добавлен в поставку!");
                            }
                            else
                            {
                                MessageBox.Show("Ошибка при добавлении товара в поставку!");
                            }
                        }
                    }

                    conn.Close();
                }
            }
            else
            {
                MessageBox.Show("Ошибка при получении ID из базы данных!");
            }
        }

        private decimal GetUnitPrice(int productId)
        {
            decimal unitPrice = -1;

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db"))
            {
                conn.Open();
                string query = "SELECT Price FROM Products WHERE Product_ID = @ProductId";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        unitPrice = Convert.ToDecimal(result);
                    }
                }

                conn.Close();
            }

            return unitPrice;
        }


        private int GetLastInsertedDeliveryId(SQLiteConnection conn)
        {
            int deliveryId = -1;

            string query = "SELECT last_insert_rowid()";

            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    deliveryId = Convert.ToInt32(result);
                }
            }

            return deliveryId;
        }

        private int GetCompanyId(string companyName)
        {
            int companyId = -1;

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db"))
            {
                conn.Open();

                string query = "SELECT Company_ID FROM Companies WHERE Name = @CompanyName";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyName", companyName);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        companyId = Convert.ToInt32(result);
                    }
                }

                conn.Close();
            }

            return companyId;
        }

        private int GetStoreId(string storeName)
        {
            int storeId = -1;

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db"))
            {
                conn.Open();

                string query = "SELECT Store_ID FROM Stores WHERE Name = @StoreName";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StoreName", storeName);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        storeId = Convert.ToInt32(result);
                    }
                }

                conn.Close();
            }

            return storeId;
        }

        private int GetProductId(string productName)
        {
            int productId = -1;

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db"))
            {
                conn.Open();

                string query = "SELECT Product_ID FROM Products WHERE Name = @ProductName";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductName", productName);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        productId = Convert.ToInt32(result);
                    }
                }

                conn.Close();
            }

            return productId;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
