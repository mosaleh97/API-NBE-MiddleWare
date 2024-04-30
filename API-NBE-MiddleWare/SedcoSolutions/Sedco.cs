namespace API_NBE_MiddleWare.SedcoSolutions
{
    public class Sedco
    {

        /*Tuple bool: indicate if success or failure
         * if success -> bool=true and string = ticket number
         * else -> bool = false and string= exception msg.
        */
        public async Task<Tuple<bool, string>> Ticket(string Name,string Id, string Phone, string Service,string Branch)
        {
            try
            {
                IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .Build();

                Dictionary<string,int> Services = configuration.GetSection("ServiceTypes").GetChildren().ToDictionary(x=>x.Key, x => int.Parse(x.Value));
                Dictionary<string, int> Branches = configuration.GetSection("Branches").GetChildren().ToDictionary(x => x.Key, x => int.Parse(x.Value));

                Sedco80.CustomerVisitServiceClient custom = new Sedco80.CustomerVisitServiceClient();
                Sedco80.clsTicket Issueticket = new Sedco80.clsTicket();
                Sedco80.clsCustomer customer = new Sedco80.clsCustomer();
                string segmentation = string.Empty;
                
                customer.Name = Name;
                customer.MobileNo = Phone;
                customer.ID = Id;
                int targetService = Services[Service];
                int targetBranch = Branches[Branch];

                Sedco80.IssueTicketRequest issueTicketRequest =
                    new Sedco80.IssueTicketRequest("EN", targetBranch, targetService, segmentation, customer, Issueticket);
                var result = await custom.IssueTicketAsync(issueTicketRequest);
                if (result.IssueTicketResult.Code == 0)
                {
                    return new Tuple<bool, string>(true, result.TicketInfo.Number);
                }
                else
                {
                    throw new Exception(result.IssueTicketResult.Description);
                }
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
        }



    }
}
