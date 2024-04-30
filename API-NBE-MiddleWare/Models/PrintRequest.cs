namespace API_NBE_MiddleWare.Models
{
    public class PrintRequest
    {
        public string FormName { get; set; }
        public string Name { get; set; }
        public string NationalID { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Branch { get; set; }
        public string ImgURL { get; set; }

        public string SuccessFlow { get; set; }
        public string FailureFlow { get; set; }
        public string SuccessMessage { get; set; }
        public string FailureMessage { get; set; }
    }

}
