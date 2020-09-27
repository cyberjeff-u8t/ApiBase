using Api.Base;
using Api.Configuration;
using Api.Models;

namespace Api.Services
{
    public class BookService : BaseService<Book>
    {

        public BookService(BookstoreDatabaseSettings settings) : base(settings)
        {

        }

    
    }
}