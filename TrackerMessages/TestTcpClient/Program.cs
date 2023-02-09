// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Net.Sockets;

var server = "localhost";
var port = 6000;
var message = "01 20 00 01 9A 02 18 03 38 36 31 32 33 30 30 34 33 39 30 37 36 32 36 04 32 00 FE 06 00 01 00 00 00 00 00 8F 29";

try
{
    using var client = new TcpClient(server, port);

    var data = message.Split(" ")
        .Select(x => byte.Parse(x, NumberStyles.HexNumber))
        .ToArray();
    
    var stream = client.GetStream();
    stream.Write(data, 0, data.Length);

    Console.WriteLine("Sent: {0}", message);

    data = new byte[256];

    var reader = new StreamReader(stream);

    while (!reader.EndOfStream)
    {
        var resp = reader.ReadLine();
    }

    var responseData = data.Select(x => x.ToString("X"));
    
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



