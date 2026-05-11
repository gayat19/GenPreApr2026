using Npgsql;
using System.Data;
using System.Net.NetworkInformation;

namespace UnderstandingADOApp
{
    
    internal class Program
    {
        string connectionString =
            "Host=localhost;Port=5432;Database=dummydb;Username=postgres;Password=password";
        NpgsqlConnection connection;
        public Program()
        {
          connection = new NpgsqlConnection(connectionString);
           
        }
        void GetProductDataFromDatabase()
        {
            string selectQuery = "Select * from Products";
            NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection);
            try
            {
                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("Product Id : " + reader[0].ToString());
                    Console.WriteLine("Product Name : " + reader[1].ToString());
                }
                Console.WriteLine("Done reading");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection?.Close();
            }

        }

        void InsertUserInToDatabase()
        {
            User user = GetUserDataFromConsole();
            string insertCmd = $"Insert into Users values('{user.Username}','{user.Password}','{user.Role}')";
            NpgsqlCommand command = new NpgsqlCommand(insertCmd, connection);
            try
            {
                connection.Open();
                int result = command.ExecuteNonQuery();
                if(result>0)
                    Console.WriteLine("User created successfully");
            }
            catch(NpgsqlException ne)
            {
                throw new Exception("Duplicate username");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Source);
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection?.Close();
            }
        }
        void UpdateUserToDatabase(User user, string field)
        {
            string updateCmd = "";
            if (field == "password")
                updateCmd = $"update  Users set user_password = '{user.Password}' where user_id='{user.Username}'";
            else if (field == "role")
                updateCmd = $"update  Users set user_role = '{user.Role}' where user_id='{user.Username}'";
            else
                throw new Exception("Invalid column to update");

            NpgsqlCommand command = new NpgsqlCommand(updateCmd, connection);
            try
            {
                connection.Open();
                int result = command.ExecuteNonQuery();
                if (result > 0)
                    Console.WriteLine("User details updated");
                else
                    Console.WriteLine("No such user");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection?.Close();
            }
        }
        void InitiateUserDataUpdate()
        {
            Console.WriteLine("Please enter the user id you want to update");
            User user = new User();
            user.Username = Console.ReadLine() ?? "";
            Console.WriteLine("Please enter the field you want to update Password/Role");
            
            string option = Console.ReadLine()??"".ToLower();
            if(option =="role")
            {
                Console.WriteLine("Please enter the new value for role");
                user.Role = Console.ReadLine() ?? "";
            }
            else if(option == "password")
            {
                Console.WriteLine("Please enter the new value for password");
                user.Password = Console.ReadLine() ?? "";
            }
            try
            {
                UpdateUserToDatabase(user, option);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        private User GetUserDataFromConsole()
        {
            User user = new User();
            Console.WriteLine("Please eneter your preffered username");
            user.Username = Console.ReadLine()??"";
            Console.WriteLine("Please eneter teh password");
            user.Password = Console.ReadLine()??"";
            Console.WriteLine("Please eneter your role");
            user.Role = Console.ReadLine() ?? "";
            return user;

        }

        void ReadDataFromDatabaseUsingDiscon()
        {
            string selectQuery = "Select * from Products";
            //NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection);
            try
            {
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(selectQuery,connection);
                DataSet dataSet = new DataSet();
                da.Fill(dataSet, "Products");//Conenct to the database-> execute the query->fetch the data-> put in dataset-> disconnect
                Console.WriteLine(connection.State);
                foreach (DataRow dr in dataSet.Tables[0].Rows)
                {
                    Console.WriteLine("Product Id : " + dr[0].ToString());
                    Console.WriteLine("Product Name : " + dr[1].ToString());
                }
                Console.WriteLine("Done reading");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection?.Close();
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
           try
            {
                new Program().InsertUserInToDatabase();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }


    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
