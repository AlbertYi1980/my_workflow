using System;
using System.Activities;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mirror.Workflows.CustomActivities.MirrorCrud
{
    public class MirrorDetail<TModel> : MirrorCrud<TModel>
    {
        public InArgument<string> Filter { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            var mirrorBase = MirrorBase.Get(context).TrimEnd('/');
            var uri = new Uri($"{mirrorBase}/model/{ModelKey}/detail");
            var httpClient = new HttpClient();
            var body = WrapArgs(Filter.Get(context));
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

        private TModel UnwrapResult(string result)
        {
            var json = JsonConvert.SerializeObject(JObject.Parse(result).Property("model").Value);
            return JsonConvert.DeserializeObject<TModel>(json);
        }

        private string WrapArgs(string filter)
        {
            var warpper = new
            {
                filter = JToken.Parse(filter)

            };
            return JsonConvert.SerializeObject(warpper);
        }
    }
}