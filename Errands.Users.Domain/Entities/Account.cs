using Errands.Users.Domain.Exceptions;
using Errands.Users.Domain.Models;
using System;
using System.Collections.Generic;

namespace Errands.Users.Domain.Entities
{
    public class Account
    {
        public Account()
        {
            
        }

        public Account(SignUpRequest model)
        {
            if (model == null) throw new BusinessException($"{nameof(SignUpRequest)} cannot be null!");
            if (!model.IsValid) throw new BusinessException(model.ErrorMessage, model);
            Id = Guid.NewGuid().ToString();
            Username = model.Username.Trim().ToLower();
            Email = model.Email.Trim().ToLower();
            Name = model.Name.Trim();
            DateCreated = DateTime.UtcNow;
            Roles = new List<string> { "user" };
            LastLogin = DateCreated;
            Phone = model.Phone;
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public string Phone { get; set; }
        public int AccessFailedCount { get; set; }
        public bool IsAccountLocked { get; set; }
        public DateTime? LockoutExpiry { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? LastPasswordChange { get; set; }
        public string PasswordToken { get; set; }
        public DateTime? PasswordTokenExpiry { get; set; }
        public List<string> Roles { get; set; }
        public DateTime? LastLogin { get; set; }

        public string GeneratePasswordToken(DateTime expiry)
        {
            var token = new Random().Next(100001, 1000000).ToString();
            PasswordToken = SaltToken(Username, token);
            PasswordTokenExpiry = expiry;
            return token;
        }

        public bool IsPasswordTokenExpired() =>
            (!string.IsNullOrEmpty(PasswordToken)) &&
            PasswordTokenExpiry.HasValue &&
            DateTime.UtcNow > PasswordTokenExpiry;
        public void SetPassword(string passwordHash)
        {
            PasswordHash = passwordHash;
            PasswordToken = null;
            PasswordTokenExpiry = null;
            LastPasswordChange = DateTime.UtcNow;
        }

        public static string SaltToken(string username, string token)
        {
            return $"{username}|{token}";
        }

        public void AddRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) throw new BusinessException("Invalid role: role cannot be null!");
            var roleCode = role.Trim().ToLower();
            if (Roles?.Contains(roleCode) == true) throw new BusinessException("User already belongs to this role");
            (Roles ??= new List<string>()).Add(roleCode);
        }        
        public void RemoveRole(string role) => Roles?.Remove(role?.ToLower());
    }
}
