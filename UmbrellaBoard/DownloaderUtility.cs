using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine.Networking;

namespace UmbrellaBoard
{
    internal struct Response<T>
    {
        internal int httpCode;
        internal T content;

        public bool Valid => httpCode >= 200 && httpCode < 300 && content != null;
    }

    internal class DownloaderUtility
    {
        internal Response<byte[]> GetData(string url, Dictionary<string, string> headers)
        {
            var response = new Response<byte[]>();

            if (url.StartsWith("file://"))
            {
                string path = url.Substring(7);
                if (File.Exists(path))
                {
                    response.httpCode = 200;
                    response.content = File.ReadAllBytes(path);
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
                response.content = uwr.downloadHandler.data;
            }
            else
            {
                response.httpCode = 404;
                response.content = null;
            }

            return response;
        }

        internal Response<JObject> GetJson(string url, Dictionary<string, string> headers)
        {
            var data = GetString(url, headers);
            Response<JObject> output = new()
            {
                content = null,
                httpCode = data.httpCode
            };


            if (data.content == null)
                return output;

            output.content = JObject.Parse(data.content);
            return output;
        }

        internal Response<string> GetString(string url, Dictionary<string, string> headers = null)
        {
            var data = GetData(url, headers);
            Response<string> output = new()
            {
                content = null,
                httpCode = data.httpCode
            };

            if (data.content == null)
                return output;

            output.content = System.Text.Encoding.Default.GetString(data.content);
            return output;
        }
    }
}
