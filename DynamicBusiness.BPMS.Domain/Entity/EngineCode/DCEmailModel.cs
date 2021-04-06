using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class DCEmailModel : DCBaseModel
    {
        public DCEmailModel() { }
        public DCEmailModel(string id, string name, string shapeid, string parentShapeId, bool? isOutputYes,
            bool isFirst, string funcName, Guid emailAccountID, string emailTo, string emailSubject, string emailBody)
            : base(id, name, e_ActionType.Email, parentShapeId, shapeid, isOutputYes, isFirst, funcName)
        {
            this.EmailAccountID = emailAccountID;
            this.EmailTo = emailTo;
            this.EmailSubject = emailSubject;
            this.EmailBody = emailBody;
        }
        [DataMember]
        [Required]
        public Guid EmailAccountID { get; set; }
        [DataMember]
        [Required]
        public string EmailTo { get; set; }
        [DataMember]
        public string EmailSubject { get; set; }
        [DataMember]
        public string EmailBody { get; set; }

        public override object FillData(XElement xElement)
        {
            base.FillData(xElement);
            this.EmailAccountID = xElement.GetValue(nameof(DCEmailModel.EmailAccountID)).ToGuidObj();
            this.EmailTo = xElement.GetValue(nameof(DCEmailModel.EmailTo));
            this.EmailSubject = HttpUtility.UrlDecode(xElement.GetValue(nameof(DCEmailModel.EmailSubject)));
            this.EmailBody = HttpUtility.UrlDecode(xElement.GetValue(nameof(DCEmailModel.EmailBody)));

            return this;
        }

        public override XElement ToXmlElement()
        {
            return new XElement(nameof(DCEmailModel),
                     base.ToXmlElementArray(),
                     new XElement(nameof(DCEmailModel.EmailAccountID), this.EmailAccountID),
                     new XElement(nameof(DCEmailModel.EmailTo), this.EmailTo.ToStringObj().Trim()),
                     new XElement(nameof(DCEmailModel.EmailSubject), HttpUtility.UrlEncode(this.EmailSubject)),
                     new XElement(nameof(DCEmailModel.EmailBody), HttpUtility.UrlEncode(this.EmailBody))
                     );
        }

        public override bool Execute(ICodeBase codeBase)
        {
            string subject = this.EmailSubject;
            string content = this.EmailBody;
            List<string> toEmailList = this.EmailTo.ToStringObj().Trim().Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
           
            //replace variable tokens with its values.
            for (int i = 0; i < toEmailList.Count; i++)
            {
                foreach (string item in DomainUtility.GetRegularValue("[", "]", EmailTo).Distinct())
                {
                    toEmailList[i] = toEmailList[i].Replace("[" + item + "]", codeBase.VariableHelper.GetValue(item.Trim()).ToStringObj());
                }
            }

            //replace variable tokens with its values.
            if (!string.IsNullOrWhiteSpace(subject))
            {
                foreach (string item in DomainUtility.GetRegularValue("[", "]", subject).Distinct())
                {
                    subject = subject.Replace("[" + item + "]", codeBase.VariableHelper.GetValue(item.Trim()).ToStringObj());
                }
            }

            //replace variable tokens with its values.
            if (!string.IsNullOrWhiteSpace(content))
            {
                foreach (string item in DomainUtility.GetRegularValue("[", "]", content).Distinct())
                {
                    content = content.Replace("[" + item + "]", codeBase.VariableHelper.GetValue(item.Trim()).ToStringObj());
                }
            }
            return codeBase.MessageHelper.SendEmail(this.EmailAccountID, toEmailList, "", "", subject, content);
        }

    }
}
