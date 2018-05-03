using System.Data.SqlClient;

namespace AuthDemo.Data
{
    public class AuthDb
    {
        private readonly string _connectionString;

        public AuthDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(User user, string password)
        {
            string passwordSalt = PasswordHelper.GenerateSalt();
            string passwordHash = PasswordHelper.HashPassword(password, passwordSalt);
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash, PasswordSalt) " +
                                  "VALUES (@name, @email, @passwordHash, @passwordSalt)";
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                cmd.Parameters.AddWithValue("@passwordSalt", passwordSalt);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public User GetByEmail(string email)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }

                return new User
                {
                    Email = email,
                    Name = (string) reader["Name"],
                    Id = (int) reader["Id"],
                    PasswordHash = (string) reader["PasswordHash"],
                    PasswordSalt = (string) reader["PasswordSalt"],
                };
            }
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isCorrectPassword = PasswordHelper.PasswordMatch(password, user.PasswordSalt, user.PasswordHash);
            if (isCorrectPassword)
            {
                return user;
            }

            return null;
        }
    }
}