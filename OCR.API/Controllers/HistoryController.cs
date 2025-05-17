using Microsoft.AspNetCore.Mvc;
using OCR.BusinessService;
using OCR.Model;
namespace OCR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryBusinessService _historyBusinessService;

        public HistoryController(IHistoryBusinessService historyBusinessService)
        {
            _historyBusinessService = historyBusinessService;
        }

        [HttpGet("get_history/{UserId}")]
        public IActionResult GetLast10Files(int UserId)
        {
            var files = _historyBusinessService.GetLast10Files(UserId);
            return Ok(files);
        }

        [HttpGet("text/{fileId}")]
        public IActionResult GetExtractedText(int fileId)
        {
            var text = _historyBusinessService.GetExtractedText(fileId);
            return Ok(new { text });
        }
    }

}
