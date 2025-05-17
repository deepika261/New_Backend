using Microsoft.Extensions.Configuration;
using OCR.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR.Provider
{
    public interface IHistoryProvider
    {
        List<FileItem> GetLast10Files(int UserId);
        string GetExtractedText(int fileId);
    }
    public class HistoryProvider : IHistoryProvider
    {
        private readonly IConfiguration _configuration;

        public HistoryProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<FileItem> GetLast10Files(int UserId)
        {
            var result = new List<FileItem>();
            //result = new List<FileItem>();
            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            using (var cmd = new SqlCommand("sp_ocr_history", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_UserId", UserId);
                cmd.Parameters.Add("@p_output_status_code", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@p_ouput_error_message", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new FileItem
                        {
                            
                            FilePath = reader["FilePath"].ToString(),
                            ExtractedText = reader["ExtractedText"].ToString()
                        });

                    }
                }
            }
            return result;
        }

        public string GetExtractedText(int fileId)
        {
            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            using (var cmd = new SqlCommand("sp_GetExtractedTextByFileId", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FileId", fileId);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return result?.ToString() ?? string.Empty;
            }
        }
    }

}
