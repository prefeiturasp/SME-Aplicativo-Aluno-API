using Newtonsoft.Json;

namespace SME.AE.Comum
{
    public static class UtilJson
    {
        public static JsonSerializerSettings ObterConfigConverterNulosEmVazio()
        {
            return new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() };
        }
    }
}
