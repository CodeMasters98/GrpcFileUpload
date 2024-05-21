
using Google.Protobuf;
using Grpc.Net.Client;
using GrpcService.Protos;
using System.Reflection;

Console.WriteLine("press a key to start");
Console.ReadKey();
Console.Clear();

var channel = GrpcChannel.ForAddress("https://localhost:7002");
var client = new FileStreamingService.FileStreamingServiceClient(channel);

var contentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

string file = Path.Combine(contentRootPath, "Files", "sample.mp4");
var fileInfo = new FileInfo(file);

decimal chunkSize = 0;
var buffer = new byte[1024 * 2];
using var call = client.UploadFile();
using var fileStream = new FileStream(file, FileMode.Open);
var content = new ByteContent
{
    FileSize = fileStream.Length,
    ReadedByte = 0,
    Info = new Info { FileName = Path.GetFileNameWithoutExtension(fileInfo.Name), FileExtension = fileInfo.Extension }
};
while ((content.ReadedByte = fileStream.Read(buffer, 0, buffer.Length)) > 0)
{
    content.Buffer=ByteString.CopyFrom(buffer);
    await call.RequestStream.WriteAsync(content);
    chunkSize += buffer.Length;
    Console.WriteLine($"{Math.Round(chunkSize * 100 / fileStream.Length)} %");
    await Task.Delay(100);
}
await call.RequestStream.CompleteAsync();
Console.ReadKey();
