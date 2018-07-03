using System;

namespace GamePuppet
{
    [Serializable]
    public class PuppetDriverResponse
    {
        public string method;
        public string result;
        public string errormessage;
        public int statuscode;
        public string editortype;

        public void Clear()
        {
            method = null;
            result = null;
            errormessage = null;
            statuscode = 0;
            editortype = null;
        }
    }
}
