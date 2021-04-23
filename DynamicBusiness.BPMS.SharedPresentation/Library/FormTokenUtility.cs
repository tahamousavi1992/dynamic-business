using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    public static class FormTokenUtility
    {
        public static string FormToken = "formToken";
        public static string GetFormToken(string sessionId, Guid mainDynamicFormID, bool isEncrypted)
        {
            return StringCipher.Encrypt(mainDynamicFormID.ToString() + "_" + (isEncrypted ? "1" : "0"), sessionId);
        }

        /// <summary>
        /// It validates whether formToken is valid for this sessionId or not.
        /// It must change in future to validate formId alongside checking sessionId.
        /// </summary>
        public static bool ValidateFormToken(string formToken, string sessionId)
        {
            return !string.IsNullOrWhiteSpace(StringCipher.Decrypt(formToken, sessionId));
        }

        public static bool GetIsEncrypted(string formToken, string sessionId)
        {
            return StringCipher.Decrypt(formToken, sessionId).Split('_').LastOrDefault() == "1";
        }
    }
}