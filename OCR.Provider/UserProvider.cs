using Microsoft.Extensions.Configuration;
using OCR.Model;
using System.Data.SqlClient;
using System.Data;
using Chessie.ErrorHandling;
using DocumentFormat.OpenXml.Presentation;

namespace OCR.Provider
{
    public class UserProvider
    {
        private readonly string _connectionString;

        private readonly IConfiguration _configuration;
        public UserProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public UserModel UserRegister(UserModel user)
        {   var result = new UserModel();
            int statusCode = -1,outUserId;
            string errorMessage = "";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                //UserModel result = new UserModel();
                using (SqlCommand cmd = new SqlCommand("sp_RegisterUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_Email", user.Email);
                    cmd.Parameters.AddWithValue("@p_PasswordHash", user.PasswordHash);
                    cmd.Parameters.Add("@p_UserId", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@p_output_status_code", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@p_ouput_error_message", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    result.UserId = Convert.ToInt32(cmd.Parameters["@p_UserId"].Value);
                    result.StatusCode= Convert.ToInt32(cmd.Parameters["@p_output_status_code"].Value);
                    result.ErrorMessage = cmd.Parameters["@p_ouput_error_message"].Value.ToString();

                    
                    /*int rows = cmd.ExecuteNonQuery();
                    outUserId = Convert.ToInt32(cmd.Parameters["@p_UserId"].Value);
                    statusCode = Convert.ToInt32(cmd.Parameters["@p_output_status_code"].Value);
                    errorMessage = cmd.Parameters["@p_ouput_error_message"].Value.ToString();
                    if (statusCode == 1)
                    {
                        result.StatusCode = statusCode;
                        result.ErrorMessage = errorMessage;
                        result.UserId = outUserId;
                        
                            }
                    
                    return statusCode==1;*/
                }

                return result;

            }
        }

            public UserModel UserLogin(string email)
        {
            int statusCode = -1;
            string errorMessage = "";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_LoginUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_Email", email);
                    cmd.Parameters.Add("@p_output_status_code", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@p_output_error_message", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
                    
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            statusCode = Convert.ToInt32(cmd.Parameters["@p_output_status_code"].Value);
                            errorMessage = cmd.Parameters["@p_output_error_message"].Value.ToString();
                            return new UserModel
                            {
                                UserId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : 0,
                                Email = email,
                                PasswordHash = reader["PasswordHash"].ToString(),
                                StatusCode= statusCode,
                                ErrorMessage= errorMessage
                            };
                        }
                        else
                        {
                            return new UserModel
                            {
                                StatusCode = Convert.ToInt32(cmd.Parameters["@p_output_status_code"].Value),
                                ErrorMessage = cmd.Parameters["@p_output_error_message"].Value.ToString()

                            };
                        }
                    }
                   
                }
            }
        }
    }


}
