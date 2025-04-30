using Microsoft.AspNetCore.Mvc;
using OCR.Model;
using OCR.BusinessService;

namespace OCR.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly OCRBusinessService _fileManager;

        public FileUploadController(IConfiguration configuration)
        {
            _fileManager = new OCRBusinessService(configuration);
        }

        // Upload image
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { status = "Error", message = "No file uploaded." });


                string uploadsFolder = @"D:\Users\daffny\Desktop\Deepika\OCRSolution\Uploads";
                // Directory.CreateDirectory(uploadsFolder); 

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string fullPath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file to disk
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Save to DB
                var model = new ImageUploadModel
                {
                    FileName = uniqueFileName,
                    FilePath = fullPath
                };

                var result = _fileManager.UploadFile(model);

                if (result.statusCode >1 && result.statusCode < 0)
                {
                    return StatusCode(500, new
                    {
                        Status = "Error",
                        //SatausCode = result.statusCode,
                        Message = result.errorMessage
                    });
                }
                return Ok(new
                {
                    StatusCode = "Success",
                    Message = "File upload successfully!"

                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { status = "Error", message = "Internal server error: " + ex.Message });
            }
            
        }      

        // Get extracted Text from DB
        [HttpGet("get-extracted-text")]
        public IActionResult GetExtractedText()
        {
            // Step 1: Save extracted text to DB before retrieving
            var (statusCode, message) = _fileManager.SaveExtractedText();

            if (statusCode != 0)
            {
                return StatusCode(500, new { status = "Error", code = statusCode, message });
            }
            // Step 2: Get extracted text from DB
            var result = _fileManager.GetExtractedText();

            if (result.StatusCode == 0)
                return Ok(result);
            else
                return StatusCode(500, result);
        }

        // Download Docx
        [HttpGet("download-docx")]
        public IActionResult DownloadDocx()
        {
            //Step 2: Get ExtractedText fron the DB
            var result = _fileManager.GetExtractedText();
          
            if (result == null || string.IsNullOrEmpty(result.ExtractedText))
            {
                return NotFound("Extracted text not found.");
            }
            
            //Step 2: Download Docx
            var fileData = _fileManager.DownloadDocx(result.ExtractedText);

            return File(fileData.FileContent, fileData.ContentType, fileData.FileName);
        }

        //Download PDF
        [HttpGet("download-pdf")]
        public IActionResult DownloadPdf()
        {
            // Step 1: Get the extracted text from DB
            var result = _fileManager.GetExtractedText();

            if (result == null || string.IsNullOrEmpty(result.ExtractedText))
            {
                return NotFound("Extracted text not found.");
            }
            
            // Step 2: Download PDF
            var fileData = _fileManager.DownloadPdf(result.ExtractedText);

            return File(fileData.FileContent, fileData.ContentType, fileData.FileName);
        }

    }

}
