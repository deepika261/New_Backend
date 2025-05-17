using OCR.Provider;
using OCR.Model;
using Microsoft.Extensions.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;
namespace OCR.Service
{
    public interface IOcrService
    {
        (int statusCode, string errorMessage) UploadFile(ImageUploadModel model, int UserId);
        (int, string) SaveExtractedText(int UserId);
        TextExtractedModel GetExtractedText(int UserId);
        TextDownloadModel DownloadDocx(string extractedText);
        TextDownloadModel DownloadPdf(string extractedText);
        

    }
    public class OcrService : IOcrService
    {
        private readonly OcrProvider _provider;

        public OcrService(IConfiguration configuration)
        {
            _provider = new OcrProvider(configuration);
        }

        public (int statusCode, string errorMessage) UploadFile(ImageUploadModel model, int UserId)
        {
            return _provider.UploadFile(model, UserId);
        }
        public (int, string) SaveExtractedText(int UserId)
        {
            return _provider.SaveExtractedText(UserId);
        }
        public TextExtractedModel GetExtractedText(int UserId)
        {
            return _provider.GetExtractedText(UserId);
        }
        public TextDownloadModel DownloadDocx(string extractedText)
        {
            return _provider.DownloadDocx(extractedText);
        }
        public TextDownloadModel DownloadPdf(string extractedText)
        {
            return _provider.DownloadPdf(extractedText);
        }
       

        

    }

}
