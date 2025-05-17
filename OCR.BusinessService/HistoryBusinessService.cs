using OCR.Model;
using OCR.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR.BusinessService
{
    public interface IHistoryBusinessService
    {
        List<FileItem> GetLast10Files(int UserId);
        string GetExtractedText(int fileId);
    }
    public class HistoryBusinessService : IHistoryBusinessService
    {
        private readonly IHistoryService _historyService;

        public HistoryBusinessService(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        public List<FileItem> GetLast10Files(int UserId)
        {
            return _historyService.GetLast10Files(UserId);
        }

        public string GetExtractedText(int fileId)
        {
            return _historyService.GetExtractedText(fileId);
        }
    }

}
