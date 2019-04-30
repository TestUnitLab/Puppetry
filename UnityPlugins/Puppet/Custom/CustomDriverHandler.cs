namespace Puppetry.Puppet
 {
     public static class CustomDriverHandler
     {
         public static string ProcessGameCustomMethod(string key, string value)
         {
             string result;
             switch (key)
             {
                 default:
                     result = "There are no handler for method: " + key;
                     break;
             }

             return result;
         }

        public static string ProcessGameObjectCustomMethod(string upath, string key, string value)
        {
            string result;
            switch (key)
            {
                default:
                    result = "There are no handler for method: " + key;
                    break;
            }

            return result;
        }
    }
 }