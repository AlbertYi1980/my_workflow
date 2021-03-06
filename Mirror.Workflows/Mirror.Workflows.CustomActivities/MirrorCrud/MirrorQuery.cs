﻿using System;
using System.Activities;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mirror.Workflows.CustomActivities.MirrorCrud
{
    public sealed class MirrorQuery<TModel> : MirrorCrud<object>
    {
        public InArgument<string> Filter { get; set; }
        public InArgument<string> Sort { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
          
            
            var mirrorBase = MirrorBase.Get(context).TrimEnd('/');
            var uri = new Uri($"{mirrorBase}/model/{ModelKey}/query");
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

        private IEnumerable<TModel> UnwrapResult(string result)
        {
            var json = JsonConvert.SerializeObject(JObject.Parse(result).Property("items").Value);
            return JsonConvert.DeserializeObject<IEnumerable<TModel>>( json);
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