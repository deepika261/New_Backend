using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR.Model
{
    public class FileItem
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ExtractedText { get; set; }
        public DateTime UploadedDate { get; set; }
        public string Email { get; set; }
    }
}
