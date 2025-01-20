using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory { HostName = "127.0.0.1", Port = 5672 };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "special-queue", durable: false, exclusive: false, autoDelete: false,
    arguments: null);

Console.WriteLine("Digite a mensagem que será enviada ou 'sair' para encerrar o programa:");

var message = "";
while ((message = Console.ReadLine()) != "sair")
{

    var options = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    var enrichedMessage = JsonSerializer.Serialize(new Message(message), options);

    var body = Encoding.UTF8.GetBytes(enrichedMessage);

    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "special-queue", body: body);
    
    Console.WriteLine("Mensagem enviada!");

    Console.WriteLine("Você pode enviar uma nova mensagem ou digitar 'sair' para encerrar o programa:");

}

public class Message
{
    DateTime now = DateTime.Now;
    public Message(string texto)
    {
        Texto = texto;
        Horario = now.ToString("HH:mm:ss");
        Data = now.ToString("dd/MM/yyyy");
    }

    public string Texto { get; set; }
    public string Horario { get; set; }
    public string Data { get; set; }
}

