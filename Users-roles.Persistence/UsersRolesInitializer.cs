﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Internal;
using Users_roles.Domain.Entities;
using Users_roles.Domain.Enumerations;
using Users_roles.Domain.ValueObjects;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Users_roles.Persistence
{
    public class UsersRolesInitializer
    {
        private readonly IDictionary<string, Role> Roles = new Dictionary<string, Role>();

        private readonly IDictionary<string, User> Users = new Dictionary<string, User>();

        public static void Initialize(UsersRolesDbContext context)
        {
            var initializer = new UsersRolesInitializer();
            initializer.SeedEverything(context);
        }

        private void SeedEverything(UsersRolesDbContext context)
        {
            context.Database.EnsureCreated();

            if (EnumerableExtensions.Any(context.Roles))
                return;

            SeedRoles(context);
            SeedUsers(context);
            SeedUserRoles(context);
        }

        private void SeedUserRoles(UsersRolesDbContext context)
        {
            var userRoles = new List<UserRole>
            {
                new UserRole
                {
                    UserId = Users.Keys.ElementAt(0),
                    RoleId = Roles.Keys.ElementAt(0)
                },
                new UserRole
                {
                    UserId = Users.Keys.ElementAt(0),
                    RoleId = Roles.Keys.ElementAt(1)
                },
            };

            context.UserRoles.AddRange(userRoles);

            context.SaveChanges();
        }

        private void SeedUsers(UsersRolesDbContext context)
        {
            string password = "mygenericpassword";

            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var users = new List<User>
            {
                new User
                {
                    Name = (FullName)"Ivan Pažanin",
                    UserAvatarRelativePath = "/pictures/placeholder.jpg",
                    Email = (Email)"ipazan00@fesb.hr",
                    Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, 
                        KeyDerivationPrf.HMACSHA1, 10000, 256 / 8)),
                },
            };
            
            Users.Add(users[0].Id, users[0]);

            context.AddRange(users);

            context.SaveChanges();
        }

        public void SeedRoles(UsersRolesDbContext context)
        {
            var roles = new List<Role>
            {
                new Role
                {
                    RoleType = RoleType.Admin
                },
                new Role
                {
                    RoleType = RoleType.User
                },
                new Role
                {
                    RoleType = RoleType.Guest
                },
            };

            Roles.Add(roles[0].Id, roles[0]);
            Roles.Add(roles[1].Id, roles[1]);
            Roles.Add(roles[2].Id, roles[2]);

            context.Roles.AddRange(roles);

            context.SaveChanges();
        }
    }
}
