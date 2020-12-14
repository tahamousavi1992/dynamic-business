using DotNetNuke.Web.Api;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace DynamicBusiness.BPMS.EngineApi.Controllers
{
    public class EngineCaptchaController : BpmsEngineApiControlBase
    {
        /// <param name="key">is controlId</param>
        [HttpGet]
        public void Get(string key, Guid? formId = null)
        {
            if (formId.HasValue)
            {
                sysBpmsDynamicForm dynamicForm = new DynamicFormService().GetInfo(formId.Value);
                WordCaptchaHtml control = (WordCaptchaHtml)dynamicForm?.FindControl(key);
                HttpContext.Current.Session[key] = this.RandomString(control.Length);
            }
            else
                HttpContext.Current.Session[key] = this.RandomString(8);

            string CaptchaImageText = HttpContext.Current.Session[key].ToString();

            // Create a CAPTCHA image using the text stored in the Session object.

            CaptchaImageDTO ci = new CaptchaImageDTO(CaptchaImageText, 200, 50, "Tahoma", 8);

            // Change the response headers to output a JPEG image.
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "image/jpeg";

            // Write the image to the response stream in JPEG format.
            MemoryStream oMemoryStream = new MemoryStream();
            ci.Image.Save(oMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] oBytes = oMemoryStream.GetBuffer();
            oMemoryStream.Close();

            // Dispose of the CAPTCHA image object.
            ci.Dispose();

            HttpContext.Current.Response.BinaryWrite(oBytes);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        private string RandomString(int length)
        {
            Random _Random = new Random();
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, length)
                .Select(x => pool[_Random.Next(0, pool.Length)]);
            return new string(chars.ToArray());
        }
    }
}