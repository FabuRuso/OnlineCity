using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Transfer;
using Util;

namespace Server
{
    //public class Database : IDisposable
    //{
    //    private NpgsqlConnection connection;
    //    public NpgsqlConnection Connection => connection;

    //    public const string UsersTable = "ADMIN.ONLINE_USER";

    //    public const string UserRolesTable = "ADMIN.USER_ROLE";

    //    public const string RolesTable = "ADMIN.ROLE";

    //    public Database(string connectionString)
    //    {
    //        connection = new NpgsqlConnection(connectionString);
    //    }

    //    public void Open()
    //    {
    //        if (connection.State != ConnectionState.Open)
    //        {
    //            connection.Open();
    //        }
    //    }

    //    public void Close()
    //    {
    //        if (connection.State != ConnectionState.Closed)
    //        {
    //            connection.Close();
    //        }
    //    }

    //    public void CreateUser(string login, string password, string passwordSalt, string email, string discordUserName, long version, string sessionKey, string sessionSalt)
    //    {
    //        using (NpgsqlCommand cmd = connection.CreateCommand())
    //        {
    //            cmd.CommandText = $"with new_user as (INSERT INTO {UsersTable} (login, password, password_salt, email, discord_user_name, version, session_key, session_key_salt, last_join_time) VALUES (@login, @password, @password_salt, @email, @discordUserName, @version, @session_key, @session_key_salt, @last_join_time) returning *)" +
    //                $"insert into {UserRolesTable} (user_id, role_id) SELECT n.id, r.id from new_user n cross join {RolesTable} r where r.is_default";
    //            cmd.CommandType = CommandType.Text;

    //            cmd.Parameters.Add("login", NpgsqlDbType.Text).Value = login;
    //            cmd.Parameters.Add("password", NpgsqlDbType.Varchar).Value = password;
    //            cmd.Parameters.Add("password_salt", NpgsqlDbType.Varchar).Value = passwordSalt;
    //            cmd.Parameters.Add("email", NpgsqlDbType.Text).Value = email;
    //            cmd.Parameters.Add("discordUserName", NpgsqlDbType.Text).Value = discordUserName;
    //            cmd.Parameters.Add("version", NpgsqlDbType.Integer).Value = version;
    //            cmd.Parameters.Add("session_key", NpgsqlDbType.Varchar).Value = sessionKey;
    //            cmd.Parameters.Add("session_key_salt", NpgsqlDbType.Varchar).Value = sessionSalt;
    //            cmd.Parameters.Add("last_join_time", NpgsqlDbType.Timestamp).Value = DateTime.Now;

    //            cmd.ExecuteNonQuery();
    //        }
    //    }

    //    public bool IsUserExist(string login)
    //    {
    //        using (NpgsqlCommand cmd = connection.CreateCommand())
    //        {
    //            cmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM {UsersTable} WHERE login = @login)";
    //            cmd.CommandType = CommandType.Text;

    //            cmd.Parameters.Add("login", NpgsqlDbType.Text).Value = login;

    //            return bool.Parse(cmd.ExecuteScalar().ToString());
    //        }
    //    }

    //    public void Login(string login, string sessionKey, string saltInt)
    //    {
    //        using (NpgsqlCommand cmd = connection.CreateCommand())
    //        {
    //            cmd.CommandText = $"UPDATE {UsersTable} SET LAST_JOIN_TIME = @last_join_time, LAST_DISCONNECT_TIME = @last_disconnect_time, SESSION_KEY = @session_key, SESSION_KEY_SALT = @session_key_salt WHERE LOGIN = @login";
    //            cmd.CommandType = CommandType.Text;

    //            cmd.Parameters.Add("last_join_time", NpgsqlDbType.Timestamp).Value = DateTime.Now;
    //            cmd.Parameters.Add("last_disconnect_time", NpgsqlDbType.Timestamp).Value = DBNull.Value;
    //            cmd.Parameters.Add("session_key", NpgsqlDbType.Varchar).Value = sessionKey;
    //            cmd.Parameters.Add("session_key_salt", NpgsqlDbType.Varchar).Value = saltInt;
    //            cmd.Parameters.Add("login", NpgsqlDbType.Text).Value = login;

    //            cmd.ExecuteNonQuery();
    //        }
    //    }

