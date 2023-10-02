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
using System.Diagnostics;
using System.Drawing;

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

        // 3. процедура получения всех записей таблицы Client_t
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
        // 3. процедура получения всех записей таблицы Order_t
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

        // 4. процедура получения записи по id (Order_t)
        static void SelectRowByIdOrder(int id)
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

        static void SelectRowByIdClientOrder(int id)
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
                SqlCommand query = new SqlCommand($"SELECT * FROM Client_t t, Order_t o WHERE t.id_f = @id and o.client_id_f = @id", connection);
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
        // 5. процедура добавления новой записи в таблицу Client_t
        static void InsertRow_client(string name, int age)
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
                cmd.Parameters.AddWithValue("@age_f", DbType.Int16).Value = age;
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

        // 6. процедура удаления записи из таблицы Client_t
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
        // 6. процедура удаления записи из таблицы Order_t
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
        // 7. процедура изменения записи в таблице Client_t
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
                cmd.Parameters.AddWithValue("@age_f", DbType.Int16).Value = newAge;
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
        // 7. процедура изменения записи в таблице Order_t
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
        //9. процедура добавления 5 клиентов (Client_t) и от 2 до 5 заказов (Order_t) каждому из них (на C#)
        static void ProcedureCreateClientOrder()
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
                                   $"Alter PROC ProcAddClientOrder AS " +
                                   $"BEGIN " +
                                   $"INSERT INTO Client_t (name_f, age_f) VALUES (\'юрмала\', 25) " +
                                    $"INSERT INTO Client_t (name_f, age_f) VALUES (\'ирина\', 65) " +
                                     $"INSERT INTO Client_t (name_f, age_f) VALUES (\'алина\', 40) " +
                                      $"INSERT INTO Client_t (name_f, age_f) VALUES (\'марина\', 75) " +
                                       $"INSERT INTO Client_t (name_f, age_f) VALUES (\'стас\', 15) " +
                                   $"INSERT INTO Order_t (description_f, client_id_f) VALUES (\'каша\', 23) " +
                                   $"INSERT INTO Order_t (description_f, client_id_f) VALUES (\'суп\', 23) " +
                                   $"INSERT INTO Order_t (description_f, client_id_f) VALUES (\'пирожек с капустой\', 23) " +
                                   $"INSERT INTO Order_t (description_f, client_id_f) VALUES (\'бифштекс\', 24) " +
                                   $"INSERT INTO Order_t (description_f, client_id_f) VALUES (\'суп грибной\', 24) " +
                                   $"INSERT INTO Order_t (description_f, client_id_f) VALUES (\'куриный рулет\', 24) " +
                                   $"INSERT INTO Order_t (description_f, client_id_f) VALUES (\'омлет с сыром\', 25) " +
                                   $"INSERT INTO Order_t (description_f, client_id_f) VALUES (\'бурито\', 25) " +
                                   $"INSERT INTO Order_t (description_f, client_id_f) VALUES (\'салат коктейль\', 25) " +
                                   $"INSERT INTO Order_t (description_f, client_id_f) VALUES (\'курица в лаваше\', 26) " +
                                    $"INSERT INTO Order_t (description_f, client_id_f) VALUES (\'уха из горбуши\', 26) " +
                                     $"INSERT INTO Order_t (description_f, client_id_f) VALUES (\'салат мимоза\', 27) " +
                                      $"INSERT INTO Order_t (description_f, client_id_f) VALUES (\'салат ZA президента\', 27) " +
                                       $"INSERT INTO Order_t (description_f, client_id_f) VALUES (\'Карбонат Пари\', 27) " +
                                   $"END";


                    //$"INSERT INTO Client_t (name_f, age_f) VALUES (@name_f, @age_f);";
                SqlCommand cmd = new SqlCommand(cmdString, connection);
                //cmd.Parameters.AddWithValue("@name_f", DbType.String).Value = name;
                //cmd.Parameters.AddWithValue("@age_f", DbType.Int16).Value = age;
                //cmd.Parameters.AddWithValue("@description_f", DbType.String).Value = description_f;
                //cmd.Parameters.AddWithValue("@client_id_f", DbType.Int16).Value = client_id_f;
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
        //9. запуск процедуры добавления 5 клиентов и от 2 до 5 заказов каждому из них (на C#)
        static void ProcedureRunClientOrder()
      
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
                                   $"EXEC ProcAddClientOrder";

                //$"INSERT INTO Client_t (name_f, age_f) VALUES (@name_f, @age_f);";
                SqlCommand cmd = new SqlCommand(cmdString, connection);
                //cmd.Parameters.AddWithValue("@name_f", DbType.String).Value = name;
                //cmd.Parameters.AddWithValue("@age_f", DbType.Int16).Value = age;
                //cmd.Parameters.AddWithValue("@description_f", DbType.String).Value = description_f;
                //cmd.Parameters.AddWithValue("@client_id_f", DbType.Int16).Value = client_id_f;
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

        static void Main(string[] arg)
        {
            //InsertRow_order("солянка",4);
            //InsertRow_client("марина", 25);
            //InsertRow_client("сергей", 355);
            //InsertRow_client("илья", 50);
            //InsertRow_client("света", 45);
            //InsertRow_client("александр", 32);
            //InsertRow_client("полина", 50);
            //InsertRow_order("солянка", 1);
            // InsertRow_order("борщ", 2);
            //InsertRow_order("картошка жареная", 3);
            //InsertRow_order("салат министерский", 4);
            //InsertRow_order("солянка", 1);
            //InsertRow_order("солянка", 1);
            //InsertRow_order("солянка", 1);
            //InsertRow_order("солянка", 1);
            //InsertRow_order("солянка", 1);
            //InsertRow_order("солянка", 1);
            //InsertRow_order("солянка", 1);
            //InsertRow_order("солянка", 1);
            //InsertRow_order("солянка", 1);
            //SelectRowByIdOrder(1);
            //DeleteRowOrder(9);
            //DeleteRowOrder(10);
            //for (int i = 28; i < 38; i++)
            //{
            // DeleteRowClient(i);
            //}
            // DeleteRowClient(4);
            //SelectRowByIdClientOrder(4);
            SelectAllRowsClient();
            SelectAllRowsClientOrder();
            //UpdateRowClient(2, "Сергей", 528);
            //SelectAllRowsClientOrder();
            //UpdateRowOrder(2, "картошка жареная");
            //SelectAllRowsClientOrder();
            //ProcedureCreateClientOrder();
           // ProcedureRunClientOrder();
        }
    }
}
