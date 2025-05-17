using System;
using OCR.Service;
using OCR.Model;
using Microsoft.Extensions.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;

namespace OCR.BusinessService
{
    public interface IFileManager
    {
        (int statusCode, string errorMessage) UploadFile(ImageUploadModel model,int UserId);
        (int, string) SaveExtractedText(int UserId);
        TextExtractedModel GetExtractedText(int UserId);
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
        

        
        public (int statusCode, string errorMessage) UploadFile(ImageUploadModel model, int UserId)
        {
            try 
            {
                return _service.UploadFile(model, UserId);
            }
            catch (Exception ex)
            {
                return (500, $"Upload failed: {ex.Message}");
            }
                
        }
        public (int, string) SaveExtractedText(int UserId)
        {
            return _service.SaveExtractedText(UserId);
        }
        public TextExtractedModel GetExtractedText(int UserId)
        {
            return _service.GetExtractedText(UserId);
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