    //    public void Logout(string login)
    //    {
    //        using (NpgsqlCommand cmd = connection.CreateCommand())
    //        {
    //            cmd.CommandText = $"UPDATE {UsersTable} SET LAST_DISCONNECT_TIME = @last_disconnect_time, LAST_JOIN_TIME = @last_join_time, SESSION_KEY = @session_key, SESSION_KEY_SALT = @session_key_salt WHERE LOGIN = @login";
    //            cmd.CommandType = CommandType.Text;

    //            cmd.Parameters.Add("last_disconnect_time", NpgsqlDbType.Timestamp).Value = DateTime.Now;
    //            cmd.Parameters.Add("last_join_time", NpgsqlDbType.Timestamp).Value = DBNull.Value;
    //            cmd.Parameters.Add("session_key", NpgsqlDbType.Varchar).Value = DBNull.Value;
    //            cmd.Parameters.Add("session_key_salt", NpgsqlDbType.Varchar).Value = DBNull.Value;
    //            cmd.Parameters.Add("login", NpgsqlDbType.Text).Value = login;

    //            cmd.ExecuteNonQuery();
    //        }
    //    }

    //    public void UpdatePassword(string login, string passwordHash, string passwordSalt)
    //    {
    //        using (NpgsqlCommand cmd = connection.CreateCommand())
    //        {
    //            cmd.CommandText = $"UPDATE {UsersTable} SET PASSWORD = @password, PASSWORD_SALT = @password_salt WHERE LOGIN = @login";
    //            cmd.CommandType = CommandType.Text;

    //            cmd.Parameters.Add("password", NpgsqlDbType.Varchar).Value = passwordHash;
    //            cmd.Parameters.Add("password_salt", NpgsqlDbType.Varchar).Value = passwordSalt;
    //            cmd.Parameters.Add("login", NpgsqlDbType.Text).Value = login;

    //            cmd.ExecuteNonQuery();
    //        }
    //    }

    //    public void CheckAndClearOldSessions()
    //    {
    //        using (NpgsqlCommand cmd = connection.CreateCommand())
    //        {
    //            cmd.CommandText = $"UPDATE {UsersTable} SET LAST_DISCONNECT_TIME = @last_disconnect_time, LAST_JOIN_TIME = @last_join_time, SESSION_KEY = @session_key, SESSION_KEY_SALT = @session_key_salt WHERE SESSION_KEY IS NOT NULL";
    //            cmd.CommandType = CommandType.Text;

    //            cmd.Parameters.Add("last_disconnect_time", NpgsqlDbType.Timestamp).Value = DateTime.Now;
    //            cmd.Parameters.Add("last_join_time", NpgsqlDbType.Timestamp).Value = DBNull.Value;
    //            cmd.Parameters.Add("session_key", NpgsqlDbType.Varchar).Value = DBNull.Value;
    //            cmd.Parameters.Add("session_key_salt", NpgsqlDbType.Varchar).Value = DBNull.Value;

    //            cmd.ExecuteNonQuery();
    //        }
    //    }

    //    public void Dispose()
    //    {
    //        connection.Dispose();
    //    }

    //    public static void GenerateSessionKey(string password, out string sessionKey, out string sessionSalt)
    //    {
    //        byte[] salt = new byte[128 / 8];

    //        SessionClient.RandomNumberGenerator.GetBytes(salt);
    //        sessionSalt = Convert.ToBase64String(salt);

    //        sessionKey = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: password,
    //                                               salt: salt,
    //                                               prf: KeyDerivationPrf.HMACSHA256,
    //                                               iterationCount: 100000,
    //                                               numBytesRequested: 256 / 8));
    //    }

    //    public static string ComputeHash(string input, HashAlgorithm algorithm, byte[] salt)
    //    {
    //        StringBuilder stringBuilder = new StringBuilder();

    //        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

    //        byte[] saltedInput = new byte[salt.Length + inputBytes.Length];
    //        salt.CopyTo(saltedInput, 0);
    //        inputBytes.CopyTo(saltedInput, salt.Length);

    //        byte[] hashedBytes = algorithm.ComputeHash(saltedInput);

    //        foreach (byte b in hashedBytes)
    //        {
    //            stringBuilder.Append(b.ToString("x2"));
    //        }

    //        return stringBuilder.ToString();
    //    }
    //}
}
