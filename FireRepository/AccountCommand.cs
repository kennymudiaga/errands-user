using Errands.Users.Domain.Commands;
using Errands.Users.Domain.Entities;
using Errands.Users.Domain.Exceptions;
using Errands.Users.Domain.Models;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
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
            //await accountDoc.CreateAsync(new
            //{
            //    account.Name,
            //    account.Email,
            //    account.Username,
            //    account.Phone,
            //    account.DateCreated,
            //    account.LastLogin,
            //    account.Roles
            //});
            var data = account.ToDictionary();            
            var result = await accountDoc.CreateAsync(data);
            
            return new LoginResponse
            {
                Email = account.Email,
                Name = account.Name,
                Username = account.Username
            };
        }
    }
}
