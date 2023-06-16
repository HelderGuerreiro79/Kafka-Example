using Confluent.Kafka;
 
var config = new ProducerConfig
{
    BootstrapServers = "localhost:9092"//,
    //SslCaLocation = "/Path-to/cluster-ca-certificate.pem",
    //SecurityProtocol = SecurityProtocol.SaslSsl,
    //SaslMechanism = SaslMechanism.ScramSha256,
    //SaslUsername = "ickafka",
    //SaslPassword = "yourpassword",

};

using (var p = new ProducerBuilder<Null, string>(config).Build())
{
    try
    {
        do
        {
            string value = await Task.Run(() =>
            {
                Console.WriteLine("Enter text (or quit to exit)");
                Console.Write("> ");
                return Console.ReadLine();
            });

            if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
                break;

            var dr = await p.ProduceAsync("test-topic", new Message<Null, string> { Value = value });
            Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
        } while (true);
    }
    catch (ProduceException<Null, string> e)
    {
        Console.WriteLine($"Delivery failed: {e.Error.Reason}");
    }
}