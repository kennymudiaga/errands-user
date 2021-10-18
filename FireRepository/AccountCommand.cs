using Errands.Users.Domain.Commands;
using Errands.Users.Domain.Config;
using Errands.Users.Domain.Entities;
using Errands.Users.Domain.Exceptions;
using Errands.Users.Domain.Models;
using Errands.Users.Domain.Utilities;
using Google.Cloud.Firestore;
using JwtFactory;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FireRepository
{
    public class AccountCommand : IAccountCommand
    {
        private readonly CollectionReference accountsCollection;
        private readonly JwtProvider jwtProvider;

        public readonly IPasswordHasher<string> passwordHasher;
        private readonly UserPolicy userPolicy;

        public AccountCommand(FirestoreDb firestoreDb, JwtProvider jwtProvider,
            IPasswordHasher<string> passwordHasher, UserPolicy userPolicy)
        {
            accountsCollection = firestoreDb.Collection("user-accounts");
            this.jwtProvider = jwtProvider;
            this.passwordHasher = passwordHasher;
            this.userPolicy = userPolicy;
        }
        public async Task<LoginResponse> Login(LoginRequest model)
        {
            if (model == null || !model.IsValid) throw new BusinessException(model?.ErrorMessage ?? "Invalid or null login request!", model);
            var userQuery = await accountsCollection.WhereEqualTo(nameof(Account.Email), model.Email.Trim().ToLower())
                .Limit(1).GetSnapshotAsync();
            var user = userQuery.Documents.FirstOrDefault()?.Parse<Account>();
            if(user == null) throw new BusinessException("Invalid login!");
            if (userPolicy.EnableLockout && user.IsAccountLocked)
            {
                if (user.LockoutExpiry > DateTime.Now) throw new BusinessException(LockoutMessage);
            }
            if (user.PasswordHash == null) throw new BusinessException("Your password is not set. Please check your mail for instructions or do a password reset.");
            if (VerifyPassword(user.Username, user.PasswordHash, model.Password))
            {
                var updates = new Dictionary<string, object>();
                updates.Add(nameof(Account.LastLogin), user.LastLogin = DateTime.UtcNow);
                if (user.AccessFailedCount > 0 || user.IsAccountLocked)
                {
                    user.IsAccountLocked = false;
                    user.AccessFailedCount = 0; 
                    updates.Add(nameof(Account.IsAccountLocked), user.IsAccountLocked = false);
                    updates.Add(nameof(Account.AccessFailedCount), user.AccessFailedCount = 0);                    
                }
                await accountsCollection.Document(user.Id).SetAsync(updates, SetOptions.MergeAll);
                return CreateLogin(user);
            }
            await InvalidateUser(user);
            return default;
        }

        public async Task<LoginResponse> SignUp(SignUpRequest model)
        {
            var account = new Account(model);
            var emailExists = await accountsCollection
                .WhereEqualTo(nameof(Account.Email), account.Email)
                .Limit(1).Select(nameof(Account.Email)).GetSnapshotAsync();
            var usernameExists = await accountsCollection
                .WhereEqualTo(nameof(Account.Username), account.Username)
                .Limit(1).Select(nameof(Account.Username)).GetSnapshotAsync();
            if (emailExists.Any() || usernameExists.Any())
                throw new BusinessException("Email or username already in use!", model);
            account.SetPassword(HashPassword(account.Email, model.Password));
            var accountDoc = accountsCollection.Document(account.Id);   
            _ = await accountDoc.CreateAsync(account.ToDictionary());
            return CreateLogin(account);
        }

        public string HashPassword(string username, string password)
        {
            return passwordHasher.HashPassword(username, password);
        }
        protected bool VerifyPassword(string username, string passwordHash, string providedPassword)
        {
            return passwordHasher.VerifyHashedPassword(username, passwordHash, providedPassword)
                != PasswordVerificationResult.Failed;
        }

        protected async Task InvalidateUser(Account user)
        {
            if (userPolicy.EnableLockout)
            {
                if (++user.AccessFailedCount >= userPolicy.MaxPasswordFailCount)
                {
                    user.IsAccountLocked = true;
                    user.LockoutExpiry = DateTime.Now.AddMinutes(userPolicy.PasswordLockoutMinutes);
                }
                await accountsCollection.Document(user.Id).SetAsync(new
                {
                    user.AccessFailedCount,
                    user.IsAccountLocked,
                    user.LockoutExpiry
                }, SetOptions.MergeAll);
                string errorMessage = user.IsAccountLocked ? LockoutMessage : GetPasswordWarning(userPolicy.MaxPasswordFailCount - user.AccessFailedCount);
                throw new BusinessException(errorMessage);
            }
            throw new BusinessException(InvalidPasswordMessage);
        }

        protected List<Claim> GetUserClaims(Account user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.GivenName, user.Name),
            };
            user.Roles?.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));        
            return claims;
        }

        public LoginResponse CreateLogin(Account user)
        {
            var expiryDate = DateTime.Now.AddMinutes(userPolicy.SessionTimeout);
            var claims = GetUserClaims(user);
            var jwt = jwtProvider.GetUserToken(claims, expiryDate);
            var roles = claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            return new LoginResponse
            {
                Email = user.Email,
                Name = user.Name,
                Token = jwt,
                Username = user.Username,
                Roles = string.Join(',', roles)
            };
        }

        protected const string InvalidPasswordMessage = "Invalid password.";
        protected static string GetPasswordWarning(int attemptsLeft) => $"Invalid password. {"attempt".Pluralize(attemptsLeft)} remaining.";
        protected string LockoutMessage => $"Your account is temporarily locked. Try again in {"minute".Pluralize(userPolicy.PasswordLockoutMinutes)}.";
    }
}
