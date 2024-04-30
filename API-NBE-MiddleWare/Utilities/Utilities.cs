namespace API_NBE_MiddleWare.Utilities
{
    public class Utilities
    {
        public async Task<Tuple<bool,byte[]>> DownloadImage(string URL)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(URL);

                if(response.IsSuccessStatusCode)
                {
                    return new Tuple<bool, byte[]> ( true, await response.Content.ReadAsByteArrayAsync() );
                }
                else
                {
                    throw new Exception(response.StatusCode.ToString() +" , " + response.Content.ToString());
                }
            }
            catch (Exception ex)
            {
                return new Tuple<bool, byte[]>(false,new byte[] {});
            }

        }
         
    }
}
