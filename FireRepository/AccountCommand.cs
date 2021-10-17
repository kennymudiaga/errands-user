using Errands.Users.Domain.Commands;
using Errands.Users.Domain.Entities;
using Errands.Users.Domain.Models;
using Google.Cloud.Firestore;
using System;
using System.Threading.Tasks;

namespace FireRepository
{
    public class AccountCommand : IAccountCommand
    {
        private readonly CollectionReference accountsCollection;
        public AccountCommand(FirestoreDb firestoreDb)
        {
            accountsCollection = firestoreDb.Collection("user-accounts");
        }
        public Task<LoginResponse> Login(LoginRequest model)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginResponse> SignUp(SignUpRequest model)
        {
            var account = new Account(model);
            var accountDoc = accountsCollection.Document(account.Id);            
            _ = await accountDoc.CreateAsync(account.ToDictionary());
            return new LoginResponse
            {
                Email = account.Email,
                Name = account.Name,
                Username = account.Username
            };
        }
    }
}
