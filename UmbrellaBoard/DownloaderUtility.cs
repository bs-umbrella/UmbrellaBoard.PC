using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace UmbrellaBoard
{
    internal struct Response
    {
        internal int httpCode;
        internal object content;
    }

    internal class DownloaderUtility
    {
        internal Response GetData(string url, Dictionary<string, string> headers)
        {
            Response response = new();

            if (url.StartsWith("file://"))
            {
                string path = url.Substring(7);
                if (File.Exists(path))
                {
                    response.httpCode = 200;
                    response.content = File.ReadAllText(path);
                }
                else
                {
                    response.httpCode = 404;
                    response.content = null;
                }
                return response;
            }

            UnityWebRequest uwr = new UnityWebRequest(new Uri(url), "GET");
            foreach (var item in headers)
                uwr.SetRequestHeader(item.Key, item.Value);

            uwr.SendWebRequest();

            while (!uwr.isDone)
                Thread.Sleep(10);
            if (uwr.result != UnityWebRequest.Result.ProtocolError && uwr.result != UnityWebRequest.Result.ConnectionError)
            {
                response.httpCode = 200;
                response.content = uwr.downloadHandler.text;
            }
            else
            {
                response.httpCode = 404;
                response.content = null;
            }

            return response;
        }

        internal Response GetJson(string url, Dictionary<string, string> headers)
        {
            Response data = GetData(url, headers);
            Response output = new()
            {
                content = null,
                httpCode = data.httpCode
            };


            if (data.content == null)
                return output;

            if (data.content is string)
                output.content = JObject.Parse((string) data.content);

            return output;
        }

        internal Response GetString(string url, Dictionary<string, string> headers = null)
        {
            Response data = GetData(url, headers);
            Response output = new()
            {
                content = null,
                httpCode = data.httpCode
            };

            if (data.content == null)
                return output;

            output.content = (string) data.content;
            return output;
        }
    }
}
