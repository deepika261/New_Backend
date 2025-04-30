using System;
using OCR.Service;
using OCR.Model;
using Microsoft.Extensions.Configuration;

namespace OCR.BusinessService
{
    public interface IFileManager
    {
        (int statusCode, string errorMessage) UploadFile(ImageUploadModel model);
        (int, string) SaveExtractedText();
        TextExtractedModel GetExtractedText();
        TextDownloadModel DownloadDocx(string extractedText);
        TextDownloadModel DownloadPdf(string extractedText);


    }

    public class OCRBusinessService : IFileManager
    {
        private readonly OcrService _service;

        public OCRBusinessService(IConfiguration configuration)
        {
            _service = new OcrService(configuration);
        }

        public (int statusCode, string errorMessage) UploadFile(ImageUploadModel model)
        {
            try 
            {
                return _service.UploadFile(model);
            }
            catch (Exception ex)
            {
                return (500, $"Upload failed: {ex.Message}");
            }
                
        }
        public (int, string) SaveExtractedText()
        {
            return _service.SaveExtractedText();
        }
        public TextExtractedModel GetExtractedText()
        {
            return _service.GetExtractedText();
        }
        public TextDownloadModel DownloadDocx(string extractedText)
        {
            return _service.DownloadDocx(extractedText);    
        }

        public TextDownloadModel DownloadPdf(string extractedText)
        {
            return _service.DownloadPdf(extractedText);
        }
    }

}
