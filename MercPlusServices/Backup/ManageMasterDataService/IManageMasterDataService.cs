using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ManageMasterDataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IManageMasterData
    {
        [OperationContract]
        List<PayAgent> GetPayAgent();

        [OperationContract]
        PayAgent UpdatePayAgent(PayAgent PayAgentToBeUpdated);

        [OperationContract]
        bool DeletePayAgent(string RRISPayAgentCode);

        [OperationContract]
        bool CreatePayAgent(PayAgent PayAgentListToBeUpdated);

        // TODO: Add your service operations here
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations
    [DataContract]
    public class PayAgent
    {
        [DataMember]
        public string PayAgentCode { get; set; }

        [DataMember]
        public string CorpPayAgentCode { get; set; }

        [DataMember]
        public string RRISFormat { get; set; }

        [DataMember]
        public string ProfitCenter { get; set; }

        [DataMember]
        public string ChangeUser { get; set; }

        [DataMember]
        public DateTime ChangeTime { get; set; }
    }
}
