using Api.Base;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "1.0")]
    public class BooksController : BaseController<Book>
    {
        public BooksController(BaseService<Book> bookService) : base(bookService)
        {

        }
    }

}