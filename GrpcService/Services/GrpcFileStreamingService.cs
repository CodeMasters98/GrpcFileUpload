using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcService.Protos;

namespace GrpcService.Services
{
    public class GrpcFileStreamingService : FileStreamingService.FileStreamingServiceBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string uploadPath;
        public GrpcFileStreamingService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            uploadPath = Path.Combine(_webHostEnvironment.ContentRootPath, "files");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
        }
        public override async Task<Empty> UploadFile(IAsyncStreamReader<ByteContent> requestStream, ServerCallContext context)
        {
            FileStream fileStream = null;
            decimal chunkSize = 0;
            bool init = true;
            await foreach (var byteContent in requestStream.ReadAllAsync())
            {
                if (init)
                {
                    fileStream = new FileStream($"{uploadPath}/{byteContent.Info.FileName}{byteContent.Info.FileExtension}",FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.None);

                }
                var buffer = byteContent.Buffer.ToByteArray();
                await fileStream.WriteAsync(buffer,0, buffer.Length);
                chunkSize += byteContent.ReadedByte;
                Console.WriteLine($"{Math.Round(chunkSize*100/byteContent.FileSize)} %");
                init= false;
            }
            await fileStream.DisposeAsync();
            fileStream.Close();
            return new Empty();
        }
    }
}
