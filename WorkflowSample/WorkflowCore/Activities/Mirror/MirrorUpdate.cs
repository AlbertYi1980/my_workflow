using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Activities;
using System.Net.Http;
using System.Text;

namespace WorkflowCore.Activities
{
    public sealed class MirrorUpdate : MirrorCrud<bool>
    {
        public InArgument<string> Filter { get; set; }
        public InArgument<string> Model { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            var mirrorBase = MirrorBase.Get(context).TrimEnd('/');
            var uri = new Uri($"{mirrorBase}/model/{ModelKey.Get(context)}/update");
            var httpClient = new HttpClient();
            var body = WrapArgs(Filter.Get(context), Model.Get(context));
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
            var updated = JObject.Parse(json)["before"].ToObject<JObject>() != null;
            Result.Set(context, updated);
        }

   

        private string WrapArgs(string filter, string model)
        {
            var update = new JObject();
            update["@set"] = JObject.Parse(model);
            var wrapper = new
            {
                filter = JObject.Parse(filter),
                update = update
            };
          
            return JsonConvert.SerializeObject(wrapper);
        }
    }
}