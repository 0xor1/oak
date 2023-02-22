// See https://aka.ms/new-console-template for more information

using Oak.Flow;

var flow = Flow.FromJson(File.ReadAllText("test.json"));
var str = flow.ToJson(true);
Console.WriteLine(flow.Actions.First().Key);
Console.WriteLine(str);
