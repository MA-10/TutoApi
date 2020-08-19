﻿namespace Test.Response
{
    public class Response<T>
    {
        public T Data { get; set; }

        public Response(T response)
        {
            Data = response;
        }
    }
}