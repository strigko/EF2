using Azure;
using Dapper;
using EFP48.DapperCase;
using EFP48.DapperCase.Entity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EFP48
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\User\source\repos\EFP48\db_dapper.mdf;Integrated Security=True";

            using IDbConnection connection = new SqlConnection(connectionString);

            string query = @"
CREATE TABLE Users
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Surname NVARCHAR(255) NOT NULL,
    Age INT NOT NULL

);
";
            //connection.Execute(query);


            var users = new List<User>
            {
                new User{ Id = Guid.NewGuid(), Name = "Tom", Surname = "Due", Age = 25},
                new User{ Id = Guid.NewGuid(), Name = "Alex", Surname = "Jackson", Age = 25},
                new User{ Id = Guid.NewGuid(), Name = "Bill", Surname = "White", Age = 25},
            };

            query = @"INSERT INTO Users(Id, Name, Surname, Age) VALUES(@Id,@Name,@Surname,@Age)";
/*            connection.Execute(query, users);*/


            query = @"SELECT * FROM Users as u ORDER BY u.Age DESC";

            var q_users = connection.Query<User>(query).ToList();

            if(q_users != null)
            {
                foreach(var item in q_users)
                {
                    Console.WriteLine(item);
                }
            }

            Guid searchId = Guid.Parse("91437934-bc67-4906-980b-1243d1642849");
            query = @"SELECT * FROM Users as u WHERE u.Id = @Id";
            var user = connection.QueryFirstOrDefault<UserCard>(query, new { Id = searchId });

            if(user != null) Console.WriteLine(user);

            query = @"
DROP TABLE IF EXISTS Posts;
CREATE TABLE Posts
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(200),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    DeletedAt DATETIME DEFAULT NULL,
    CONSTRAINT fk_post_user FOREIGN KEY (UserId) REFERENCES Users(Id)
);
";
/*            connection.Execute(query);*/

            query = @"INSERT INTO Posts(UserId, Title) VALUES(@UserId, @Title)";

            var list = new[]
            {
                new { UserId = Guid.Parse("7c35246d-a06d-480b-8a8c-531f742b8b16"), Title = "That`s a fine day!" },
                new { UserId = Guid.Parse("c4767365-09b7-4b5a-87a1-a3bef27f7e30"), Title = "Hello world"} 
            }.ToList();

            /*            connection.Execute(query, list);*/

            query = @"
            CREATE TABLE Comments
            (
                Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
                PostId UNIQUEIDENTIFIER NOT NULL,
                UserId UNIQUEIDENTIFIER NOT NULL,
                Content NVARCHAR(500),
                CreatedAt DATETIME DEFAULT GETDATE(),

                CONSTRAINT fk_comment_post FOREIGN KEY(PostId) REFERENCES Posts(Id),
                CONSTRAINT fk_comment_user FOREIGN KEY(UserId) REFERENCES Users(Id)
            );

            CREATE TABLE PostLikes
            (
                Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
                PostId UNIQUEIDENTIFIER NOT NULL,
                UserId UNIQUEIDENTIFIER NOT NULL,

                CONSTRAINT fk_post_like FOREIGN KEY(PostId) REFERENCES Posts(Id),
                CONSTRAINT fk_post_like_user FOREIGN KEY(UserId) REFERENCES Users(Id)
            );

            CREATE TABLE CommentLikes
            (
                Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
                CommentId UNIQUEIDENTIFIER NOT NULL,
                UserId UNIQUEIDENTIFIER NOT NULL,

                CONSTRAINT fk_comment_like FOREIGN KEY(CommentId) REFERENCES Comments(Id),
                CONSTRAINT fk_comment_like_user FOREIGN KEY(UserId) REFERENCES Users(Id)
            );";
            /*            connection.Execute(query);*/

            query = @"INSERT INTO Comments(PostId, UserId, Content) 
VALUES(@PostId, @UserId, @Content)";

            var comments = new[]
            {
    new {
        PostId = Guid.Parse("228cc7a5-0015-4513-8252-1c15e1c50860"),
        UserId = Guid.Parse("7c35246d-a06d-480b-8a8c-531f742b8b16"),
        Content = "Nice post!"
    },
    new {
        PostId = Guid.Parse("228cc7a5-0015-4513-8252-1c15e1c50860"),
        UserId = Guid.Parse("03a90025-ff3a-4812-9abb-ec3fa04ca510"),
        Content = "I agree!"
    }
};

            connection.Execute(query, comments);

            query = @"INSERT INTO CommentLikes(CommentId, UserId)
