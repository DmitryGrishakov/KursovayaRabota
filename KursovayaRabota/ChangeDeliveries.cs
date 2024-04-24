using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace KursovayaRabota
{
    public partial class ChangeDeliveries : Form
    {
        private string oldCompanyName;
        private string oldStoreName;
        private DateTime oldDeliveryDate;

        public ChangeDeliveries(string companyName, string storeName, string productName, int quantity, decimal cost, DateTime deliveryDate)
        {
            InitializeComponent();

            // Присвоение переданных значений элементам управления
            comboBox1.Text = companyName;
            comboBox2.Text = storeName;
            comboBox3.Text = productName;
            textBox1.Text = quantity.ToString();
            textBox2.Text = cost.ToString();
            dateTimePicker1.Value = deliveryDate;

            // Сохранение старых значений
            oldCompanyName = companyName;
            oldStoreName = storeName;
            oldDeliveryDate = deliveryDate;
        }

        private void ChangeDeliveries_Load(object sender, EventArgs e)
        {
            LoadCompanies();
            LoadStores();
            LoadProducts();
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

        private int GetCompanyId(string companyName)
        {
            int companyId = -1;

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db"))
            {
                conn.Open();

                string query = "SELECT Company_ID FROM Companies WHERE Name = @CompanyName";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyName", oldCompanyName); 

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

        private int GetCompanyIdNew(string companyName)
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
                    cmd.Parameters.AddWithValue("@StoreName", oldStoreName); 
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

        private int GetStoreIdNew(string storeName)
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

        private int GetDeliveryIdUsingOldData()
        {
            int companyId = GetCompanyId(oldCompanyName);
            int storeId = GetStoreId(oldStoreName);

            if (companyId != -1 && storeId != -1)
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db"))
                {
                    conn.Open();

                    string query = "SELECT Delivery_ID, Company_ID, Store_ID, Delivery_date FROM Deliveries WHERE Company_ID = @CompanyID AND Store_ID = @StoreID AND Delivery_date = @DeliveryDate";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CompanyID", companyId);
                        cmd.Parameters.AddWithValue("@StoreID", storeId);
                        cmd.Parameters.AddWithValue("@DeliveryDate", oldDeliveryDate.ToString("yyyy-MM-dd"));

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Возвращаем Delivery_ID, Company_ID, Store_ID и Delivery_date
                                return Convert.ToInt32(reader["Delivery_ID"]);
                            }
                        }
                    }

                    conn.Close();
                }
            }

            return -1;
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

        private void UpdateDeliveryData(string companyName, string storeName, string productName, int quantity, decimal cost, DateTime deliveryDate)
        {
            int oldDeliveryId = GetDeliveryIdUsingOldData();

            if (oldDeliveryId != -1)
            {
                int companyId = GetCompanyId(companyName);
                int storeId = GetStoreId(storeName);
                int productId = GetProductId(productName);

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=D:\\Курсовая работа\\TradingCompanies.db"))
                {
                    conn.Open();

                    // Получение новых Company_ID и Store_ID
                    int newCompanyId = GetCompanyIdNew(companyName);
                    int newStoreId = GetStoreIdNew(storeName);

                    // Обновление данных в таблице поставок
                    string updateDeliveriesQuery = "UPDATE Deliveries SET Cost = @cost, Store_ID = @storeID, Company_ID = @companyID, Delivery_date = @deliveryDate WHERE Delivery_ID = @deliveryID";

                    using (SQLiteCommand cmdDeliveries = new SQLiteCommand(updateDeliveriesQuery, conn))
                    {
                        cmdDeliveries.Parameters.AddWithValue("@cost", cost);
                        cmdDeliveries.Parameters.AddWithValue("@storeID", newStoreId); // Используем новый Store_ID
                        cmdDeliveries.Parameters.AddWithValue("@companyID", newCompanyId); // Используем новый Company_ID
                        cmdDeliveries.Parameters.AddWithValue("@deliveryDate", deliveryDate.ToString("yyyy-MM-dd"));
                        cmdDeliveries.Parameters.AddWithValue("@deliveryID", oldDeliveryId);

                        int resultDeliveries = cmdDeliveries.ExecuteNonQuery();

                        if (resultDeliveries > 0)
                        {
                            // Обновление данных в таблице товаров в поставках
                            string updateProductsInDeliveriesQuery = "UPDATE ProductsInDeliveries SET Product_ID = @productID, Quantity = @quantity WHERE Delivery_ID = @deliveryID";

                            using (SQLiteCommand cmdProductsInDeliveries = new SQLiteCommand(updateProductsInDeliveriesQuery, conn))
                            {
                                cmdProductsInDeliveries.Parameters.AddWithValue("@productID", productId);
                                cmdProductsInDeliveries.Parameters.AddWithValue("@quantity", quantity);
                                cmdProductsInDeliveries.Parameters.AddWithValue("@deliveryID", oldDeliveryId);

                                int resultProductsInDeliveries = cmdProductsInDeliveries.ExecuteNonQuery();

                                if (resultProductsInDeliveries > 0)
                                {
                                    MessageBox.Show("Данные успешно обновлены!");
                                }
                                else
                                {
                                    MessageBox.Show("Ошибка при обновлении данных товаров в поставках!");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при обновлении данных поставок!");
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

            decimal calculatedCost = CalculateTotalCost(productName, quantity);
            DateTime deliveryDate = dateTimePicker1.Value;

            UpdateDeliveryData(companyName, storeName, productName, quantity, calculatedCost, deliveryDate);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
