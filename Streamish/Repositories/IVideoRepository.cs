using System;
using System.Collections.Generic;

using Streamish.Models;

namespace Streamish.Repositories
{
    public interface IVideoRepository
    {
        void Add(Video video);
        void Delete(int id);
        List<Video> Search(string criterion, bool sortDescending);
        List<Video> Since(DateTime? date);
        List<Video> GetAll();
        List<Video> GetAllWithComments();
        Video GetById(int id);
        Video GetByIdWithComments(int id);
        void Update(Video video);
    }
}