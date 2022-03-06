// See https://aka.ms/new-console-template for more information

using Exam.Calculator.ConsoleApp;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

while (true)
{
    await Calculate();
    Console.WriteLine();
}

static async Task Calculate()
{
    try
    {
        Console.Write("Input your first number:");
        var firstNumber = Console.ReadLine();

        Console.Write("Input your second number:");
        var secondNumber = Console.ReadLine();

        Console.Write("Input the operation (e.g. + - * /):");
        var operation = Console.ReadLine() ?? String.Empty;

        var config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
        var baseAddress = config["BaseAddress"];
        using var httpClient = new HttpClient();

        var request = new
        {
            FirstNumber = firstNumber,
            SecondNumber = secondNumber,
            OperationType = GetOperationType(operation)
        };
        string requestJson = JsonConvert.SerializeObject(request);
        var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(new Uri(baseAddress + "/api/Calculator"), content);
        response.EnsureSuccessStatusCode();

        var contentString = await response.Content.ReadAsStringAsync();
        var contentObject = JsonConvert.DeserializeObject<decimal>(contentString);
        Console.WriteLine($"Result is: {contentObject}");
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine("[400] Input was invalid. {0}", e.Message);
    }
    catch (InvalidOperationException)
    {
        Console.WriteLine("Please input valid operation type.");
    }
    catch (Exception)
    {
        Console.WriteLine("Please try again.");
    }
}

static OperationType GetOperationType(string operationType)
{
    return operationType switch
    {
        "+" => OperationType.Add,
        "-" => OperationType.Subtract,
        "*" => OperationType.Multiply,
        "/" => OperationType.Divide,
        _ => throw new InvalidOperationException("Invalid operation type."),
    };
}

