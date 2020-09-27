using Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error([FromServices] IWebHostEnvironment webHostEnvironment)
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exceptionType = context.Error.GetType();

            if (exceptionType == typeof(ArgumentException)
                || exceptionType == typeof(ArgumentNullException)
                || exceptionType == typeof(ArgumentOutOfRangeException))
            {
                if (!webHostEnvironment.IsDevelopment())
                {
                    return ValidationProblem("An exception occured due to an incorrection argument",
                        title: context.Error.Message,
                        type:"Argument Exception");
                }

                return ValidationProblem(context.Error.Message);
            }

            if (exceptionType == typeof(NotFoundException)) {
                return NotFound(context.Error.Message);
            }

            if (exceptionType == typeof(TimeoutException)){
                return Problem(type:"Database", 
                               title: "Connect to database timeout");
            }

           if (!webHostEnvironment.IsDevelopment())
            {
                return Problem("An exception occured due to an incorrection argument",
                                   title: context.Error.Message, 
                                   type:"Unhandled Exception");
            }

            return Problem();
        }
    }


}