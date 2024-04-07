using System;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CloudyWing.MoneyTrack.Infrastructure.ActionResults {
    /// <summary>
    /// 使用 Json.Net 序列化回傳的 ActionResult，程式碼來源為 Json.Net 官網
    /// </summary>
    public class JsonNetResult : JsonResult {
        public Formatting Formatting { get; set; }

        public override void ExecuteResult(ControllerContext context) {
            if (context is null) {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType)
                ? ContentType
                : "application/json";

            if (ContentEncoding != null) {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null) {
                JsonTextWriter writer = new JsonTextWriter(response.Output) { Formatting = Formatting };

                JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
                serializer.Serialize(writer, Data);

                writer.Flush();
            }
        }
    }
}
