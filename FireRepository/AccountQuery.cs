using Errands.Users.Domain.Entities;
using Errands.Users.Domain.Models;
using Errands.Users.Domain.Queries;
using Google.Cloud.Firestore;
using System.Linq;
using System.Threading.Tasks;

namespace FireRepository
{
    public class AccountQuery : IAccountQuery
    {
        private readonly CollectionReference accountsCollection;
        public AccountQuery(FirestoreDb firestoreDb)
        {
            accountsCollection = firestoreDb.Collection("user-accounts");
        }
        public async Task<bool> IsEmailAvailable(string email)
        {
            var userQuery = await accountsCollection.WhereEqualTo("Email", email).Select("Email").Limit(1).GetSnapshotAsync();
            return userQuery.Documents.Count == 0;
        }

        public async Task<bool> IsUsernameAvailable(string username)
        {
            var userQuery = await accountsCollection.WhereEqualTo("Username", username).Select("Username").Limit(1).GetSnapshotAsync();
            return userQuery.Documents.Count == 0;
        }

        public async Task<LoginResponse> GetUser(string username)
        {
            var userQuery = await accountsCollection.WhereEqualTo(nameof(Account.Username), username)                
                .Select(nameof(Account.Id), nameof(Account.Name), nameof(Account.Email), nameof(Account.Username))
                .Limit(1).GetSnapshotAsync();
            var accountDoc = userQuery.Documents.FirstOrDefault();
            if (accountDoc == null) return null;
            var account = accountDoc.Parse<Account>();
            return new LoginResponse
            {
                Email = account.Email,
                Name = account.Name,
                Username = account.Username
            };
        }
    }
}
