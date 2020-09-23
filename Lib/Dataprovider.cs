using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sinder.Models;

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

        public async Task<List<UserModel>> ReadAllUsers()
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<UserModel>("SELECT * FROM Users")).ToList();
            }
        }

        public async Task<UserModel> ReadUserById(int id)
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<UserModel>("SELECT * FROM Users WHERE Users.ID = @Id", new { Id = id })).First();
            }
        }

        /// <summary>
        /// Single return
        /// </summary>
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

        /// <summary>
        /// Careful, since it updates a bunch of values
        /// </summary>
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

        /// <summary>
        /// Please don't use this one too much
        /// </summary>
        public async Task<List<ImageModel>> GetAllUserImages()
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<ImageModel>("SELECT * FROM sinder.Images ;")).ToList();
            }
        }

        /// <summary>
        /// Get all users which occurs in supplied int[] array, might be extra vulnerable to SQL-injection ¯\_(ツ)_/¯
        /// </summary>
        public async Task<List<ImageModel>> GetUserImagesWhichIsInList(List<UserModel> users)
        {
            using (var connection = CreateDBConnection())
            {
                string idList = Helper.GenerateStringFromIds<UserModel>(users);
                return (await connection.QueryAsync<ImageModel>("SELECT * FROM sinder.Images WHERE @idArray ;", new { idArray = idList })).ToList();
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

        public async Task AddUserRelationship(int loggedInUser, int targetUser, Relationship relationshipType)
        {
            using (var connection = CreateDBConnection())
            {
                await connection.QueryAsync("INSERT INTO sinder.Relationship(Relationship.UserID1, Relationship.UserID2, Relationship.Status1, Relationship.Status2) VALUES (@userId1, @userid2, @relationshipType, 0) ;", new { userId1 = loggedInUser, userid2 = targetUser, relationshipType = (int)relationshipType });
            }
        }

        /// <summary>
        /// Set two user's realtionship to the same value
        /// </summary>
        public async Task MatchRelationship(int usedId1, int usedId2, Relationship relationshipType)
        {
            using (var connection = CreateDBConnection())
            {
                await connection.QueryAsync("UPDATE Relationship SET `Status1` = @relType, `Status2` = @relType WHERE `UserID1` = @userId1 AND `UserID2` = @userId2 OR `UserID1` = @userId2 AND `UserID2` = @userId1;", new { userId1 = usedId1, userid2 = usedId2, relType = (int)relationshipType });
            }
        }



        public async Task<bool> CheckIfRelationshipExists(int relationID)
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<bool>("SELECT EXISTS(SELECT * FROM Relationship WHERE Relationship.ID = @relId ) ;", new { relId = relationID })).First();
            }
        }
        /// <summary>
        /// ? if relationship exists
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckIfRelationshipExists(int loggedInUser, int targetUser)
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<bool>("SELECT EXISTS(SELECT * FROM sinder.Relationship WHERE Relationship.UserID1 = @userId1 AND Relationship.UserID2 = @userid2 OR Relationship.UserID1 = @userId2 AND Relationship.UserID2 = @userId1 )", new { userId1 = loggedInUser, userid2 = targetUser })).First();
            }
        }

        public async Task<int> ReadRealtionShipId(int userA, int userB)
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<int>("SELECT r.ID FROM Relationship r WHERE  r.UserID1 = @userA AND r.UserID2 = @userB OR r.UserID1 = @userB AND r.UserID2 = @userA", new { userA = userA, userB = userB })).First();
            }
        }

        public async Task<bool> CheckIfSameRelationShip(int userA, int userB)
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<bool>("SELECT EXISTS(SELECT * FROM sinder.Relationship WHERE Relationship.UserID1 = @userId1 AND Relationship.UserID2 = @userid2 OR Relationship.UserID1 = @userId2 AND Relationship.UserID2 = @userId1 )", new { userId1 = userA, userid2 = userB })).First();
            }
        }

        public async Task<RelationShipModel> ReadRelationShipById(int relationID)
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<RelationShipModel>("SELECT * FROM Relationship r WHERE r.ID = @relId; ;", new { relId = relationID })).First();
            }
        }

        /// <summary>
        /// Query an users active realtionship from the db. Both pending and active
        /// </summary>
        public async Task<List<RelationshipDto>> ReadUserRelationships(int userID)
        {
            using (var connection = CreateDBConnection())
            {
                string query = @"SELECT 
                r.ID as RelationShipID, 
                r.UserID1 as ProtagonistID, 
                r.UserID2 as AntagonistID, 
                r.Status1 as Status1, 
                r.Status2 as Status2, 
                r.CreatedAt as CreatedAt, 
                u.firstName as AntagonistFirstName, 
                r.CreatedAt as CreatedAt 
                FROM Relationship r 
                RIGHT JOIN Users u ON u.ID = r.UserID2 
                WHERE r.UserID1 = @userID OR r.UserID2 = @userID ;";

                List<RelationshipDto> relationshipDtos = (await connection.QueryAsync<RelationshipDto>(query, new { userID = userID })).ToList();
                foreach (RelationshipDto r in relationshipDtos)
                {
                    r.Images = (await GetUserImagesByUserID(r.AntagonistID)).ToList();
                }
                return relationshipDtos;
            }
        }
        /// <summary>
        /// Reads recieved requests for the specific user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<List<UserMatchDto>> ReadRecievedRequests(int userID)
        {
            using (var connection = CreateDBConnection())
            {
                string query = @"SELECT *
                FROM Relationship r
                RIGHT JOIN Users u on u.ID = r.UserID1
                WHERE r.Status1 > 0 AND r.UserID2 = @userID AND r.Status1 != r.Status2;";

                List<UserMatchDto> requester = (await connection.QueryAsync<UserMatchDto>(query, new { userID = userID })).ToList();
                foreach (UserMatchDto r in requester)
                {
                    r.Images = (await GetUserImagesByUserID(r.ID)).ToList();
                }
                return requester;
            }
        }
        /// <summary>
        /// Reads the requests sent from the specific user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<List<UserMatchDto>> ReadRequests(int userID)
        {
            using (var connection = CreateDBConnection())
            {
                string query = @"SELECT * 
                FROM Relationship r
                RIGHT JOIN Users u on u.ID = r.UserID2
                WHERE r.Status2 = 0 AND r.UserID1 = @userID AND r.Status1 != r.Status2;";

                List<UserMatchDto> requests = (await connection.QueryAsync<UserMatchDto>(query, new { userID = userID })).ToList();
                foreach (UserMatchDto r in requests)
                {
                    r.Images = (await GetUserImagesByUserID(r.ID)).ToList();
                }
                return requests;
            }
        }

        /// <summary>
        /// Reads all the matches for the specific user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<List<UserMatchDto>> ReadMatches(int userID)
        {
            using (var connection = CreateDBConnection())
            {
                string query = @"SELECT * 
                FROM Relationship r
                RIGHT JOIN Users u on u.ID = r.UserID1 OR u.ID = r.UserID2 
                WHERE r.Status1 = r.Status2
                AND r.UserID1 = @userID AND @userID != r.UserID2 
                OR r.UserID2 = @userID AND r.UserID1 != r.UserID2 
                AND r.Status1 = r.Status2;";

                List<UserMatchDto> matches = (await connection.QueryAsync<UserMatchDto>(query, new { userID = userID })).ToList();
                foreach (UserMatchDto m in matches)
                {
                    m.Images = (await GetUserImagesByUserID(m.ID)).ToList();
                }
                return matches;
            }
        }

        public async Task DeleteImageById(int imageId)
        {
            using (var connection = CreateDBConnection())
            {
                await connection.QueryAsync("DELETE FROM sinder.Images WHERE ID = @imageId;", new { imageId = imageId });
            }
        }

        public async Task DeleteUserImage(int userId, string url)
        {
            using (var connection = CreateDBConnection())
            {
                await connection.QueryAsync("DELETE FROM sinder.Images WHERE UserID = @userId AND Url = @url ;", new { userId = userId, Url = url });
            }
        }

        public async Task AddUserInterest(int userID, string value, string category = "unknown")
        {
            using (var connection = CreateDBConnection())
            {
                await connection.QueryAsync("INSERT INTO sinder.Interests(UserID, Value, Category) VALUES(@userId, @value, @category)", new { userId = userID, value = value, category = category }); ;
            }
        }

        public async Task<List<InterestModel>> GetUserInterests(int userID)
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<InterestModel>("SELECT * FROM sinder.Interests WHERE `UserID` = @userId;", new { userId = userID })).ToList();
            }

        }

        public async Task DeleteUserInterest(int userId, string nameOfInterest)
        {
            using (var connection = CreateDBConnection())
            {
                await connection.QueryAsync<InterestModel>("DELETE FROM Interests WHERE `UserID` = @userId AND `Value` = @nameOfInterest ;", new { userId = userId, nameOfInterest = nameOfInterest });
            }
        }

        public async Task<List<InterestModel>> GetAllInterests(int limit)
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<InterestModel>("SELECT * FROM sinder.InterestsStatic LIMIT @limit", new { limit = limit })).ToList();
            }
        }

        public async Task<List<InterestModel>> GetAllInterests() => await GetAllInterests(9999999);

        public async Task<List<InterestModel>> GetMatchingInterest(string searchValue, int limit = 5)
        {
            using (var connection = CreateDBConnection())
            {
                string query = @"SELECT * FROM sinder.InterestsStatic WHERE (Value LIKE CONCAT('%', @searchValue, '%')) LIMIT @limit";
                return (await connection.QueryAsync<InterestModel>(query, new { searchValue = searchValue, limit = limit })).ToList();
            }
        }

        public async Task<List<string>> AddStaticInterest(string value)
        {
            using (var connection = CreateDBConnection())
            {
                // Check if value already exists, else insert
                // Also return bool
                string query =
                    $"INSERT INTO sinder.InterestsStatic(Value)" +
                    $"SELECT * FROM(SELECT @value) AS tmp " +
                    $"WHERE NOT EXISTS( " +
                    $"SELECT InterestsStatic.Value FROM sinder.InterestsStatic WHERE InterestsStatic.Value = @value) LIMIT 1 ";
                return (await connection.QueryAsync<string>(query, new { value = value })).ToList();
            }
        }

        public async Task<List<string>> ReadAllMessagesWithUserID(int userid)
        {
            using (var connection = CreateDBConnection())
            {

                return new List<string>() { };
            }
        }

        /// <summary>
        /// Read whole chat log between two users
        /// </summary>
        /// <param name="relationshipID"></param>
        public async Task<List<MessageModel>> ReadAllMessagesBetweenTwoUsers(int relationshipID)
        {
            using (var connection = CreateDBConnection())
            {
                return (await connection.QueryAsync<MessageModel>("SELECT * FROM Messages WHERE Messages.RelationshipID = @relId", new { relId = relationshipID })).ToList();
            }
        }


        /// <summary>
        /// Send a message to a relation
        /// </summary>
        /// <param name="realtionShipId"></param>
        /// <param name="senderId"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task SendMessageFromTo(int relationshipId, int senderId, string text)
        {
            using (var connection = CreateDBConnection())
            {
                await connection.QueryAsync("INSERT INTO Messages(Messages.RelationshipID, `Sender`, `Text`) VALUES(@relID, @sender, @txt)", new { relId = relationshipId, sender = senderId, txt = text });
            }
        }

        public async Task DeleteMessageByMessageId(int messageId)
        {
            using (var connection = CreateDBConnection())
            {

            }
        }

        /// <summary>
        /// Join two tables together with foreign keys. MessageHistory.ID <--fk-- Messages --fk--> Relationship.ID
        /// </summary>
        public async Task CreateMessageTable(int messageId)
        {
            using (var connection = CreateDBConnection())
            {
                //await connection.QueryAsync("INSERT INTO Messages(Messages.RelationshipID) VALUES(@value) ;", new { value = value });
            }
        }

        #endregion
    }
}

