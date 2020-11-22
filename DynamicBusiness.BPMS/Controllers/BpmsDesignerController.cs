using DotNetNuke.Web.Mvc.Framework.Controllers;
using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Xsl;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;

namespace DynamicBusiness.BPMS.Controllers
{
    public class BpmsDesignerController : BpmsAdminApiControlBase
    {
        [HttpGet]
        public object GetIndex(Guid ID)
        {
            using (ProcessService processService = new ProcessService())
            {
                sysBpmsProcess process = processService.GetInfo(ID);
                if (process != null)
                {
                    if (string.IsNullOrWhiteSpace(process.DiagramXML))
                        process.DiagramXML = "<definitions xmlns=\"http://www.omg.org/spec/BPMN/20100524/MODEL\" xmlns:bpmndi=\"http://www.omg.org/spec/BPMN/20100524/DI\" xmlns:omgdc=\"http://www.omg.org/spec/DD/20100524/DC\" xmlns:omgdi=\"http://www.omg.org/spec/DD/20100524/DI\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" targetNamespace=\"\" xsi:schemaLocation=\"http://www.omg.org/spec/BPMN/20100524/MODEL http://www.omg.org/spec/BPMN/2.0/20100501/BPMN20.xsd\"><collaboration id=\"sid-c0e745ff-361e-4afb-8c8d-2a1fc32b1424\"><participant id=\"sid-87F4C1D6-25E1-4A45-9DA7-AD945993D06F\" name=\"مشتری\" processRef=\"sid-C3803939-0872-457F-8336-EAE484DC4A04\" /></collaboration><process id=\"sid-C3803939-0872-457F-8336-EAE484DC4A04\" name=\"مشتری\" processType=\"None\" isClosed=\"false\" isExecutable=\"false\"></process><bpmndi:BPMNDiagram id=\"sid-74620812-92c4-44e5-949c-aa47393d3830\"></bpmndi:BPMNDiagram></definitions>";
                }
                return new ProcessDTO(process);
            }

        }

        [HttpGet]
        public object GetEditInfo(Guid ID)
        {
            using (ProcessService processService = new ProcessService())
            {
                ProcessDTO model = new ProcessDTO(processService.GetInfo(ID));
                model.ListTypes = EnumObjHelper.GetEnumList<sysBpmsProcess.e_TypeLU>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList();
                return model;
            }
        }

        [HttpPost]
        public object PostEditInfo(ProcessDTO processDTO)
        {
            using (ProcessService processService = new ProcessService())
            {
                sysBpmsProcess process = processService.GetInfo(processDTO.ID);
                process.Update(processDTO.Name, processDTO.Description, processDTO.ParallelCountPerUser, processDTO.TypeLU);

                ResultOperation resultOperation = processService.UpdateInfo(process);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpPost]
        public object PostEdit(ProcessDTO processDTO)
        {
            if (processDTO.DiagramXML != string.Empty)
            {
                using (ProcessService processService = new ProcessService())
                {
                    sysBpmsProcess currentProcess = processService.GetInfo(processDTO.ID);
                    if (currentProcess != null)
                    {

                        currentProcess.DiagramXML = processDTO.DiagramXML.Replace("\r\n", "").Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "");
                        this.UpdateWorkflowXML(currentProcess);

                        ResultOperation resultOperation = processService.Update(currentProcess);
                        if (!resultOperation.IsSuccess)
                            return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                    }
                }
            }
            return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
        }

        private void UpdateWorkflowXML(sysBpmsProcess _Process)
        {
            this.RemoveNameSpaces(_Process);
            XmlDocument xDocDiagram = this.StringToXml(_Process.WorkflowXML);
            _Process.WorkflowXML = this.GetXMLAsString(xDocDiagram.SelectSingleNode("//process"));
            this.ConvertWorkflowXMLwithXSL(_Process);
        }

        private XmlDocument StringToXml(string xmlString)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xmlString);
            return xdoc;
        }

        private string GetXMLAsString(XmlNode xml)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            xml.WriteTo(tx);
            return sw.ToString();
        }


        private void ConvertWorkflowXMLwithXSL(sysBpmsProcess _Process)
        {
            XmlDocument xdoc = this.StringToXml(_Process.WorkflowXML);

            //Load class pattern
            string XslFile = System.Web.Hosting.HostingEnvironment.MapPath(BPMSResources.Repository + "SplitNodeName.xslt");

            XslCompiledTransform xsl = new XslCompiledTransform();
            xsl.Load(XslFile);

            // get transformed results
            StringWriter sw = new StringWriter();

            xsl.Transform(xdoc, null, sw);

            _Process.WorkflowXML = sw.ToString();
            sw.Close();
        }

        private void RemoveNameSpaces(sysBpmsProcess _Process)
        {
            XmlDocument xdoc = this.StringToXml(_Process.DiagramXML);

            //Load class pattern
            string XslFile = System.Web.Hosting.HostingEnvironment.MapPath(BPMSResources.Repository + "RemoveNameSpaces.xslt");

            XslCompiledTransform xsl = new XslCompiledTransform();
            xsl.Load(XslFile);

            // get transformed results
            StringWriter sw = new StringWriter();

            xsl.Transform(xdoc, null, sw);

            _Process.WorkflowXML = sw.ToString();
            sw.Close();
        }

    }
}