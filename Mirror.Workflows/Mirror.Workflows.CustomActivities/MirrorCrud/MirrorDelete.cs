using System;
using System.Activities;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mirror.Workflows.CustomActivities.MirrorCrud
{
    public sealed class MirrorDelete : MirrorCrud<bool>
    {
        public InArgument<string> Filter { get; set; }
   

        protected override void Execute(NativeActivityContext context)
        {
            
            var mirrorBase = MirrorBase.Get(context).TrimEnd('/');
            var uri = new Uri($"{mirrorBase}/model/{ModelKey}/delete");
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
            var deleted = JObject.Parse(json)["model"].ToObject<JObject>() != null;
            Result.Set(context, deleted);
        }

      

        private string WrapArgs(string filter)
        {
            var wrapper = new JObject();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                wrapper["filter"] = JObject.Parse(filter);
            }
           

            return JsonConvert.SerializeObject(wrapper);
        }
    }
}