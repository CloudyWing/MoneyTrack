using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CloudyWing.MoneyTrack.Infrastructure.Util {
    public static class JsonUtils {
        public static string Serialize<T>(T value, SerializerOptions options = SerializerOptions.None) {
            Formatting formatting = options.HasFlag(SerializerOptions.Formatted) ? Formatting.Indented : Formatting.None;
            JsonSerializerSettings settings = new JsonSerializerSettings {
                ContractResolver = new SerializerResolver(options),
                DateFormatString = "yyyy/MM/dd HH:mm:ss"
            };

            return JsonConvert.SerializeObject(value, formatting, settings);
        }

        public static T Deserialize<T>(string value) {
            return JsonConvert.DeserializeObject<T>(value);
        }

        private class SerializerResolver : DefaultContractResolver {
            private readonly SerializerOptions serializerOptions;

            public SerializerResolver(SerializerOptions serializerOptions) {
                this.serializerOptions = serializerOptions;

                if (this.serializerOptions.HasFlag(SerializerOptions.CamelCase)) {
                    NamingStrategy = new CamelCaseNamingStrategy {
                        ProcessDictionaryKeys = true,
                        OverrideSpecifiedNames = true
                    };
                }
            }

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
                JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);
                jsonProperty.ValueProvider = new SerializerValueProvider(serializerOptions, member, jsonProperty.ValueProvider);

                return jsonProperty;
            }
        }

        private sealed class SerializerValueProvider : IValueProvider {
            private readonly SerializerOptions serializerOptions;
            private readonly MemberInfo memberInfo;
            private readonly IValueProvider originalValueProvider;

            public SerializerValueProvider(SerializerOptions serializerOptions, MemberInfo memberInfo, IValueProvider originalValueProvider) {
                this.serializerOptions = serializerOptions;
                this.memberInfo = memberInfo ?? throw new ArgumentNullException(nameof(memberInfo));
                this.originalValueProvider = originalValueProvider ?? throw new ArgumentNullException(nameof(originalValueProvider));
            }

            public void SetValue(object target, object value) {
                throw new NotImplementedException();
            }

            public object GetValue(object target) {
                if (serializerOptions.HasFlag(SerializerOptions.ConvertNullToEmpty)
                    && memberInfo is PropertyInfo propertyInfo
                    && propertyInfo.PropertyType == typeof(string)) {
                    return propertyInfo.GetValue(target) ?? "";
                }

                return originalValueProvider.GetValue(target);
            }
        }
    }
}
