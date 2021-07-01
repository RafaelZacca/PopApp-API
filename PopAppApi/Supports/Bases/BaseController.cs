using Core.Support.Interfaces;
using Core.Supports.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace API.Supports.Bases
{
    public abstract class BaseController<T, Y> : ControllerBase
        where T : class
        where Y : class, IBizLogic<T>
    {
        protected Y BizLogic;
        protected readonly ILogger<BaseController<T, Y>> _logger;

        public BaseController(ILogger<BaseController<T, Y>> logger)
        {
            _logger = logger;
        }

        public virtual async Task<IActionResult> GetAll([FromQuery] IDictionary<string, string> query = null)
        {
            try
            {
                return Ok(await BizLogic.GetAll(query));
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

        public virtual async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await BizLogic.Get(id));
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

        public virtual async Task<IActionResult> Post([FromBody] T request)
        {
            try
            {
                return Ok(await BizLogic.Insert(request));
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

        public virtual async Task<IActionResult> Put(int id, [FromBody] T request)
        {
            try
            {
                return Ok(await BizLogic.Update(id, request));
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

        public virtual async Task<IActionResult> Delete(int id)
        {
            try
            {
                await BizLogic.Delete(id);
                return Ok();
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
