﻿using System;
using System.Data.SQLite;
using System.IO;
using System.Text;
using Dapper;
using HQF.Tutorials.Dapper.Domain;

namespace HQF.Tutorials.Dapper.UnitTest
{
    internal class DapperForSQLite
    {
        private static SQLiteConnection _dbConnection;
        private static Random _random = new Random();

        public static void CreateAndOpenDb()
        {
            var dbFilePath = "./TestDb.sqlite";
            if (File.Exists(dbFilePath))
                File.Delete(dbFilePath);

            if (!File.Exists(dbFilePath))
                SQLiteConnection.CreateFile(dbFilePath);
            _dbConnection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", dbFilePath));
            _dbConnection.Open();
        }

        public static int GetUsersCount()
        {
            var usersCount = _dbConnection.ExecuteScalar("Select Count(*) From Users");
            return usersCount != null ? int.Parse(usersCount.ToString()) : 0;
        }

        public static void SeedDatabase()
        {
            // Create a Users table
            _dbConnection.ExecuteNonQuery(@"
        CREATE TABLE IF NOT EXISTS [Users] (
            [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            [Username] NVARCHAR(64) NOT NULL,
            [Email] NVARCHAR(128) NOT NULL,
            [Password] NVARCHAR(128) NOT NULL,
            [DateCreated] TIMESTAMP DEFAULT CURRENT_TIMESTAMP
        )");

            // Insert an ADMIN user
            _dbConnection.ExecuteNonQuery(@"
        INSERT INTO Users
            (Username, Email, Password)
        VALUES
            ('admin', 'niemand.richard@gmail.com', 'test')");
        }

        public static void CreateSecondUser()
        {
            var secondUser = new User
            {
                Username = "rachel",
                Email = "1@2.com",
                Password = "password"
            };

            if (!secondUser.ExistsInDb(_dbConnection))
            {
                secondUser.SaveAsNewUser(_dbConnection);
            }
        }

        public static void ModifyAdminUser()
        {
            var adminUser = _dbConnection.GetUserByName("admin");
            adminUser.Password = string.Format(
                "pass_{0}", DateTime.Now.Millisecond);
            adminUser.SaveChanges(_dbConnection);
        }

        public static void AddMoreUsers(int amount)
        {
            var baseUsername = string.Format(
                "{0}{1}{2}{3}{4}{5}",
                DateTime.Now.Year,
                DateTime.Now.Month.ToString().PadLeft(2, '0'),
                DateTime.Now.Day.ToString().PadLeft(2, '0'),
                DateTime.Now.Hour.ToString().PadLeft(2, '0'),
                DateTime.Now.Minute.ToString().PadLeft(2, '0'),
                DateTime.Now.Second.ToString().PadLeft(2, '0'));

            for (var i = 0; i < amount; i++)
            {
                var tempUser = new User
                {
                    Username = string.Format("{0}{1}",
                        baseUsername, RandomString(4)),
                    Password = RandomString(10),
                    Email = string.Format("{0}@{1}.com",
                        RandomString(12), RandomString(5))
                };

                tempUser.SaveAsNewUser(_dbConnection);
            }
        }

        public static void RemoveLastNonAdminUser()
        {
            var user = _dbConnection.GetLastUser();
            if (user.Username == "admin") return;
            user.RemoveFromDb(_dbConnection);
        }

        private static string RandomString(int size)
        {
            // http://stackoverflow.com/questions/1122483/random-string-generator-returning-same-string
            var builder = new StringBuilder();

            for (var i = 0; i < size; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(
                    Math.Floor(26 * _random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}