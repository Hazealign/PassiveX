﻿using System;
using System.Threading.Tasks;

namespace PassiveX.Handler
{
    class NProtectHandler : IHandler
    {
        public Task<HttpResponse> HandleRequest(HttpRequest request)
        {
            var response = new HttpResponse();

            if (request.Method == "GET")
            {
                var code = request.Parameters["code"];
                var dummy = request.Parameters["dummy"];

                response.SetResource("1x1.gif");
            }

            return Task.FromResult(response);
        }
    }
}