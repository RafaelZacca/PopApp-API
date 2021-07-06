using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Database.Models;
using API.Supports.Bases;
using Core.BizLogics;
using Database.Supports.Contexts;
using Microsoft.Extensions.Configuration;
using Core.Supports.Exceptions;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RecognitionsController : BaseController<RecognitionModel, RecognitionsBizLogic>
    {
        public RecognitionsController(ILogger<RecognitionsController> logger, PopAppContext context, IConfiguration configuration) : base(logger)
        {
            BizLogic = new RecognitionsBizLogic(configuration, context);
        }

        [NonAction]
        public override Task<IActionResult> Get(int id)
        {
            return base.Get(id);    
        }

        [NonAction]
        public override Task<IActionResult> GetAll([FromQuery] IDictionary<string, string> query = null)
        {
            return base.GetAll(query);
        }

        [NonAction]
        public override Task<IActionResult> Post([FromBody] RecognitionModel request)
        {
            return base.Post(request);
        }

        [NonAction]
        public override Task<IActionResult> Put(int id, [FromBody] RecognitionModel request)
        {
            return base.Put(id, request);
        }

        [NonAction]
        public override Task<IActionResult> Delete(int id)
        {
            return base.Delete(id);
        }

        [HttpPost]
        public async Task<IActionResult> Recognize([FromBody] RecognitionModel request)
        {
            try
            {
                var result = await BizLogic.InsertAndRecognize(request);
                return Ok(result);
            }
            catch (HttpFailedStatusException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Something wrong happened in our servers, please try again or contact an administrator");
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}/mp3")]
        public async Task<IActionResult> GetMp3 (int id)
        {
            try
            {
                var bytes = await BizLogic.GetRecognitionBytes(id);
                return File(bytes, "audio/mpeg", $"music.mp3", true);
            }
            catch (HttpFailedStatusException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Something wrong happened in our servers, please try again or contact an administrator");
            }
        }
    }
}
