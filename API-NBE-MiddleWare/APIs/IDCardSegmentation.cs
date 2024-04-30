namespace API_NBE_MiddleWare.APIs
{
    public class IDCardSegmentation
    {
        public async Task<Tuple<bool, byte[]>> GetIdCardAsync(byte[] inputImage)
        {
            try
            {

                IConfiguration configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                   .Build();

                using (var client = new HttpClient())
                {
                    // Set the base address of the API
                    client.BaseAddress = new Uri(configuration["IDCardSegmentationAPI:BaseURL"]);

                    // Define the request content
                    var content = new MultipartFormDataContent();
                    byte[] imageData = inputImage;
                    ByteArrayContent imageContent = new ByteArrayContent(imageData);
                    imageContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/jpeg");
                    content.Add(imageContent, "image", "curretImage.jpg");

                    // Set the accept header
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    // Make the POST request
                    HttpResponseMessage response = await client.PostAsync(configuration["IDCardSegmentationAPI:Route"], content);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response as a byte array
                        return new Tuple<bool, byte[]>(true, await response.Content.ReadAsByteArrayAsync());

                    }
                    else
                    {
                        throw new Exception($"Error: {response.StatusCode} , {response.Content.ToString()}");
                    }
                }
            }
            catch (Exception)
            {

                return new Tuple<bool, byte[]>(false, null!);
            }
        }
    }
}



