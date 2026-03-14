using Dapper;
using EFP48.DapperCase.Entity;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFP48.DapperCase
{
    internal class UserRepository : IRepository<User>
    {
        private readonly string connectionString;

        public UserRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Create(User user)
        {
            using IDbConnection db = new SqlConnection(connectionString);
            db.Execute(
                "INSERT INTO Users(Id, Name, Surname, Age) VALUES(@Id,@Name,@Surname,@Age)",
                user);
        }

        public void Update(User user)
        {
            using IDbConnection db = new SqlConnection(connectionString);
            db.Execute(
                "UPDATE Users SET Name=@Name, Surname=@Surname, Age=@Age WHERE Id=@Id",
                user);
        }

        public void Delete(Guid id)
        {
            using IDbConnection db = new SqlConnection(connectionString);
            db.Execute("DELETE FROM Users WHERE Id=@Id", new { Id = id });
        }

        public User GetById(Guid id)
        {
            using IDbConnection db = new SqlConnection(connectionString);
            return db.QueryFirstOrDefault<User>(
                "SELECT * FROM Users WHERE Id=@Id",
                new { Id = id });
        }

        public IEnumerable<User> GetAll()
        {
            using IDbConnection db = new SqlConnection(connectionString);
            return db.Query<User>("SELECT * FROM Users");
        }
    }
}