VALUES(@CommentId, @UserId)";

            var likes = new[]
            {
    new {
        CommentId = Guid.Parse("ff5b8253-47cf-451d-bc29-840b1f4ad02f"),
        UserId = Guid.Parse("c4767365-09b7-4b5a-87a1-a3bef27f7e30")
    },
    new {
        CommentId = Guid.Parse("ff5b8253-47cf-451d-bc29-840b1f4ad02f"),
        UserId = Guid.Parse("6e20ee03-d0e1-4734-b142-ff4ba3038883")
    }
};

            connection.Execute(query, likes);


            query = @"
SELECT p.Title, c.Content, COUNT(cl.Id) AS Likes
FROM Posts p
JOIN Comments c ON c.PostId = p.Id
LEFT JOIN CommentLikes cl ON cl.CommentId = c.Id
GROUP BY p.Title, c.Content
ORDER BY Likes DESC";

var result = connection.Query(query);

foreach(var item in result)
{
    Console.WriteLine($"{item.Title} | {item.Content} | Likes: {item.Likes}");
}

            query = @"
SELECT p.Title, COUNT(c.Id) AS CommentCount
FROM Posts p
LEFT JOIN Comments c ON c.PostId = p.Id
GROUP BY p.Title";

            var commentsCount = connection.Query(query);

            foreach (var item in commentsCount)
            {
                Console.WriteLine($"{item.Title} -> {item.CommentCount} comments");
            }


            query = @"
SELECT TOP 1 u.Name, u.Surname, COUNT(cl.Id) AS TotalLikes
FROM Users u
JOIN Comments c ON c.UserId = u.Id
JOIN CommentLikes cl ON cl.CommentId = c.Id
GROUP BY u.Name, u.Surname
ORDER BY TotalLikes DESC";

            var topUser = connection.QueryFirstOrDefault(query);

            Console.WriteLine($"Top user: {topUser.Name} {topUser.Surname} | Likes: {topUser.TotalLikes}");


            var userRepo = new UserRepository(connectionString);
            var postRepo = new PostRepository(connectionString);

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Name = "Alice",
                Surname = "Wonder",
                Age = 28
            };
            userRepo.Create(newUser);
            Console.WriteLine($"Created User: {newUser.Name} {newUser.Surname}");

            var allUsers = userRepo.GetAll().ToList();
            Console.WriteLine("All users:");
            foreach (var u in allUsers)
                Console.WriteLine(u.Name + " " + u.Surname);

            var singleUser = userRepo.GetById(newUser.Id);
            Console.WriteLine($"Single user fetched by Id: {singleUser.Name} {singleUser.Surname}");

            newUser.Name = "Alice Updated";
            userRepo.Update(newUser);
            var updatedUser = userRepo.GetById(newUser.Id);
            Console.WriteLine($"Updated user: {updatedUser.Name} {updatedUser.Surname}");

            userRepo.Delete(newUser.Id);
            var afterDelete = userRepo.GetById(newUser.Id);
            Console.WriteLine("Deleted user fetch returns: " + (afterDelete == null ? "null" : "error"));

            var existingUser = allUsers.First();

            var newPost = new Post
            {
                Id = Guid.NewGuid(),
                UserId = existingUser.Id,
                Title = "My Repository Post",
                CreatedAt = DateTime.Now
            };
            postRepo.Create(newPost);
            Console.WriteLine($"Created Post: {newPost.Title}");

            var allPosts = postRepo.GetAll().ToList();
            Console.WriteLine("All posts:");
            foreach (var p in allPosts)
                Console.WriteLine($"{p.Title} by {p.UserId}");

            var singlePost = postRepo.GetById(newPost.Id);
            Console.WriteLine($"Single post fetched: {singlePost.Title}");

            newPost.Title = "Updated Post Title";
            postRepo.Update(newPost);
            var updatedPost = postRepo.GetById(newPost.Id);
            Console.WriteLine($"Updated post: {updatedPost.Title}");

            postRepo.Delete(newPost.Id);
            var afterDeletePost = postRepo.GetById(newPost.Id);
            Console.WriteLine("Deleted post fetch returns: " + (afterDeletePost == null ? "null" : "error"));


        }
    }
    class UserCard
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public override string ToString()
        {
            return $@"
-------------------------------------
Name -> {Name}
Surname -> {Surname}
-------------------------------------
";
        }
    }
}