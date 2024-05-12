using API_NBE_MiddleWare.APIs;
using API_NBE_MiddleWare.MailSolutions;
using API_NBE_MiddleWare.PdfSolutions;
using API_NBE_MiddleWare.SedcoSolutions;
using API_NBE_MiddleWare.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace API_NBE_MiddleWare.Controllers
{

    [ApiController]
    [Route("api/printer")]
    public class PrinterController : ControllerBase
    {
        [HttpPost("print")]
        public async Task<IActionResult> Print([FromBody] Models.PrintRequest requestInfo)
        {
            string FormName = string.Empty;
            string Name = string.Empty;
            string NationalID = string.Empty;
            string Amount = string.Empty;
            string Currency = string.Empty;
            string Branch = string.Empty;
            string ImgURL = string.Empty;
            string SuccessFlow = string.Empty;
            string FailureFlow = string.Empty;
            string SuccessMessage = string.Empty;
            string FailureMessage = string.Empty;
            try
            {
                // Extract parameters from the JSON data
                FormName = requestInfo.FormName;
                Name = requestInfo.Name;
                NationalID = requestInfo.NationalID;
                Amount = requestInfo.Amount;
                Currency = requestInfo.Currency;
                Branch = requestInfo.Branch;
                ImgURL = requestInfo.ImgURL;
                SuccessFlow = requestInfo.SuccessFlow;
                FailureFlow = requestInfo.FailureFlow;
                SuccessMessage = requestInfo.SuccessMessage;
                FailureMessage = requestInfo.FailureMessage;

                //Download Image From URL and get segmented id section
                byte[]? imgBytes = null;
                if (!string.IsNullOrEmpty(ImgURL))
                {
                    Utilities.Utilities utilities = new Utilities.Utilities();
                    var imgDownloadResponse = await utilities.DownloadImage(ImgURL);
                    if (!imgDownloadResponse.Item1)
                    {
                        throw new Exception($"Failed to download the image from the target URL {ImgURL}");
                    }
                    imgBytes = imgDownloadResponse.Item2;
                
                    //Get Segmented ID Area
                    IDCardSegmentation iDCardSegmentation = new IDCardSegmentation();
                    var segmentedID = await iDCardSegmentation.GetIdCardAsync(imgBytes);
                    if(!segmentedID.Item1)
                    {
                        throw new Exception("Failed to apply ID card segmentation API");
                    }

                    imgBytes = segmentedID.Item2;
                }
                
                
                //Integrate with pdf solution to generate PDF file
                GeneratePDF generatePDF = new GeneratePDF();
                var pdfGeneration = generatePDF.CreditForm(Branch, DateTime.Now.ToString("dd/MM/yyyy"), NationalID,
                    Name, Amount, Currency,imgBytes!);
                if (!pdfGeneration.Item1)
                {
                    throw new Exception(pdfGeneration.Item2);
                }


                //Integrate with SMTP mail solution to send the generated pdf file to the printer
                SMTP smptp = new SMTP();
                var smtpRequest = smptp.Send();

                // in case of success
                if (smtpRequest.Item1)
                {
                    return Ok(
                        new
                        {
                            attributes = new
                            {
                                PrintStatus = smtpRequest.Item1,
                                PrintFormSuccessMessage = "لقد تمت الطباعة بنجاح",
                            },
                            FlowName = SuccessFlow,
                            FacebookResponse = new
                            {
                                messaging_type = "",
                                recipient = new
                                {
                                    id = ""
                                },
                                message = new
                                {
                                    text = string.Empty,
                                }
                            }
                        }
                        );
                }
                else
                {
                    throw new Exception(smtpRequest.Item2);
                }
            }
            catch (Exception ex)
            {
                // Return a failure response
                return BadRequest(
                    new
                    {
                        attributes = new
                        {
                            PrintStatus = false,
                            PrintFormFailureMessage = "لقد حدث خطأ ما, يرجى المحاولة لاحقا",
                            PrintFormFailureException = ex.Message,
                        },
                        FlowName = FailureFlow,
                        FacebookResponse = new
                        {
                            messaging_type = "",
                            recipient = new
                            {
                                id = ""
                            },
                            message = new
                            {
                                text = string.Empty
                            }
                        }
                    }
                    );
            }
        }

    }


}



