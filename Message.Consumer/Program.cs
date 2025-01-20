using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "127.0.0.1", Port = 5672 };

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "special-queue", durable: false, exclusive: false, autoDelete: false,
    arguments: null);

Console.WriteLine("Aguardando novas mensagens...");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Recebido: {message}");
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync("special-queue", autoAck: true, consumer: consumer);

Console.WriteLine("Pressione 'enter' para sair.");
Console.ReadLine();