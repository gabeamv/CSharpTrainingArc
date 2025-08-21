using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SecureNotes.Services
{
    public class HttpService
    {
        public static readonly HttpClient client = new HttpClient();
        public HttpService()
        {
        }

    }
}
