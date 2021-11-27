using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transaction.BL;
using Transaction.DAL.EF;
using Microsoft.Extensions.DependencyInjection; 
using System.ComponentModel.DataAnnotations;
using Transaction.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace Transaction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly RRContext _context;
        private readonly ILogger<TransactionController> _logger;
        public TransactionController(RRContext context, ILogger<TransactionController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile([Required] IFormFile file)
        {
            try
            {
                string result = await new TransactionManage(_context, _logger).UpdateFile(file);

                if (result == "Success")
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
             
        }

        [SwaggerResponse(200, Type = typeof(List<TransactionModel>), Description = "Get Data List.")]
        [HttpPost("GetDataFromCurrency")]
        public async Task<IActionResult> GetDataFromCurrency([FromBody] TransactionCurrencyParamModel Model)
        {
            try
            {
                var result = await new TransactionManage(_context, _logger).GetAllTransactionListFromCurrency(Model.CurrencyCode);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDataFromDate")]
        public async Task<IActionResult> GetDataFromDate([FromBody] TransactionDateParamModel TrDate)
        {
            try
            {
                var result = await new TransactionManage(_context, _logger).GetAllTransactionListFromDate(TrDate.StartDate, TrDate.EndDate);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDataFromStatus")]
        public async Task<IActionResult> GetDataFromStatus([FromBody] TransactionStatusParamModel Model)
        {
            try
            {
                var result = await new TransactionManage(_context, _logger).GetAllTransactionListFromStatus(Model.Status);
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
