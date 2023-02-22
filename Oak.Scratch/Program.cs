// See https://aka.ms/new-console-template for more information

using Common;
using Oak.Flow;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

var flow = Flow.FromJson(File.ReadAllText("test.json"));
var str = flow.ToJson(true);
Console.WriteLine(flow.Actions.First().Key);
Console.WriteLine(str);
