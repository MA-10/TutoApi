using System.Collections;
using System.Collections.Generic;

namespace Test.Response
{
    public class AuthFailedResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}