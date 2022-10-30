using ElascticCore;
using RedisCore;
using System.Configuration;

using (RedisClient client = new RedisClient())
using (ElasticCoreService<ChatModel> elastic = new ElasticCoreService<ChatModel>())
{
    // Get chat history from elastic
    var chats = elastic.SearchLog(5);
    Console.WriteLine("TOP 5 Message History:");
    Console.WriteLine("".PadRight(60, '*'));
    foreach (var chat in chats)
    {
        Console.WriteLine($"-{chat.From}({chat.PostDate}): {chat.Message}");
    }
    Console.WriteLine("".PadRight(60, '*'));
    Console.WriteLine();
    //---------------------------

    var pubSub = client.GetSubscriber();
    bool isStay = true;
    while (isStay)
    {
        // subscriber
        await pubSub.SubscribeAsync(client.Channel2, (channel, message) =>
        {
            Console.WriteLine(Environment.NewLine + client.Channel2 + ": " + message);

            // publisher
            Console.Write("Write Message : ");
            var message2 = Console.ReadLine();
            isStay = message2.ToLower() != "exit" ? true : false;
            pubSub.PublishAsync(client.Channel, message2, StackExchange.Redis.CommandFlags.FireAndForget);
            ChatModel chatModel = new ChatModel { From = client.Channel2, To = client.Channel, Message = message2, PostDate = DateTime.Now };
            elastic.CheckExistsAndInsertLog(chatModel, ConfigurationManager.AppSettings["ElasticIndexName"]);
        });

        // publisher
        Console.WriteLine("Write Message: ");
        var message = Console.ReadLine();
        isStay = message.ToLower() != "exit" ? true : false;
        await pubSub.PublishAsync(client.Channel, message, StackExchange.Redis.CommandFlags.FireAndForget);
        ChatModel chatModel = new ChatModel { From = client.Channel2, To = client.Channel, Message = message, PostDate = DateTime.Now };
        elastic.CheckExistsAndInsertLog(chatModel, ConfigurationManager.AppSettings["ElasticIndexName"]);
    }
}
