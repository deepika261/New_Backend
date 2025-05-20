using System.Data;
using System.Data.SqlClient;
using OCR.Model;
using Microsoft.Extensions.Configuration;
using Document = iTextSharp.text.Document;
using PdfWriter = iTextSharp.text.pdf.PdfWriter;
using Paragraph = iTextSharp.text.Paragraph;
using System.Diagnostics;
using DocumentFormat.OpenXml.Packaging;
using System.Runtime.CompilerServices;



namespace OCR.Provider
{
    public interface IOcrProvider
    {
        (int StatusCode, string ErrorMessage) UploadFile(ImageUploadModel model, int UserId);
        string RunPythonScript(int UserId);
        (int, string) SaveExtractedText(int UserId);
        TextExtractedModel GetExtractedText(int UserId);
        TextDownloadModel DownloadDocx(string extractedText);
        TextDownloadModel DownloadPdf(string extractedText);
    }
    public class OcrProvider : IOcrProvider
    {
        private readonly string _connectionString;

        public OcrProvider(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public (int StatusCode, string ErrorMessage) UploadFile(ImageUploadModel model, int UserId)
        {
            int statusCode = -1;
            string errorMessage = "";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ocr_insertfile", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_UserId",UserId);
                    cmd.Parameters.AddWithValue("@p_FileName", model.FileName);
                    cmd.Parameters.AddWithValue("@p_FilePath", model.FilePath);

                     SqlParameter outputCode = new SqlParameter("@p_output_status_code", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                     };
                    SqlParameter outputMsg = new SqlParameter("@p_ouput_error_message", SqlDbType.NVarChar, 100)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmd.Parameters.Add(outputCode);
                    cmd.Parameters.Add(outputMsg);

                    conn.Open();
                    cmd.ExecuteNonQuery();  

                    statusCode = outputCode.Value != DBNull.Value ? Convert.ToInt32(outputCode.Value) : -1;
                    //errorMessage = outputMsg.Value != DBNull.Value ? outputMsg.Value.ToString() : "";
                    errorMessage = outputMsg.Value.ToString();
                }
            }

            return (statusCode, errorMessage);
        }

       /* public string RunPythonScript()
        {
            string pythonExePath = @"C:\Program Files\Python313\python.exe"; 
            string scriptPath = @"D:\Users\daffny\Desktop\Deepika\OCRSolution\PythonScripts\ocr_processor.py";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = pythonExePath,
                Arguments = $"\"{scriptPath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    return result.Trim(); // Clean whitespace
                }
            }
        }*/

        public string RunPythonScript(int UserId)
        {
            string FilePath = string.Empty;
            //ImageUploadModel result = new ImageUploadModel();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ocr_getfilepath", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_UserId", UserId);
                cmd.Parameters.Add("@p_output_status_code", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_ouput_error_message", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        FilePath = reader["FilePath"]?.ToString();
                    }
                }

                //result.StatusCode = Convert.ToInt32(cmd.Parameters["@p_output_status_code"].Value);
                //result.Message = cmd.Parameters["@p_ouput_error_message"].Value.ToString();
            }

            if (string.IsNullOrEmpty(FilePath))
            {
                throw new Exception("File path could not be retrieved from the database.");
            }

            string pythonExePath = @"C:\Program Files\Python313\python.exe";
            string scriptPath = @"D:\Users\daffny\Desktop\Deepika\OCRSolution\PythonScripts\OCR1.py";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = pythonExePath,
                Arguments = $"\"{scriptPath}\" \"{FilePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true, //also capture errors
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(errors))
                {
                    throw new Exception($"Python script error: {errors}");
                }

                return output.Trim(); // clean whitespace
            }
        }


        public (int, string) SaveExtractedText(int UserId)
        {
            int statusCode = -1;
            string errorMessage = "";

            // Get extracted text by calling Python
            string extractedText = RunPythonScript(UserId);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ocr_saveextractedtext", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_ExtractedText", extractedText);
                cmd.Parameters.AddWithValue("@p_UserId",UserId);
                cmd.Parameters.Add("@p_output_status_code", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_ouput_error_message", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();

                statusCode = Convert.ToInt32(cmd.Parameters["@p_output_status_code"].Value);
                errorMessage = cmd.Parameters["@p_ouput_error_message"].Value.ToString();
            }

            return (statusCode, errorMessage);
        }

        public TextExtractedModel GetExtractedText(int UserId)
        {
            TextExtractedModel result = new TextExtractedModel();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_ocr_getextractedtext", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_UserId",UserId);
                cmd.Parameters.Add("@p_output_status_code", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_ouput_error_message", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result.ExtractedText = reader["ExtractedText"]?.ToString();
                    }
                }

                result.StatusCode = Convert.ToInt32(cmd.Parameters["@p_output_status_code"].Value);
                result.Message = cmd.Parameters["@p_ouput_error_message"].Value.ToString();
            }

            return result;
        }

        public TextDownloadModel DownloadDocx(string extractedText)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Create(stream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                    var body = mainPart.Document.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Body());
                    var para = body.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph());
                    para.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(extractedText)));
                }

                return new TextDownloadModel
                {
                    FileContent = stream.ToArray(),
                    FileName = $"ExtractedText.docx",
                    ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                };
            }
        }

        public TextDownloadModel DownloadPdf(string extractedText)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document();
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();
                
                // Add content
                document.Add(new Paragraph(extractedText));

                document.Close();
                writer.Close();

                return new TextDownloadModel
                {
                    FileContent = ms.ToArray(),
                    //FileName = fileName + ".pdf",
                    FileName = "ExtractedText.pdf",
                    ContentType = "application/pdf"
                };
            }
        }


    }


}
