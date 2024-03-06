using Blog;
using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace Client_gRPC
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Channel channel = new Channel("localhost", 50050, ChannelCredentials.Insecure);

            await channel.ConnectAsync().ContinueWith((task) =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    Console.WriteLine("The client connected successfully");
                }
            });

            var client = new BlogService.BlogServiceClient(channel);

            //var newBlog = CreateBlog(client);
            //ReadBlog(client);
            //UpdateBlog(client, newBlog);
            //DeleteBlog(client);

            await ListBlog(client);

            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }

        private static Blog.Blog CreateBlog(BlogService.BlogServiceClient client)
        {
            var response = client.CreateBlog(new CreateBlogRequest()
            {
                Blog = new Blog.Blog
                {
                    AuthorId = "Dmytro",
                    Title = "New blog!", 
                    Content = "Hello world!"
                }
            });

            Console.WriteLine("The blog " + response.Blog.Id + " was created");

            return response.Blog;
        }

        private static void ReadBlog(BlogService.BlogServiceClient client)
        {
            try
            {
                var response = client.ReadBlog(new ReadBlogRequest()
                {
                    BlogId = "65e8708bd56d23f7d347fa3a"
                });

                Console.WriteLine(response.Blog.ToString());
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Status.Detail);
            }
        }

        private static void UpdateBlog(BlogService.BlogServiceClient client, Blog.Blog blog)
        {
            try
            {
                blog.AuthorId = "Updated author";
                blog.Title = "Updated title";
                blog.Content = "Updated content";

                var response = client.UpdateBlog(new UpdateBlogRequest()
                {
                    Blog = blog
                });

                Console.WriteLine(response.Blog.ToString());
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Status.Detail);
            }
        }

        private static void DeleteBlog(BlogService.BlogServiceClient client)
        {
            try
            {
                var response = client.DeleteBlog(new DeleteBlogRequest()
                {
                    BlogId = "65e8708bd56d23f7d347fa3a"
                });

                Console.WriteLine("Blog with id " + response.BlogId + "was successfully deleted.");
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Status.Detail);
            }
        }

        private static async Task ListBlog(BlogService.BlogServiceClient client)
        {
            var response = client.ListBlog(new ListBlogRequest() { });

            while(await response.ResponseStream.MoveNext())
            {
                Console.WriteLine(response.ResponseStream.Current.Blog.ToString());
            }
        }
    }
}
