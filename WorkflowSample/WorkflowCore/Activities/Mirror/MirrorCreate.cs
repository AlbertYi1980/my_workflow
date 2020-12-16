using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Activities;
using System.Net.Http;
using System.Text;


namespace WorkflowCore.Activities
{
    public sealed class MirrorCreate : MirrorCrud<string>
    {
        public InArgument<string> Model { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            var mirrorBase = MirrorBase.Get(context).TrimEnd('/');
            var uri = new Uri($"{mirrorBase}/model/{ModelKey.Get(context)}/create");
            var httpClient = new HttpClient();
            var body = WrapArgs(Model.Get(context));
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                Headers = { { "TenantId", TenantId.Get(context) } },
                RequestUri = uri,
                Method = HttpMethod.Post,
                Content = content
            };
            var response = httpClient.SendAsync(request).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            Result.Set(context, UnwrapResult(json));
        }

        private  string UnwrapResult(string result)
        {
            return JsonConvert.SerializeObject(JObject.Parse(result).Property("model").Value);
        }

        private  string WrapArgs(string body)
        {
            var warpper = new
            {
                model =   JToken.Parse(body)
                
            };
            return JsonConvert.SerializeObject(warpper);
        }
    }
}