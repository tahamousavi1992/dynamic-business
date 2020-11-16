using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class ThreadGatewayStatusXmlModel
    {
        public ThreadGatewayStatusXmlModel()
        {
            this.List = new List<GatewayJoinSequencrXmlModel>();
        }
        /// <summary>
        /// is gateway ID which is Guid
        /// </summary>
        public Guid GatewayID { get; set; }
        public List<GatewayJoinSequencrXmlModel> List { get; set; }
        public void Update(Guid gatewayID, List<GatewayJoinSequencrXmlModel> list)
        {
            this.GatewayID = gatewayID;
            this.List = list;
        }

    }
    public class GatewayJoinSequencrXmlModel
    {
        /// <summary>
        /// is SequenceFlow ID attribute which is Guid
        /// </summary>
        public Guid SequenceFlowID { get; set; }
        public bool Done { get; set; }
    }
}
