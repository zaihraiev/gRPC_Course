using Blog;
using Grpc.Core;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using Server_gRPC.Services;
using System;

namespace Server_gRPC
{
    internal class Program
    {
        const int PORT = 50050;

        static void Main(string[] args)
        {
            var reflectionServiceImpl = new ReflectionServiceImpl(BlogService.Descriptor, ServerReflection.Descriptor);

            Server server = null;

            try
            {
                server = new Server()
                {
                    Ports = { new ServerPort("localhost", PORT, ServerCredentials.Insecure) },
                    Services = { BlogService.BindService(new BlogServiceImpl()), 
                        ServerReflection.BindService(reflectionServiceImpl)}
                };

                server.Start();
                Console.WriteLine("The server is listening on the port: " + PORT);
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("The server failed to start: " + ex.Message);
                throw;
            }
            finally
            {
                if(server != null)
                {
                    server.ShutdownAsync().Wait();
                }
            }
        }
    }
}
