using Api.Interfaces;
using Api.Exceptions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Api.ErrorHandling;
using Newtonsoft.Json;

namespace Api.Base
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController<T> : ControllerBase where T : IBaseModel
    {
        private readonly IBaseService<T> _service;
        private const string _getRouteName = "Get_" + nameof(BaseController<T>);

        protected BaseController(IBaseService<T> service) {
            this._service = service;
        }

        [HttpGet]
        public ActionResult<List<T>> Get() =>  _service.Get();

        [HttpGet("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public ActionResult<T> Get(string id)
        {
            var entity = _service.Get(id);

            if (entity == null) {
                return NotFound();
            }
            return entity;
        }

        [HttpPost]
        public ActionResult<T> Create(T entity)
        {
            _service.Create(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id.ToString() }, entity);
        }

        [HttpPut("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public IActionResult Update(string id, T entityIn)
        {
            var entity = _service.Get(id);

            if (entity == null) {
                return NotFound();
            }

            _service.Update(id, entityIn);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResponse), StatusCodes.Status404NotFound)]
        public IActionResult Delete(string id)
        {
            var entity = _service.Get(id);

            if (entity == null){
                return NotFound();
            }

            _service.Remove(entity.Id);
            return NoContent();
        }
    }
}