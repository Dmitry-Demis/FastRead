using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SampleService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISendData" in both code and config file together.
    [ServiceContract]
    public interface ISendData
    {
        /// <summary>
        /// A server method that gets partition data and send a partition dictionary
        /// </summary>
        /// <param name="data">A string array of data</param>
        /// <returns>Dictionary&lt;string, uint&gt;</returns>
        [OperationContract]
        Dictionary<string, uint> ReceiveFile(string[] data); 
    }
}
