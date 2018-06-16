
namespace Puppet.Api
{
    internal class UnrealApiHandler : IApiHandler
    {
        private string _baseUrl;
        private string _port;

        public UnrealApiHandler(string baseUrl, string port)
        {
            _baseUrl = baseUrl;
            _port = port;
        }
    }
}
