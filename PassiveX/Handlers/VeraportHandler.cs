﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Flurl.Http;
using Newtonsoft.Json;
using Org.BouncyCastle.Cms;
using PassiveX.Transports;
using PassiveX.Utils;

namespace PassiveX.Handlers
{
    [Handler(hostname: "127.0.0.1", port: 16106)]
    internal class VeraportHandler : IHttpHandler
    {
        public async Task<HttpResponse> HandleRequest(HttpRequest request)
        {
            var response = new HttpResponse();

            if (request.Method == "GET")
            {
                var callback = request.Parameters["callback"];
                var datafield = request.Parameters["data"];
                dynamic rootData = JsonConvert.DeserializeObject(datafield);
                var data = rootData.data;

                dynamic result = null;
                switch ((string)rootData.cmd)
                {
                    case "getVersion":
                        result = new { res = 0, data = "9,9,9,9" };
                        break;
                    case "isRunning":
                        result = new { res = 0 };
                        break;
                    case "getAxInfo":
                        result = new { res = 0, data = await GetAxInfoData((string)data.configure.axinfourl) };
                        break;
                    default:
                        Log.W($"Unknown command: {rootData.cmd}");
                        break;
                }

                response.SetJsonCallback(callback, result);
            }

            return response;
        }

        private async Task<dynamic> GetAxInfoData(string url)
        {
            var resp = await url.GetStringAsync();
            var signedData = Convert.FromBase64String(resp);
            var parser = new CmsSignedDataParser(signedData);
            var stream = parser.GetSignedContent().ContentStream;
            var doc = XDocument.Load(stream);

            return doc.Root.Elements("object")
                .GroupBy(x => x.Element("objectName").Value)
                .Select(g => g.First())
                .Select(x => new {
                    allowrun = true,
                    allowrundomains = "",
                    backupurl = x.Element("backupURL").Value,
                    block = false,
                    browsertype = x.Element("browserType").Value,
                    browserversion = -1,
                    description = "",
                    displayname = x.Element("displayName").Value,
                    downloadurl = x.Element("downloadURL").Value,
                    forceinstall = false,
                    installstate = true,
                    killbit = false,
                    localversion = x.Element("objectVersion").Value,
                    objectclsid = x.Element("objectMIMEType")?.Value ?? "application/vnd.microsoft.portable-executable",
                    objectname = x.Element("objectName").Value,
                    objecttype = 0,
                    objectversion = x.Element("objectVersion").Value,
                    policydisable = false,
                    systemtype = x.Element("systemType").Value,
                    updatestate = false,
                    version = 0,
                });
        }
    }
}
