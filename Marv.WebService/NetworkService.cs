using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using Marv.Common;

namespace Marv.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "NetworkService" in both code and config file together.
    [ServiceContract]
    public class NetworkService
    {
        private readonly IEnumerable<Network> networks = Read(Directory.GetCurrentDirectory());

        private static IEnumerable<Network> Read(string dirPath)
        {
            return Directory.GetFiles(dirPath, "*.net").Select(Network.Read);
        }

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "Beliefs/{fileName}?evidence={evidence}")]
        public Message GetBeliefs(string fileName, string evidence)
        {
            var network = this.networks.FirstOrDefault(net => net.FileName.Contains(fileName));

            var evidences = evidence.ParseJson<Dictionary<string, string>>();

            foreach (var kvp in evidences)
            {
                network.SetEvidence(kvp.Key, kvp.Value);
            }

            network.UpdateBeliefs();
            
            return WebOperationContext.Current.CreateTextResponse(network.GetBeliefs().ToJson(), "application/json; charset=utf-8", Encoding.UTF8);
        }

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "CurrentDirectory")]
        public string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "Networks")]
        public Message GetNetworks()
        {
            return WebOperationContext.Current.CreateTextResponse(this.networks.ToJson(), "application/json; charset=utf-8", Encoding.UTF8);
        }
    }
}