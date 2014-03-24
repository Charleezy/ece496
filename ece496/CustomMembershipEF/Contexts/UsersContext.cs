﻿using System.Data.Entity;
using System.Linq;
using CustomMembershipEF.Entities;

namespace CustomMembershipEF.Contexts
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        // Helper methods. User can also directly access "Users" property
        public void AddUser(User user)
        {
            Users.Add(user);
            SaveChanges();
        }

        public User GetUser(string userName)
        {
            var user = Users.SingleOrDefault(u => u.Username == userName);
            return user;
        }

        public User GetUser(string userName, string password)
        {
            var user = Users.SingleOrDefault(u => u.Username == userName && u.Password == password);
            return user;
        }

        public int GetUserId(string userName)
        {
            var user = Users.SingleOrDefault(u => u.Username == userName);

            if (user == null)
            {
                int id = -1;
                return id;
            }
            else
            {
                int id = user.UserID;
                return id;
            }
        }

        public string GetUserName(int? userID)
        {
            string username;
            var user = Users.SingleOrDefault(u => u.UserID == userID);
            if (user == null)
            {
                username = "";
            }
            else
            {
                username = user.Username;
            } 
            return username;
        }
    }
}
