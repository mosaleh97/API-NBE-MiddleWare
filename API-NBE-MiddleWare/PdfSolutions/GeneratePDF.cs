using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace API_NBE_MiddleWare.PdfSolutions
{
    public class GeneratePDF
    {
        public Tuple<bool, string> CreditForm(string Branch, string Date, string ID, string Name, string Amount, string Currency,byte[] imgBytes)
        {

            try
            {
                /*Form Fields
                    Text-Branch
                    Text-Date
                    Text-ID
                    Text-Name
                    Text-Amount
                    Text-Currency
                */
                
                string path = @"Assets\\CreditForm.pdf";
                string path_Result = @"Assets\\CreditForm-Result.pdf";

                if (File.Exists(path_Result))
                {
                    File.Delete(path_Result);
                }


                var SourceFileStream = File.OpenRead(path);
                var OutputStream = new MemoryStream();

                var pdf = new PdfDocument(new PdfReader(SourceFileStream), new PdfWriter(OutputStream));
                var doc = new Document(pdf);

                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdf, false);

                IDictionary<string, PdfFormField> fields = form.GetAllFormFields();
                PdfFormField? fieldValue = null;

                fields.TryGetValue("Text-Branch", out fieldValue);
                fieldValue!.SetValue(Branch);

                fields.TryGetValue("Text-Date", out fieldValue);
                fieldValue!.SetValue(Date);

                fields.TryGetValue("Text-ID", out fieldValue);
                fieldValue!.SetValue(ID);

                fields.TryGetValue("Text-Name", out fieldValue);
                fieldValue!.SetValue(Name);

                fields.TryGetValue("Text-Amount", out fieldValue);
                fieldValue!.SetValue(Amount);

                fields.TryGetValue("Text-Currency", out fieldValue);
                fieldValue!.SetValue(Currency);

                if (imgBytes != null)
                {
                    Image image = new Image(ImageDataFactory.Create(imgBytes));
                    // Calculate dimensions and position for the image
                    float pageWidth = pdf.GetDefaultPageSize().GetWidth();
                    float pageHeight = pdf.GetDefaultPageSize().GetHeight();
                    float imageWidth = 300; // Set the width of the image
                    float imageHeight = 200; // Set the height of the image
                    float x = (pageWidth - imageWidth) / 2; // Center horizontally
                    float y = 50; // Set the vertical position (adjust as needed)

                    // Set the dimensions of the image
                    image.ScaleToFit(imageWidth, imageHeight);

                    // Add the image to the document at the specified position
                    doc.Add(image.SetFixedPosition(x, y));
                }

                pdf.Close();

                byte[] bytes = OutputStream.ToArray();

                File.WriteAllBytes(path_Result, bytes);

                return new Tuple<bool, string>(true, "File generated successfully");
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, "File generated Failed: "+ex.Message);
            }

        }
    }
}
