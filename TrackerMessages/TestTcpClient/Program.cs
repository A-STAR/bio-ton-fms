using System.Globalization;
using System.Net.Sockets;

var server = "localhost";
var port = 6000;
var message = "01 1A 80 01 82 02 17 03 38 36 32 30 35 37 30 34 37 36 31 35 36 30 31 04 2C 07 48 0B 00 C9 0A";

try
{
    using var client = new TcpClient(server, port);

    var data = message.Split(" ")
        .Select(x => byte.Parse(x, NumberStyles.HexNumber))
        .ToArray();
    
    var stream = client.GetStream();
    stream.Write(data, 0, data.Length);

    Console.WriteLine("Sent: {0}", message);

    Console.WriteLine("Received: {0}", "resp");
}
catch (ArgumentNullException e)
{
    Console.WriteLine("ArgumentNullException: {0}", e);
}
catch (SocketException e)
{
    Console.WriteLine("SocketException: {0}", e);
}

Console.WriteLine("\n Press Enter to continue...");
Console.Read();



