using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Activities;
using System.Net.Http;
using System.Text;

namespace WorkflowCore.Activities
{
    public sealed class MirrorQuery : MirrorCrud<string>
    {
        public InArgument<string> Filter { get; set; }
        public InArgument<string> Sort { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            var mirrorBase = MirrorBase.Get(context).TrimEnd('/');
            var uri = new Uri($"{mirrorBase}/model/{ModelKey.Get(context)}/query");
            var httpClient = new HttpClient();
            var body = WrapArgs(Filter.Get(context), Sort.Get(context));
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

        private string UnwrapResult(string result)
        {
            return JsonConvert.SerializeObject(JObject.Parse(result).Property("items").Value);
        }

        private string WrapArgs(string filter, string sort)
        {
            var wrapper = new JObject();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                wrapper["filter"] = JObject.Parse(filter);
            }
            if (!string.IsNullOrWhiteSpace(sort))
            {
                wrapper["sort"] = JObject.Parse(sort);
            }

            return JsonConvert.SerializeObject(wrapper);
        }
    }
}