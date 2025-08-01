using Exchange_appl.Models;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Exchange_appl
{
    public class DatabaseManager
    {
        private static string connectionString = "server=localhost;user=root;password=;database=DB_exchange;";

        // Глобальне поле для зберігання ідентифікатора авторизованого користувача
        public static int? CurrentUserId { get; set; } 


        // Публічне властивість для отримання рядка підключення
        public string ConnectionString => connectionString;


        // Метод для добавления пользователя
        public void AddUser(string username, string password)
        {
            if (UserExistsByUsername(username))
            {
                throw new Exception("Користувач із таким логіном вже існує!");
            }

            string hashedPassword = HashPassword(password);
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var command = new MySqlCommand("INSERT INTO Users (username, password) VALUES (@username, @hashedPassword)", conn);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@hashedPassword", hashedPassword);
            command.ExecuteNonQuery();
        }



        // Метод для аутентификации пользователя
        public bool AuthenticateUser(string username, string password)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var command = new MySqlCommand("SELECT Id, password FROM Users WHERE username = @username", conn);
            command.Parameters.AddWithValue("@username", username);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                int userId = reader.GetInt32("Id");
                string storedPasswordHash = reader.GetString("password");

                string hashedPassword = HashPassword(password); // Хешування введеного пароля
                if (storedPasswordHash == hashedPassword) // Порівняння хешів
                {
                    CurrentUserId = userId; // Запам'ятовуємо Id користувача при успішному вході
                    return true;
                }
            }
            return false;
        }


        // Метод для хеширования пароля
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));

            var builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        public List<User> GetUsers()
        {
            var users = new List<User>();

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            var command = new MySqlCommand("SELECT * FROM Users", conn);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32("Id"),
                    Username = reader.GetString("Username"),
                    Password = reader.GetString("Password"),
                    // Добавьте другие поля, если нужно
                });
            }

            return users;
        }

        // Проверка, существует ли пользователь по логину
        public bool UserExistsByUsername(string username)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var command = new MySqlCommand("SELECT COUNT(*) FROM Users WHERE username = @username", conn);
            command.Parameters.AddWithValue("@username", username);
            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }

        // Метод для получения всех пользователей
        public List<Models.User> GetAllUsers()
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var command = new MySqlCommand("SELECT Id, Username, Password FROM Users", conn);
            using var reader = command.ExecuteReader();
            var users = new List<Models.User>();
            while (reader.Read())
            {
                users.Add(new Models.User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2)
                });
            }
            return users;
        }

        // Метод для удаления пользователя
        public void DeleteUser(int userId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var command = new MySqlCommand("DELETE FROM Users WHERE Id = @userId", conn);
            command.Parameters.AddWithValue("@userId", userId);
            command.ExecuteNonQuery();
        }

        // Метод для получения всех предметов пользователя
        public List<Models.Item> GetAllUserItems(int userId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var command = new MySqlCommand("SELECT * FROM Items WHERE user_id = @userId", conn);
            command.Parameters.AddWithValue("@userId", userId);

            using var reader = command.ExecuteReader();
            var items = new List<Models.Item>();
            while (reader.Read())
            {
                items.Add(new Models.Item
                {
                    Id = reader.GetInt32("Id"),
                    UserId = reader.GetInt32("user_id"), // Убедитесь, что используется точное имя столбца в соответствии с базой данных
                    ItemName = reader.GetString("item_name"),
                    Description = reader.GetString("description"),
                    Category = reader.GetString("category"),
                    ExchangeCategory = reader.GetString("exchange_category"),  // Отримуємо значення категорії обміну
                    Image = reader.GetString("Image"),
                    AuthorPhone = GetPhoneNumberById(reader.GetInt32("user_id")) // Отримуємо номер телефону автора
                });
            }
            return items;
        }



        // Метод для удаления предмета
        public void DeleteItem(int itemId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var command = new MySqlCommand("DELETE FROM Items WHERE Id = @itemId", conn);
            command.Parameters.AddWithValue("@itemId", itemId);
            command.ExecuteNonQuery();
        }

        // Метод для загрузки всех предметов из базы данных
        public ObservableCollection<Models.Item> GetAllItems()
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var command = new MySqlCommand("SELECT * FROM Items", conn);
            using var reader = command.ExecuteReader();
            var items = new ObservableCollection<Models.Item>();
            while (reader.Read())
            {
                items.Add(new Models.Item
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    ItemName = reader.GetString(2),
                    Description = reader.GetString(3),
                    Category = reader.GetString(4),
                    Image = reader.GetString(5),
                    ExchangeCategory = reader.GetString(6),
                    AuthorPhone = GetPhoneNumberById(reader.GetInt32(1))
                });
            }
            return items;
        }


        // Метод для сохранения пользователей в XML
        public void SaveUsersToXml(string filePath)
        {
            var users = GetAllUsers();
            var serializer = new XmlSerializer(typeof(List<Models.User>));
            using var writer = new StreamWriter(filePath);
            serializer.Serialize(writer, users);
        }

        // Метод для загрузки пользователей из XML
        public static List<Models.User> LoadUsersFromXml(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<Models.User>();

            var serializer = new XmlSerializer(typeof(List<Models.User>));
            using var reader = new StreamReader(filePath);
            var users = serializer.Deserialize(reader) as List<Models.User>;
            return users ?? new List<Models.User>();
        }

        // Метод для сохранения предметов в XML
        public void SaveItemsToXml(string filePath)
        {
            var items = GetAllItems();
            var serializer = new XmlSerializer(typeof(List<Models.Item>));
            using var writer = new StreamWriter(filePath);
            serializer.Serialize(writer, items);
        }

        // Метод для загрузки предметов из XML
        public static List<Models.Item> LoadItemsFromXml(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<Models.Item>();

            var serializer = new XmlSerializer(typeof(List<Models.Item>));
            using var reader = new StreamReader(filePath);
            var items = serializer.Deserialize(reader) as List<Models.Item>;
            return items ?? new List<Models.Item>();
        }

       
        // Метод для очищення Id користувача при виході
        public void Logout()
        {
            CurrentUserId = null;
        }


        // Метод для отримання даних користувача за ідентифікатором
        public string GetPhoneNumberById(int userId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var command = new MySqlCommand("SELECT username FROM Users WHERE Id = @userId", conn);
            command.Parameters.AddWithValue("@userId", userId);

            var result = command.ExecuteScalar() as string;
            return result ?? string.Empty; // Повертаємо порожній рядок, якщо результат null
        }

        // Метод для получения идентификатора пользователя по логину
        public int GetUserIdByUsername(string username)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var command = new MySqlCommand("SELECT Id FROM Users WHERE username = @username", conn);
            command.Parameters.AddWithValue("@username", username);

            var result = command.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : throw new Exception("Користувача не знайдено!");
        }

        public Models.User GetUserById(int userId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            var command = new MySqlCommand("SELECT Id, Username FROM Users WHERE Id = @userId", conn);
            command.Parameters.AddWithValue("@userId", userId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Models.User
                {
                    Id = reader.GetInt32("Id"),
                    Username = reader.GetString("Username")
                };
            }

            throw new Exception("Пользователь с указанным идентификатором не найден.");
        }


        public void UpdateItem(Models.Item item)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new MySqlCommand(
                    "UPDATE Items SET item_name = @Name, description = @Description, category = @Category, exchange_category = @ExchangeCategory, image = @Image WHERE id = @Id", conn);
                command.Parameters.AddWithValue("@Name", item.ItemName);
                command.Parameters.AddWithValue("@Description", item.Description);
                command.Parameters.AddWithValue("@Category", item.Category);
                command.Parameters.AddWithValue("@ExchangeCategory", item.ExchangeCategory);
                command.Parameters.AddWithValue("@Image", item.Image);
                command.Parameters.AddWithValue("@Id", item.Id);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateUser(Models.User user)
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new MySqlCommand(
                    "UPDATE Users SET password = @Password WHERE username = @Username", conn);

                command.Parameters.AddWithValue("@Password", user.Password); 
                command.Parameters.AddWithValue("@Username", user.Username);
                command.ExecuteNonQuery();
            }
        }



        public ObservableCollection<Models.Exchangeoffer> GetOffersToMe(int userId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            var command = new MySqlCommand(
                @"SELECT eo.id, eo.item_offered_id, eo.item_requested_id, eo.status,
          io.item_name AS offered_item_name,
          ir.item_name AS requested_item_name,
          u_offered.username AS sender_username,
          u_requested.username AS receiver_username
   FROM exchangeoffers eo
   JOIN items io ON eo.item_offered_id = io.id
   LEFT JOIN items ir ON eo.item_requested_id = ir.id
   LEFT JOIN users u_offered ON eo.sender_id = u_offered.id
   LEFT JOIN users u_requested ON eo.receiver_id = u_requested.id
   WHERE eo.receiver_id = @userId", conn);

            command.Parameters.AddWithValue("@userId", userId);

            using var reader = command.ExecuteReader();
            var offers = new ObservableCollection<Models.Exchangeoffer>();

            while (reader.Read())
            {
                offers.Add(new Models.Exchangeoffer
                {
                    Id = reader.GetInt32("id"),
                    ItemOffered = new Models.Item
                    {
                        ItemName = reader.GetString("offered_item_name")
                    },
                    // Обработка отсутствующего товара
                    ItemRequested = reader.IsDBNull("requested_item_name")
                        ? new Models.Item { ItemName = "No item requested", Id = 0 } // ID 0, если товара нет
                        : new Models.Item
                        {
                            ItemName = reader.GetString("requested_item_name"),
                            Id = reader.GetInt32("item_requested_id") // Устанавливаем правильный ID для requested_item
                        },
                    Status = reader.GetString("status"),
                    Sender = new Models.User
                    {
                        Username = reader.GetString("sender_username")
                    },
                    Receiver = new Models.User
                    {
                        Username = reader.GetString("receiver_username")
                    }
                });
            }

            return offers;
        }

        public ObservableCollection<Models.Exchangeoffer> GetMyOffers(int userId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            var command = new MySqlCommand(
                @"SELECT eo.id, 
                 eo.item_offered_id, 
                 eo.item_requested_id, 
                 eo.status,
                 io.item_name AS offered_item_name,
                 io.id AS offered_item_id,
                 ir.item_name AS requested_item_name,
                 ir.id AS requested_item_id,
                 u_offered.username AS sender_username,
                 u_requested.username AS receiver_username
          FROM exchangeoffers eo
          JOIN items io ON eo.item_offered_id = io.id
          LEFT JOIN items ir ON eo.item_requested_id = ir.id
          LEFT JOIN users u_offered ON eo.sender_id = u_offered.id
          LEFT JOIN users u_requested ON eo.receiver_id = u_requested.id
          WHERE eo.sender_id = @userId", conn);

            command.Parameters.AddWithValue("@userId", userId);

            using var reader = command.ExecuteReader();
            var offers = new ObservableCollection<Models.Exchangeoffer>();

            while (reader.Read())
            {
                offers.Add(new Models.Exchangeoffer
                {
                    Id = reader.GetInt32("id"),
                    ItemOffered = new Models.Item
                    {
                        Id = reader.GetInt32("offered_item_id"), // Убедитесь, что здесь заполняется ID
                        ItemName = reader.GetString("offered_item_name")
                    },
                    ItemRequested = reader.IsDBNull("requested_item_id") ? null : new Models.Item
                    {
                        Id = reader.GetInt32("requested_item_id"),
                        ItemName = reader.GetString("requested_item_name")
                    },
                    Status = reader.GetString("status"),
                    Sender = new Models.User
                    {
                        Username = reader.GetString("sender_username")
                    },
                    Receiver = new Models.User
                    {
                        Username = reader.GetString("receiver_username")
                    }
                });
            }

            return offers;
        }



        /*  public void CreateExchangeOffer(Models.Exchangeoffer offer)
          {
              using (var connection = new MySqlConnection(connectionString))
              {
                  connection.Open();

                  string query = @"
              INSERT INTO ExchangeOffers (item_offered_id, item_requested_id, receiver_id, status)
              VALUES (@ItemOfferedId, @ItemRequestedId, @ReceiverId, @Status)";
                  var command = new MySqlCommand(query, connection);
                  command.Parameters.AddWithValue("@ItemOfferedId", offer.ItemOfferedId);
                  command.Parameters.AddWithValue("@ItemRequestedId", offer.ItemRequestedId);
                  command.Parameters.AddWithValue("@ReceiverId", offer.ReceiverId);
                  command.Parameters.AddWithValue("@Status", offer.Status ?? "Очікує");
                  command.ExecuteNonQuery();
              }
          }
        */

        public ObservableCollection<Models.Item> GetApplicableItemsForExchange()
        {
            // Assuming you want to get items that are available for exchange based on some condition
            var allItems = GetAllItems(); // You can modify this to apply specific filtering if needed
            var applicableItems = new ObservableCollection<Models.Item>();

            foreach (var item in allItems)
            {
                // Example condition: item must have a specific exchange category
                if (!string.IsNullOrEmpty(item.ExchangeCategory))
                {
                    applicableItems.Add(item);
                }
            }

            return applicableItems;
        }

        // Метод для отримання елемента за його ідентифікатором
        public Models.Item GetItemById(int itemId)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            var command = new MySqlCommand("SELECT * FROM Items WHERE Id = @itemId", conn);
            command.Parameters.AddWithValue("@itemId", itemId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Models.Item
                {
                    Id = reader.GetInt32("Id"),
                    UserId = reader.GetInt32("user_id"),
                    ItemName = reader.GetString("item_name"),
                    Description = reader.GetString("description"),
                    Category = reader.GetString("category"),
                    ExchangeCategory = reader.GetString("exchange_category"),
                    Image = reader.GetString("Image"),
                    AuthorPhone = GetPhoneNumberById(reader.GetInt32("user_id"))
                };
            }

            throw new Exception("Елемент із вказаним ідентифікатором не знайдено.");
        }

        public void AddExchangeOffer(int itemOfferedId, int itemRequestedId, int senderId, int receiverId, string status)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            var command = new MySqlCommand(
                @"INSERT INTO exchangeoffers (item_offered_id, item_requested_id, sender_id, receiver_id, status) 
          VALUES (@itemOfferedId, @itemRequestedId, @senderId, @receiverId, @status)", conn);

            command.Parameters.AddWithValue("@itemOfferedId", itemOfferedId);
            command.Parameters.AddWithValue("@itemRequestedId", itemRequestedId);
            command.Parameters.AddWithValue("@senderId", senderId);
            command.Parameters.AddWithValue("@receiverId", receiverId);
            command.Parameters.AddWithValue("@status", status);

            command.ExecuteNonQuery();
        }

        public static void UpdateExchangeOffer(Exchangeoffer offer)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"UPDATE exchangeoffers 
                                 SET Status = @Status 
                                 WHERE Id = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Status", offer.Status);
                    command.Parameters.AddWithValue("@Id", offer.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Метод для удаления предложения
        public static void DeleteExchangeOffer(int offerId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"DELETE FROM exchangeoffers 
                                 WHERE Id = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", offerId);
                    command.ExecuteNonQuery();
                }
            }
        }


    }
}
