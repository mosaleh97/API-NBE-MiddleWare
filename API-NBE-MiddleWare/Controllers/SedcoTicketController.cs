using API_NBE_MiddleWare.DatabaseSolutions;
using API_NBE_MiddleWare.SedcoSolutions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace API_NBE_MiddleWare.Controllers
{
    [ApiController]
    [Route("api/sedco/[controller]")]
    public class SedcoTicketController : ControllerBase
    {
        
        [HttpPost("newticket")]
        public async Task<IActionResult> NewTicket([FromBody] Models.TicketRequest request)
        {
            // Extract parameters from the JSON data
            string name = string.Empty;
            string id = string.Empty;
            string phone = string.Empty;
            string service = string.Empty;
            string branch = string.Empty;
            string successFlow = string.Empty;
            string failureFlow = string.Empty;
            string successMessage = string.Empty;
            string failureMessage = string.Empty;
            try
            {
                name = request.Name;
                id = request.ID;
                phone = request.Phone;
                service = request.Service;
                branch = request.Branch;
                successFlow = request.SuccessFlow;
                failureFlow = request.FailureFlow;
                successMessage = request.SuccessMessage;
                failureMessage = request.FailureMessage;

                // integrate with sedco to get new ticket number
                Sedco sedco = new Sedco();
                Tuple<bool, string> newTicketNumber = await sedco.Ticket(Name: name,Id: id, Phone: phone,Service: service,Branch: branch);

                if (newTicketNumber.Item1)
                {
                    //DB transaction insert
                    NbeSedcoDB nbeSedcoDB = new NbeSedcoDB();
                    await nbeSedcoDB.InsertNewTicketRequest(request,newTicketNumber.Item2);

                    return Ok(
                        new
                        {
                            attributes = new
                            {
                                TicketNumber = newTicketNumber.Item2,
                                ReservationSuccessMessage = newTicketNumber.Item2 + " " + "لقد تم حجز دور بنجاح برقم",
                            },
                            FlowName = successFlow,
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
                    throw new Exception(newTicketNumber.Item2);
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
                            TicketNumber = string.Empty,
                            ReservationFailureMessage = "لقد حدث خطأ ما, يرجى المحاولة لاحقا",

                        },
                        FlowName = failureFlow,
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
        }

    }

}
