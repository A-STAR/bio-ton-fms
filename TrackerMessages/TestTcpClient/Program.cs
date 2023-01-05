// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;

Console.WriteLine("Hello, World!");


var server = "localhost";
var port = 6000;
var message = "simple message";

try
{
    using TcpClient client = new TcpClient(server, port);

    Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

    NetworkStream stream = client.GetStream();
    stream.Write(data, 0, data.Length);

    Console.WriteLine("Sent: {0}", message);

    data = new Byte[256];

    // String to store the response ASCII representation.
    String responseData = String.Empty;

    // Read the first batch of the TcpServer response bytes.
    Int32 bytes = stream.Read(data, 0, data.Length);
    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
    Console.WriteLine("Received: {0}", responseData);
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



