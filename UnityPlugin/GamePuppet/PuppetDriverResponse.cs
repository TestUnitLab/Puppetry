using System;

namespace GamePuppet
{
    [Serializable]
    public class PuppetDriverResponse
    {
        public string session;
        public string method;
        public string result;
        public string errormessage;
        public int statuscode;

        public void Clear()
        {
            session = null;
            method = null;
            result = null;
            errormessage = null;
            statuscode = 0;
        }
    }
}
