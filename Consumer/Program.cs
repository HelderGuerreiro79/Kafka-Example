using Confluent.Kafka;

var config = new ConsumerConfig
{
    GroupId = "test-consumer-group",
    BootstrapServers = "localhost:9092",
    //SslCaLocation = "/PathTO/cluster-ca-certificate.pem",
    //SecurityProtocol = SecurityProtocol.SaslSsl,
    //SaslMechanism = SaslMechanism.ScramSha256,
    //SaslUsername = "ickafka",
    //SaslPassword = "yourpassword",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

Console.WriteLine("Preparing connection");

using (var c = new ConsumerBuilder<Ignore, string>(config).Build())
{
    c.Subscribe("test-topic");

    CancellationTokenSource cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) =>
    {
        e.Cancel = true; // prevent the process from terminating.
        cts.Cancel();
    };

    Console.WriteLine("triggering reads.");

    try
    {
        while (true)
        {
            try
            {
                var cr = c.Consume(cts.Token);
                c.Commit();
                Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Error occurred: {e.Error.Reason}");

            }
        }
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("finnishing.");
        // Ensure the consumer leaves the group cleanly and final offsets are committed.
        c.Close();
    }
}