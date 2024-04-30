using System.Data.SqlClient;

namespace API_NBE_MiddleWare.DatabaseSolutions
{
    public class NbeSedcoDB
    {
        public async Task<Tuple<bool, string>> InsertNewTicketRequest(Models.TicketRequest requestInfo, string ticketNumber)
        {
            SqlConnection? conn = null;
            try
            {
                IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .Build();

                var dbConnString = configuration.GetConnectionString("NbeSedcoConnectionString");
                conn = new SqlConnection(dbConnString);
                string insertCommandText = "INSERT INTO Tickets (CustomerName,CustomerPhone,CustomerNationalId,ServiceName,Branch,TicketNumber,ReservationTime) VALUES" +
                    " (@CustomerName,@CustomerPhone,@CustomerNationalId,@ServiceName,@Branch,@TicketNumber,@ReservationTime)";

                conn.Open();

                SqlCommand command = new SqlCommand(insertCommandText, conn);
                command.Parameters.AddWithValue("@CustomerName", requestInfo.Name);
                command.Parameters.AddWithValue("@CustomerPhone", requestInfo.Phone);
                command.Parameters.AddWithValue("@CustomerNationalId", requestInfo.ID);
                command.Parameters.AddWithValue("@ServiceName", requestInfo.Service);
                command.Parameters.AddWithValue("@Branch", requestInfo.Branch);
                command.Parameters.AddWithValue("@TicketNumber", ticketNumber);
                command.Parameters.AddWithValue("@ReservationTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"));

                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    return new Tuple<bool, string>(true, "New row had been added successfully to database");
                }
                else
                {
                    throw new Exception("No rows added to database!");
                }


            }
            catch (Exception ex)
            {

                return new Tuple<bool, string>(false, ex.Message);
            }
            finally
            {
                conn?.Close();
            }
        }
    }
}
