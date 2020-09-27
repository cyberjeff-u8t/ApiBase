using Api.Base;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "1.0")]
    public class FilmsController : BaseController<Film>
    {
        private FilmService _filmService;

        public FilmsController(BaseService<Film> filmService) : base(filmService)
        {
            this._filmService = (FilmService)filmService;
        }

        [HttpGet("byAuthor/{author}", Name="GetByAuthor")]
        [MapToApiVersion("2.0")]
        [ApiExplorerSettings(GroupName = "2.0")]
        public ActionResult<List<Film>> GetByAuthor(string author)
        {
            var entities = _filmService.GetByAuthor(author);
            
            if (entities == null) {
                return NotFound();
            }

            return entities;
        }


    }
}
