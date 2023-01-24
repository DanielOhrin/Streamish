using System.Collections.Generic;

using Streamish.Models;

namespace Streamish.Repositories
{
    public interface IUserProfileRepository
    {
        void Add(UserProfile profile);
        void Delete(int id);
        List<UserProfile> GetAll();
        UserProfile GetByIdWithVideos(int id);
        UserProfile GetByFirebaseUserId(string firebaseUserId);
        UserProfile GetById(int id);
        void Update(UserProfile profile);
    }
}