using ScheduleSystemApi.Engine;
using ScheduleSystemApi.Models;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ScheduleSystemApi.Controllers
{
    [RoutePrefix("api/schedulesystem")]
    public class ScheduleSystemController : ApiController
    {
        private readonly IScheduleEngine _engine = 
            new ScheduleEngine(ConfigurationManager.ConnectionStrings["ScheduleSystem"].ConnectionString);

        [Route("patient/{id:int}")]
        [HttpGet]
        public IHttpActionResult GetPatient(int id)
        {
            try
            {
                var result = _engine.GetPatient(id);

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("patient/{id:int}/appointments")]
        [HttpGet]
        public IHttpActionResult GetPatientAppointments(int id)
        {
            try
            {
                var result = _engine.GetPatientAppointments(id);

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("patient/{phoneNumber:int}/appointment")]
        [HttpGet]
        public IHttpActionResult GetNextAppointmentByPatientPhoneNumber(string phoneNumber)
        {
            try
            {
                var result = _engine.GetNextAppointmentByPatientPhoneNumber(phoneNumber);

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("appointment/reschedule")]
        [HttpPost]
        public IHttpActionResult RescheduleAppointment([FromBody] RescheduleMessage message)
        {
            try
            {
                var result = _engine.RescheduleAppointment(message.OldAppointmentId, message.NewAppointment);
                return Created(Request.RequestUri + "/" + result.Id.ToString(), result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("appointment/{id:int}")]
        [HttpGet]
        public IHttpActionResult GetAppointment(int id)
        {
            try
            {
                var result = _engine.GetAppointment(id);

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("appointment")]
        [HttpPost]
        public IHttpActionResult CreateAppointment(Appointment appointment)
        {
            try
            {
                var result = _engine.CreateAppointment(appointment);
                return Created(Request.RequestUri + "/" + result.Id.ToString(), result);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex); //o.O
            }
        }

        [Route("appointment/{id:int}/cancel")]
        [HttpPost]
        public IHttpActionResult CancelAppointment(int id, [FromBody] string disposition)
        {
            try
            {
                var result = _engine.CancelAppointment(id, disposition);

                if (result == null)
                    return NotFound();

                var response = new HttpResponseMessage(HttpStatusCode.Accepted);
                response.Headers.Location = new Uri(Request.RequestUri + "/" + result.Id.ToString());
                return ResponseMessage(response);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
