// See https://aka.ms/new-console-template for more information

using Oak.Flow;

var flow = Flow.FromJson(File.ReadAllText("test.json"));
Console.WriteLine(flow.Actions.First().Key);
Console.WriteLine(flow.ToJson(true));
