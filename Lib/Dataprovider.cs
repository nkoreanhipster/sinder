using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        public async Task<UserModel> ReadUserById(int id)
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<UserModel>("SELECT * FROM Users WHERE Users.ID = @Id", new { Id = id })).First();
            }
        }
        public async Task<UserModel> ReadUserByEmail(string email) => (await ReadUsersByEmail(email)).First();

        public async Task<List<UserModel>> ReadUsersByEmail(string email)
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<UserModel>("SELECT * FROM Users WHERE @Email = Users.Email", new { Email = email })).ToList();
            }
        }

        public async Task<List<UserModel>> SearchUsersBy(string searchString)
        {
            using (var connection = CreateDBConnection())
            {
                //searchString = $"%{searchString}%";
                string sql = (@"SELECT * 
                    FROM Users
                    WHERE (Firstname LIKE CONCAT('%', @searchQuery, '%')) 
                    OR (Surname LIKE CONCAT('%', @searchQuery, '%'));");
                var result = await connection.QueryAsync<UserModel>(sql, new { searchQuery = searchString });
                return result.ToList();
            }
        }

        public async Task UpdateUser(UserModel user)
        {
            using (var connection = CreateDBConnection())
            {
                await connection.QueryAsync("UPDATE Users SET Users.Firstname = @Firstname, Users.Surname = @Surname, Users.Location = @Location, Users.HashedPassword = @HashedPassword, Users.Salt = @Salt WHERE Users.ID = @ID", user);
            }
        }

        public async Task UpdateUserPassword(UserModel user)
        {
            using (var connection = CreateDBConnection())
            {
                await connection.QueryAsync("UPDATE Users SET Users.HashedPassword = @HashedPassword, Users.Salt = @Salt WHERE Users.Email = @Email", user);
            }
        }

        public async Task<List<ImageModel>> GetAllUserImages()
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<ImageModel>("SELECT * FROM sinder.Images ;")).ToList();
            }
        }

        public async Task<List<ImageModel>> GetUserImagesByImageId(int imageId)
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<ImageModel>("SELECT * FROM sinder.Images WHERE ID = @imageId ;", new { imageId = imageId })).ToList();
            }
        }

        public async Task<List<ImageModel>> GetUserImagesByUserID(int userId)
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<ImageModel>("SELECT * FROM sinder.Images WHERE Images.UserID = @userId ;", new { userId = userId })).ToList();
            }
        }

        public async Task AddUserImage(int userId, string url)
        {
            using (var connection = CreateDBConnection())
            {
                await connection.QueryAsync("INSERT INTO sinder.Images(Images.UserID, Images.Url) VALUES (@userId, @imageUrl) ;", new { userId = userId, imageUrl = url });
            }
        }

        public async Task AddUserRelationship(int loggedInUser, int targetUser)
        {
            using (var connection = CreateDBConnection())
            {
                await connection.QueryAsync("INSERT INTO sinder.Relationship(Relationship.UserID1, Relationship.UserID2, Relationship.Status1, Relationship.Status2) VALUES (@userId1, @userid2, 1, 0) ;", new { userId1 = loggedInUser, userid2 = targetUser });
            }
        }

        public async Task<bool> CheckIfRelationshipExists(int loggedInUser, int targetUser)
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<bool>("SELECT EXISTS(SELECT * FROM sinder.Relationship WHERE Relationship.UserID1 = @userId1 AND Relationship.UserID2 = @userid2)", new { userId1 = loggedInUser, userid2 = targetUser })).First();
            }
        }

        public async Task UpdateUserImage(int userId, string oldUrl, string newUrl)
        {
            using (var connection = CreateDBConnection())
            {
                await connection.QueryAsync("UPDATE sinder.Images SET Images.Url = @newUrl WHERE UserID = @userId AND Url = @oldUrl ;", new { userId = userId, oldUrl = oldUrl, newUrl = newUrl });
            }
        }

        public async Task DeleteUserImage(int imageId)
        {
            using (var connection = CreateDBConnection())
            {
                await connection.QueryAsync("DELETE FROM sinder.Images WHERE ID = @imageId;", new { imageId = imageId});
            }
        }

        public async Task DeleteUserImage(int userId, string url)
        {
            using (var connection = CreateDBConnection())
            {
                await connection.QueryAsync("DELETE FROM sinder.Images WHERE UserID = @userId AND Url = @url ;", new { userId = userId, Url = url });
            }
        }

        #endregion
    }
}

