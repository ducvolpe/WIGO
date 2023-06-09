using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace WIGO.Core
{
    public static class NetService
    {
        static HttpClient _client = new HttpClient();

        const string URL = "http://v2.cerebrohq.com/testapi/rpc.php";

        public static async Task<string> TryRegisterNewAccount(string phoneNumber, CancellationToken token = default)
        {
            var uid = SystemInfo.deviceUniqueIdentifier;
            var language = Application.systemLanguage.ToString();
            string slang = language.Substring(0, 2).ToLower();
            RPCRequest request = new RPCRequest()
            {
                jsonrpc = "2.0",
                method = "register",
                @params = new List<string> { slang, uid, phoneNumber },
                id = "0"
            };
            string json = JsonReader.Serialize(request);
            var resJson = await PostRequest(json, token);

            if (string.IsNullOrEmpty(resJson))
            {
                Debug.LogError("Register request is empty");
                return string.Empty;
            }

            try
            {
                RPCResult<RegisterResult> res = JsonReader.Deserialize<RPCResult<RegisterResult>>(resJson);
                return res.result.token;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                return string.Empty;
            }
        }

        public static async Task<ConfirmRegisterResult> TryConfirmRegister(string token, string code, CancellationToken ctoken = default)
        {
            var uid = SystemInfo.deviceUniqueIdentifier;
            RPCRequest request = new RPCRequest()
            {
                jsonrpc = "2.0",
                method = "registerConfirm",
                @params = new List<string> { token, uid, code },
                id = "0"
            };

            string json = JsonReader.Serialize(request);
            var resJson = await PostRequest(json, ctoken);

            if (string.IsNullOrEmpty(resJson))
            {
                Debug.LogError("Confirm register request is empty");
                return new ConfirmRegisterResult();
            }

            try
            {
                RPCResult<ConfirmRegisterResult> res = JsonReader.Deserialize<RPCResult<ConfirmRegisterResult>>(resJson);
                return res.result;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                return new ConfirmRegisterResult();
            }
        }

        public static async Task<ProfileData> TryLogin(string ltoken, CancellationToken token = default)
        {
            var uid = SystemInfo.deviceUniqueIdentifier;
            RPCRequest request = new RPCRequest()
            {
                jsonrpc = "2.0",
                method = "start",
                @params = new List<string> { ltoken, uid },
                id = "0"
            };

            string json = JsonReader.Serialize(request);
            var resJson = await PostRequest(json, token);

            if (string.IsNullOrEmpty(resJson))
            {
                Debug.LogError("Log in request is empty");
                return null;
            }

            try
            {
                RPCResult<AuthorizeResult> res = JsonReader.Deserialize<RPCResult<AuthorizeResult>>(resJson);
                Debug.LogFormat("Short key: {0}", res.result.stoken);
                return res.result.profile;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                return null;
            }
        }

        public static async Task<ProfileData> TryUpdateUser(string uid, string stoken, string email, string username, CancellationToken token = default)
        {
            string userUpdJson = "{\"uid\":" + $"\"{uid}\"," +
                                 "\"email\":" + $"\"{email}\"," +
                                 "\"nickname\":" + $"\"{username}\"}}";
            RPCRequest request = new RPCRequest()
            {
                jsonrpc = "2.0",
                method = "userSet",
                @params = new List<string> { userUpdJson },
                id = "0"
            };

            string json = JsonReader.Serialize(request);
            var resJson = await PostRequest(json, token, stoken);

            if (string.IsNullOrEmpty(resJson))
            {
                Debug.LogError("Log in request is empty");
                return null;
            }

            Debug.Log(resJson);
            try
            {
                RPCResult<List<ProfileData>> res = JsonReader.Deserialize<RPCResult<List<ProfileData>>>(resJson);
                return res.result[0];
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                return null;
            }
        }

        static async Task<string> PostRequest(string request, CancellationToken token, string addHeader = null)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new System.Uri(URL),
                Headers = {
                    { HttpRequestHeader.ContentType.ToString(), "application/json" }//,
                    //{ "rpcauth", "adg463df" }
                },
                Content = new StringContent(request)
            };

            if (!string.IsNullOrEmpty(addHeader))
            {
                httpRequestMessage.Headers.Add("rpcauth", addHeader);
            }

            try
            {
                var response = await _client.SendAsync(httpRequestMessage, token);
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    return res;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            
            return string.Empty;
        }
    }
}
