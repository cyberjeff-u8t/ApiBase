using Api.Base;
using Api.Configuration;
using Api.Models;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Api.Services
{
    public class FilmService : BaseService<Film> 
    {

        public FilmService(FilmstoreDatabaseSettings settings) : base(settings) {

        }

       public List<Film>GetByAuthor(string author)
        {
            return(this._collection.Find(entity => entity.Author == author).ToList());
        }
    
    }

}