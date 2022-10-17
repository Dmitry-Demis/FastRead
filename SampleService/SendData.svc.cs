using System.Collections.Generic;
using WordFrequency;

namespace SampleService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SendData" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SendData.svc or SendData.svc.cs at the Solution Explorer and start debugging.
    public class SendData : ISendData
    {
        public Dictionary<string, uint> ReceiveFile(string[] data) => Frequency.ParallelCheck(data);
    }
}
