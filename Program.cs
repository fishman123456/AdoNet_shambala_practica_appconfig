using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Configuration;
using System.Data;
using System.Diagnostics.Metrics;
using System.Xml.Linq;

namespace ConsoleApp1
{

    internal class Program
    {

        // 1. вспомогательная процедура, создающая и открывающая подключение к БД
        static SqlConnection OpenDbConnection()
        {
            //string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=usersdb;Integrated Security=True";
            // получаем строку подключения
            // подключение через app.config 30-09-2023 3:18
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //Console.WriteLine(connectionString);
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;


            // обработка исключений будет выполняться выше по стеку
            /*string connectionString = @"Data Source=fishman\SQLEXPRESS;
                                    Initial Catalog=Practica_3_db;
                                    Integrated Security=SSPI;";
            Console.WriteLine(connectionString);
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
            */
        }
        // 2. вспомогательная процедура, читающая и выводящая табличный результат запроса (SqlDatareader)
        static void ReadQueryResult(SqlDataReader queryResult)
        {
            // 1. вывести названия столбцов результирующей таблицы (представления)
            for (int i = 0; i < queryResult.FieldCount; i++)
            {
                Console.Write($"{queryResult.GetName(i)}" + "\t");
            }
            //Console.Write(queryResult.GetName(queryResult.FieldCount - 1));
            // 2. вывести значения построчно
            Console.WriteLine();
            while (queryResult.Read())
            {
                Console.WriteLine();
                for (int i = 0; i < queryResult.FieldCount; i++)
                {

                    Console.Write($"{queryResult[i]}" + "\t");
                }

            }
            Console.WriteLine();
        }

