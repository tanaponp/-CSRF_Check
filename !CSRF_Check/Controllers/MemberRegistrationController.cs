using _CSRF_Check.Attributes;
using _CSRF_Library.Common;
using _CSRF_Library.Data;
using _CSRF_Library.Manager;
using _CSRF_Library.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebGrease;

namespace _CSRF_Check.Controllers
{
    [CheckSecretKeyActionFilter]
    [RoutePrefix("api/member")]
    public class MemberRegistrationController : ApiController
    {
        [HttpGet]
        [Route("hello")]
        public IHttpActionResult Hello()
        {
            try
            {
                return Ok(new CallResult
                {
                    Status = 1,
                    Message = "SUCCESS",
                    StackTrace = ""
                });
            }
            catch (Exception ex)
            {
                return Ok(new CallResult
                {
                    Status = -1,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }
        [HttpPost]
        [Route("UploadProfileImage")]
        public async Task<IHttpActionResult> UploadProfileImage()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var request = HttpContext.Current.Request;
            try
            {                
                MemberProfileImageData profileImageData = new MemberProfileImageData
                {
                    FileName = request.Form["Name"],
                    MemberLoginName = request.Form["MemberLoginName"],
                    CreatedByLoginName = "System",
                    CompanyID = request.Form["CompanyID"]
                };

                var provider = await Request.Content.ReadAsMultipartAsync();
                var stream = provider.Contents[0];

                var fileBytes = await stream.ReadAsByteArrayAsync();
                var fileName = stream.Headers.ContentDisposition.FileName.Replace("\"", "");
                
                profileImageData.MemberPicture = fileBytes;
                MemberRegistrationManager manager = new MemberRegistrationManager();
                var pi = manager.CreateMemberProfileImage(profileImageData);
                profileImageData.MemberPicture = null;
                profileImageData.MemberProfileImageGUID = pi.ForceToGUID();
                return Ok(profileImageData);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
        [HttpGet]
        [Route("GetProfileImageFromUpload/{MemberProfileImageGUID}")]
        public IHttpActionResult GetProfileImageFromUpload(string MemberProfileImageGUID)
        {
            try
            {
                MemberRegistrationManager manager = new MemberRegistrationManager();
                MemoryStream ms = manager.GetProfileImageFromUpload(MemberProfileImageGUID);
                ms.Position = 0;
                var responseMessageResult = ResponseMessage(new HttpResponseMessage(HttpStatusCode.OK));
                if (ms.Length > 0)
                {
                    var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                    httpResponseMessage.Content = new StreamContent(ms);

                    var ContentType = "application/octet-stream";
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
                    httpResponseMessage.Content.Headers.ContentLength = ms.Length;
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
                    {
                        FileName = MemberProfileImageGUID + ".png"
                    };

                    return responseMessageResult = ResponseMessage(httpResponseMessage);
                }
                else
                {
                    var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    httpResponseMessage.Content = new StringContent("Failed to get Profile Image from upload.", System.Text.Encoding.UTF8, "text/plain");

                    return responseMessageResult = ResponseMessage(httpResponseMessage);
                }
            }
            catch (Exception ex)
            {
                var responseMessageResult = ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                httpResponseMessage.Content = new StringContent("Message: " + ex.Message, System.Text.Encoding.UTF8, "text/plain");

                return responseMessageResult = ResponseMessage(httpResponseMessage);
            }
        }
    }
}
