namespace Puppetry.Puppet
 {
     public static class CustomDriverHandler
     {
         public static string Process(string key, string value)
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