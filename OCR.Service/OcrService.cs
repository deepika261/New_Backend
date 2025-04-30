using OCR.Provider;
using OCR.Model;
using Microsoft.Extensions.Configuration;
namespace OCR.Service
{
    public interface IOcrService
    {
        (int statusCode, string errorMessage) UploadFile(ImageUploadModel model);
        (int, string) SaveExtractedText();
        TextExtractedModel GetExtractedText();
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

        public (int statusCode, string errorMessage) UploadFile(ImageUploadModel model)
        {
            return _provider.UploadFile(model);
        }
        public (int, string) SaveExtractedText()
        {
            return _provider.SaveExtractedText();
        }
        public TextExtractedModel GetExtractedText()
        {
            return _provider.GetExtractedText();
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
