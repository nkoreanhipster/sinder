using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Sinder
{
    /// <summary>
    /// Singleton class for database
    /// </summary>
    public sealed class Dataprovider
    {
        private static Dataprovider instance = null;
        private static readonly object padlock = new object();
        private static MySqlConnection con = new MySqlConnection(Helper.GetConnectionString());

        /// To ensure it's thread safe and constructor only returns a single instance; null check and lock to enforce a single thread
        public static Dataprovider Instance { get { lock (padlock) { return instance ?? new Dataprovider(); } } }
        public MySqlConnection CreateDBConnection() => con;

        #region Methods
        /// <summary>
        /// Test if connection works
        /// </summary>
        /// <returns></returns>
        public bool TestConnection()
        {
            bool isSuccess = true;
            try
            {
                MySqlConnection c = CreateDBConnection();
                c.Open();
                string query = "SELECT Value FROM Log;";
                MySqlCommand cmd = new MySqlCommand(query, c);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    System.Diagnostics.Debug.WriteLine(rdr[0] + " -- ");
                }
                c.Close();
            }
            catch (Exception e)
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        public async Task RegisterNewUser(UserModel user)
        {
            using (var connection = CreateDBConnection())
            {
                await connection.ExecuteAsync($"INSERT INTO Users (Email, Firstname, Surname, Location, Age, HashedPassword, Salt, Gender) " +
                    $"Values (@Email, @Firstname, @Surname, @Location, @Age, @HashedPassword, @Salt, @Gender);", user);
            }
        }

        public async Task<List<UserModel>> ReadUsers(string email)
        {
            using(var connection = CreateDBConnection())
            {
                var users = (await connection.QueryAsync<UserModel>("SELECT * FROM Users WHERE @Email = Users.Email", new { Email = email })).ToList();
                return users;
            }
        }
        #endregion
        public async Task<List<UserModel>> SearchUser(string searchString)

        {
            using (var connection = CreateDBConnection())
            {

                string sql = (@"SELECT * FROM Users WHERE Name LIKE @searchQuery,"+ searchString);
                var result = await connection.QueryAsync<UserModel>(sql, new { searchQuery = searchString });
                return result.ToList();
            }
        }
    }
}

