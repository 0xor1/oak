// See https://aka.ms/new-console-template for more information

using Common;
using Oak.Flow;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

var flow = JsonConvert.DeserializeObject<Flow>(
    File.ReadAllText("test.json"),
    new JsonSerializerSettings()
    {
        Converters = new List<JsonConverter>() { new ControlConverter(), new ActionConverter(), }
    }
);
DefaultContractResolver contractResolver = new DefaultContractResolver
{
    NamingStrategy = new CamelCaseNamingStrategy()
};
var str = JsonConvert.SerializeObject(
    flow,
    Formatting.Indented,
    new JsonSerializerSettings()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = contractResolver
    }
);
Console.WriteLine(flow.Actions.First().Key);
Console.WriteLine(str);
