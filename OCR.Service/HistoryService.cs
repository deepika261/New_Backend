using OCR.Model;
using OCR.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR.Service
{
    public interface IHistoryService
    {
        List<FileItem> GetLast10Files(int UserId);
        string GetExtractedText(int fileId);
    }
    public class HistoryService : IHistoryService
    {
        private readonly IHistoryProvider _historyProvider;

        public HistoryService(IHistoryProvider historyProvider)
        {
            _historyProvider = historyProvider;
        }

        public List<FileItem> GetLast10Files(int UserId)
        {
            return _historyProvider.GetLast10Files(UserId);
        }

        public string GetExtractedText(int fileId)
        {
            return _historyProvider.GetExtractedText(fileId);
        }
    }
}
