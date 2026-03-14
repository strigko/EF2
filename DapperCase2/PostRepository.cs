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
    internal class PostRepository : IRepository<Post>
    {
        private readonly string connectionString;

        public PostRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Create(Post post)
        {
            using IDbConnection db = new SqlConnection(connectionString);
            db.Execute(
                "INSERT INTO Posts(Id, UserId, Title, CreatedAt, DeletedAt) VALUES(@Id,@UserId,@Title,@CreatedAt,@DeletedAt)",
                post);
        }

        public void Update(Post post)
        {
            using IDbConnection db = new SqlConnection(connectionString);
            db.Execute(
                "UPDATE Posts SET Title=@Title, DeletedAt=@DeletedAt WHERE Id=@Id",
                post);
        }

        public void Delete(Guid id)
        {
            using IDbConnection db = new SqlConnection(connectionString);
            db.Execute("DELETE FROM Posts WHERE Id=@Id", new { Id = id });
        }

        public Post GetById(Guid id)
        {
            using IDbConnection db = new SqlConnection(connectionString);
            return db.QueryFirstOrDefault<Post>(
                "SELECT * FROM Posts WHERE Id=@Id",
                new { Id = id });
        }

        public IEnumerable<Post> GetAll()
        {
            using IDbConnection db = new SqlConnection(connectionString);
            return db.Query<Post>("SELECT * FROM Posts");
        }
    }
}