        // 3. процедура получения всех записей таблицы client
        static void SelectAllRowsClient()
        {
            SqlConnection connection = null;
            SqlDataReader queryResult = null;
            try
            {
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос
                SqlCommand query = new SqlCommand($"SELECT * FROM Client_t", connection);
                //, Order_t where Client_t.id_f = Order_t.id_f
                // 3. выполнить запрос с табличным результом
                queryResult = query.ExecuteReader();
                // 4. считать запрос (универсальный способ)
                ReadQueryResult(queryResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
                queryResult?.Close();
            }
        }
        // 3. процедура получения всех записей таблицы order
        static void SelectAllRowsClientOrder()
        {
            SqlConnection connection = null;
            SqlDataReader queryResult = null;
            try
            {
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос
                SqlCommand query = new SqlCommand($"SELECT * FROM " +
                    $"Client_t, Order_t where Client_t.id_f = Order_t.client_id_f " +
                    $"order by Client_t.id_f", connection);

                // 3. выполнить запрос с табличным результом
                queryResult = query.ExecuteReader();
                // 4. считать запрос (универсальный способ)
                ReadQueryResult(queryResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
                queryResult?.Close();
            }
        }

        // 4. процедура получения записи по id
        static void SelectRowById(int id)
        {
            SqlConnection connection = null;
            SqlDataReader queryResult = null;
            try
            {
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос
                // поля Client_t    1)id_f    2)name_f          3)age_f
                // поля Order_t     1)id_f    2)description_f   3)client_id
                SqlCommand query = new SqlCommand($"SELECT * FROM Order_t WHERE id_f = @id", connection);
                // query.Parameters.Add("@p", SqlDbType.BigInt).Value = id;
                query.Parameters.AddWithValue("@id", id);
                // 3. выполнить запрос с табличным результом
                queryResult = query.ExecuteReader();
                // 4. считать запрос (универсальный способ)
                ReadQueryResult(queryResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
                queryResult?.Close();
            }
        }
        // 5. процедура добавления новой записи в таблицу client
        static void InsertRow_client(string name, string age)
        {
            SqlConnection connection = null;
            try
            {
                // имена client_t [id_f],[name_f],[age_f]
                // имена Order_t  [id_f],[description_f],[client_id]
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос [name_f] ,[released_in_f]   ,[prise_f]
                string cmdString =
                    $"INSERT INTO Client_t (name_f, age_f) VALUES (@name_f, @age_f);";
                SqlCommand cmd = new SqlCommand(cmdString, connection);
                cmd.Parameters.AddWithValue("@name_f", DbType.String).Value = name;
                cmd.Parameters.AddWithValue("@age_f", DbType.String).Value = age;
                // 3. выполнить запрос
                int rowsAffected = cmd.ExecuteNonQuery();   // выполнение запроса, изменяющего строки таблицы
                                                            // 4. проверить результат выполнения
                if (rowsAffected != 1)
                {
                    Console.WriteLine($"INSERT failed, rowsAffected != 1 ({rowsAffected})");
                }
                else
                {
                    Console.WriteLine("Successfully inserted");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
            }
        }
        // 5. процедура добавления новой записи в таблицу Order_t
        static void InsertRow_order(string description_f, int client_id_f)
        {
            SqlConnection connection = null;
            try
            {
                // имена client_t [id_f],[name_f],[age_f]
                // имена Order_t  [id_f],[description_f],[client_id]
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос [name_f] ,[released_in_f]   ,[prise_f]
                string cmdString =
                    $"INSERT INTO Order_t (description_f, client_id_f) VALUES (@description_f, @client_id_f);";
                SqlCommand cmd = new SqlCommand(cmdString, connection);
                cmd.Parameters.AddWithValue("@description_f", DbType.String).Value = description_f;
                cmd.Parameters.AddWithValue("@client_id_f", DbType.Int16).Value = client_id_f;
                // 3. выполнить запрос
                int rowsAffected = cmd.ExecuteNonQuery();   // выполнение запроса, изменяющего строки таблицы
                                                            // 4. проверить результат выполнения
                if (rowsAffected != 1)
                {
                    Console.WriteLine($"INSERT failed, rowsAffected != 1 ({rowsAffected})");
                }
                else
                {
                    Console.WriteLine("Successfully inserted");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
            }
        }

        // 6. процедура удаления записи из таблицы
        static void DeleteRowClient(int id)
        {
            //ALTER TABLE Order_t
            //ADD CONSTRAINT fk_client_id_f
            //FOREIGN KEY(client_id_f)
            //REFERENCES Client_t(id_f)
            //ON DELETE CASCADE;
            SqlConnection connection = null;
            try
            {
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос
                string cmdString =
                    $"DELETE from Client_t where id_f ={id};";
                SqlCommand cmd = new SqlCommand(cmdString, connection);
               
                // 3. выполнить запрос
                int rowsAffected = cmd.ExecuteNonQuery();   // выполнение запроса, изменяющего строки таблицы
                                                            // 4. проверить результат выполнения
                if (rowsAffected != 1)
                {
                    Console.WriteLine($"DELETE failed, rowsAffected != 1 ({rowsAffected})");
                }
                else
                {
                    Console.WriteLine("Successfully deleted");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
            }
        }
        // 6. процедура удаления записи из таблицы
        static void DeleteRowOrder(int id)
        {
            //ALTER TABLE Order_t
            //ADD CONSTRAINT fk_client_id_f
            //FOREIGN KEY(client_id_f)
            //REFERENCES Client_t(id_f)
            //ON DELETE CASCADE;
            SqlConnection connection = null;
            try
            {
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос
                string cmdString =
                    $"DELETE from Order_t where id_f ={id};";
                SqlCommand cmd = new SqlCommand(cmdString, connection);

                // 3. выполнить запрос
                int rowsAffected = cmd.ExecuteNonQuery();   // выполнение запроса, изменяющего строки таблицы
                                                            // 4. проверить результат выполнения
                if (rowsAffected != 1)
                {
                    Console.WriteLine($"DELETE failed, rowsAffected != 1 ({rowsAffected})");
                }
                else
                {
                    Console.WriteLine("Successfully deleted");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
            }
        }
        // 7. процедура изменения записи в таблице
        static void UpdateRowClient(int id, string newName, int newAge)
        {
            SqlConnection connection = null;
            try
            {
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос
                // поля Client_t    1)id_f    2)name_f          3)age_f
                // поля Order_t     1)id_f    2)description_f   3)client_id
                string cmdString =
                    $"update Client_t set name_f = @name_f," +
                    $"age_f = @age_f " +
                    $"where id_f ={id};";
                SqlCommand cmd = new SqlCommand(cmdString, connection);
                cmd.Parameters.AddWithValue("@name_f", DbType.String).Value = newName;
                cmd.Parameters.AddWithValue("@age_f", DbType.String).Value = newAge;
                // 3. выполнить запрос
                int rowsAffected = cmd.ExecuteNonQuery();   // выполнение запроса, изменяющего строки таблицы
                                                            // 4. проверить результат выполнения
                if (rowsAffected != 1)
                {
                    Console.WriteLine($"Update failed, rowsAffected != 1 ({rowsAffected})");
                }
                else
                {
                    Console.WriteLine("Successfully update");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
                Console.WriteLine("Connections close");
            }
        }

        static void UpdateRowOrder(int id_f, string description_f)
        {
            SqlConnection connection = null;
            try
            {
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос
                // поля Client_t    1)id_f    2)name_f          3)age_f
                // поля Order_t     1)id_f    2)description_f   3)client_id
                string cmdString =
                    $"update Order_t set description_f = @description_f " +
                   // $"client_id = @client_id " +
                    $"where id_f ={id_f};";
                SqlCommand cmd = new SqlCommand(cmdString, connection);
                cmd.Parameters.AddWithValue("@description_f", DbType.String).Value = description_f;
                //cmd.Parameters.AddWithValue("@client_id", DbType.String).Value = client_id;
                // 3. выполнить запрос
                int rowsAffected = cmd.ExecuteNonQuery();   // выполнение запроса, изменяющего строки таблицы
                                                            // 4. проверить результат выполнения
                if (rowsAffected != 1)
                {
                    Console.WriteLine($"Update failed, rowsAffected != 1 ({rowsAffected})");
                }
                else
                {
                    Console.WriteLine("Successfully update");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
                Console.WriteLine("Connections close");
            }
        }
        static void Main(string[] arg)
        {
            //InsertRow_order("солянка",4);
            //InsertRow_client("михаил600", "600");
            //SelectRowById(1);
            //DeleteRowOrder(9);
            //DeleteRowOrder(10);
            //DeleteRow(5);
           
            SelectAllRowsClient();
            //UpdateRowClient(2, "Сергей", 528);
            SelectAllRowsClientOrder();
             UpdateRowOrder(2, "картошка жареная");
            SelectAllRowsClientOrder();

        }
    }
}
