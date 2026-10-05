using Amazon;
using Amazon.SQS;
using SQSexample.Models;
using System.Text.Json;
using Amazon.SQS.Model;
using SQSexample;
using System.Linq.Expressions;


//var order = new PizzaOrder
//{
//    OrderId = Guid.NewGuid().ToString(),
//    CustomerName = "John Doe",
//    PizzaType = "Margherita",
//    OrderDate = DateTime.UtcNow
//};

//var json = JsonSerializer.Serialize(order);

//for (int i = 0; i < 10; i++)
//{
//    await sqs.SendMessageAsync(queueUrl, json);
//}


//ReceiveMessageResponse response;
//do
//{

//    response = await sqs.ReceiveMessageAsync(new ReceiveMessageRequest()
//    {
//        QueueUrl = queueUrl,
//        MaxNumberOfMessages = 10,
//        WaitTimeSeconds = 20
//    });

//    if (response.Messages == null)
//    {
//        Console.WriteLine("No messages received.");
//        break;
//    }

//    foreach (var message in response.Messages)
//    {
//        Console.WriteLine($"Received message: {message.Body}");
//        await sqs.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
//    }

//}
//while (response.Messages.Count != 0);


var sqs = new AmazonSQSClient("AKIAUFJLN4BI7TUCKHLR", "6JPNA65YGlopaSJHTADTLnEN5DgKwy3dJWqRySx5", RegionEndpoint.EUCentral1);
var queueUrl = "https://sqs.eu-central-1.amazonaws.com/286243283025/SQSqueueExample";

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1 - Create request");
    Console.WriteLine("2 - Process all requests");
    Console.WriteLine("3 - Show number of requests");
    Console.WriteLine("0 - Exit");
    Console.Write("Choose: ");

    string choice = Console.ReadLine();

    try
    {
        if (choice == "1")
        {
            await CreateRequest();
        }
        else if (choice == "2")
        {
            await ProcessRequests();
        }
        else if (choice == "3")
        {
            await ShowRequestCount();
        }
        else if (choice == "0")
        {
            break;
        }
        else
        {
            Console.WriteLine("Invalid choice");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

async Task CreateRequest()
{
    try
    {
        Console.Write("enter user name: ");
        string userName = Console.ReadLine();
        Console.Write("enter topic: ");
        string topic = Console.ReadLine();
        Console.Write("enter description: ");
        string description = Console.ReadLine();
        Console.Write("Enter priority (Low, Medium, High): ");
        string priority = Console.ReadLine();

        SupportRequest supportRequest = new SupportRequest
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Topic = topic,
            Description = description,
            Priority = priority,
            CreatedAt = DateTime.UtcNow
        };

        string json = JsonSerializer.Serialize(supportRequest);
        var msg = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = json
        };

        await sqs.SendMessageAsync(msg);

        Console.WriteLine($"Request sent with ID {supportRequest.Id}: {supportRequest.UserName} - {supportRequest.Topic}");
        Console.WriteLine("Request added to queue.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating request: {ex.Message}");
    }

}

async Task ProcessRequests()
{
    try
    {
        Console.WriteLine();
        Console.WriteLine("Receiving messages...");

        while (true)
        {
            var response = await sqs.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 2
            });

            if (response.Messages == null || response.Messages.Count == 0)
            {
                Console.WriteLine("No messages received.");
                break;
            }

            foreach (var message in response.Messages)
            {
                try
                {
                    var request = JsonSerializer.Deserialize<SupportRequest>(message.Body);

                    Console.WriteLine("Received support request:");
                    Console.WriteLine($"Request ID: {request.Id}");
                    Console.WriteLine($"User: {request.UserName}");
                    Console.WriteLine($"Topic: {request.Topic}");
                    Console.WriteLine($"Description: {request.Description}");
                    Console.WriteLine($"Priority: {request.Priority}");
                    Console.WriteLine($"Created At: {request.CreatedAt}");
                    Console.WriteLine($"Waited: {DateTime.UtcNow - request.CreatedAt}");

                    if (request.Priority == "High")
                    {
                        Console.WriteLine("This is a high-priority request.");
                    }

                    await sqs.DeleteMessageAsync(
                        queueUrl,
                        message.ReceiptHandle);

                    Console.WriteLine("Request processed and deleted from queue.");
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error receiving messages: {ex.Message}");
    }

}

async Task ShowRequestCount()
{
    try
    {
        var response = await sqs.GetQueueAttributesAsync(new GetQueueAttributesRequest
        {
            QueueUrl = queueUrl,
            AttributeNames = new List<string>
        {
            "ApproximateNumberOfMessages"
        }
        });
        string count = response.Attributes["ApproximateNumberOfMessages"];
        Console.WriteLine($"Number of requests in queue: {count}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error getting request count: {ex.Message}");
    }

}