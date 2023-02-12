// See https://aka.ms/new-console-template for more information

using Dnsk.Common;
using Dnsk.JFlow;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

var b = JsonConvert.DeserializeObject<Form>(File.ReadAllText("test.json"), new JsonSerializerSettings()
{
    Converters =
    {
        ControlConverter.Singleton,
        ControlTypeConverter.Singleton,
        IfConverter.Singleton
    }
});
var str = JsonConvert.SerializeObject(b);
DefaultContractResolver contractResolver = new DefaultContractResolver
{
    NamingStrategy = new CamelCaseNamingStrategy()
};
Console.WriteLine(b.Controls.First().Key);
Console.WriteLine(JsonConvert.SerializeObject(b, new JsonSerializerSettings()
{
    ContractResolver = contractResolver
}));

var l = JsonConvert.DeserializeObject<IReadOnlyList<Key>>(@"['a', 's']");
Console.WriteLine(l.ToString());