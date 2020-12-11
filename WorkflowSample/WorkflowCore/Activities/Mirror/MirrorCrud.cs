using System;
using System.Activities;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace WorkflowCore.Activities
{
    public abstract class MirrorCrud : NativeActivity
    {
        public InArgument<string> MirrorBase { get; set; }
        public InArgument<string>   TenantId { get; set; }
        public InArgument<string> ModelKey { get; set; }
        public InArgument<JsonElement> Args { get; set; }
        public OutArgument<JsonElement> Result { get; set; }

        protected sealed override void Execute(NativeActivityContext context)
        {
            var mirrorBase = MirrorBase.Get(context).TrimEnd('/');
            var uri = new Uri($"{mirrorBase}/model/{ModelKey.Get(context)}/{CrudType}");
            var httpClient = new HttpClient();
            var body = Args.Get(context);
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage
            {
                Headers = { { "TenantId", TenantId.Get(context) } },
                RequestUri = uri,
                Method = HttpMethod.Post,
                Content = content
            };
            var response = httpClient.SendAsync(request).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            Result.Set(context, JsonSerializer.Deserialize<JsonElement>(json));
        }

        protected abstract string  CrudType { get; }
    }
}