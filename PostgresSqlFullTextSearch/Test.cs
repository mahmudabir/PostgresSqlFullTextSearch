using Microsoft.Extensions.DependencyInjection;

using PostgresSqlFullTextSearch.Services;

using System.Diagnostics;

namespace PostgresSqlFullTextSearch
{
    public class Test
    {
        private readonly IServiceProvider _serviceProvider;

        public Test(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task RunAsync()
        {
            ProductRepository repository = _serviceProvider.GetService<ProductRepository>()!;

            //// Create
            //var newProduct = new Product { Name = "New Product", Description = "This is the body of the new post." };
            //await repository.AddProductAsync(newProduct);
            //Console.WriteLine("Product created!");

            //// Read
            //var product = await repository.GetProductAsync(newProduct.Id);
            //Console.WriteLine($"Retrieved product: {product.Name}");

            //// Update
            //product.Description = product.Description + "Updated body of the product.";
            //await repository.UpdateProductAsync(product);
            //Console.WriteLine("Product updated!");

            //// Delete
            //await repository.DeleteProductAsync(product.Id);
            //Console.WriteLine("Product deleted!");

            Stopwatch sw;
            sw = Stopwatch.StartNew();
            long productCount = await repository.GetProductCountAsync();
            sw.Stop();
            if (productCount == 0)
            {
                sw = Stopwatch.StartNew();
                await repository.SeedProductDataAsync();
                sw.Stop();
                Console.WriteLine($"Data seed complete.s");
                Console.WriteLine($"Elasped time: {Math.Round(sw.ElapsedMilliseconds / 1000.0, 2)}s");


                sw = Stopwatch.StartNew();
                productCount = await repository.GetProductCountAsync();
                sw.Stop();
            }

            Console.WriteLine($"Product count: {productCount}");
            Console.WriteLine($"Elasped time (Count): {Math.Round(sw.ElapsedMilliseconds / 1000.0, 2)}s");

            string searchTerm = "abir";

            // Full-text search
            sw = Stopwatch.StartNew();
            var ssearchResults2 = await repository.FullTextSearchProductsAsync(searchTerm);
            sw.Stop();
            Console.WriteLine($"Elasped time (Full-text Search): {Math.Round(sw.ElapsedMilliseconds / 1000.0, 2)}s");

            //Regular search
            sw = Stopwatch.StartNew();
            var searchResults1 = await repository.RegularSearchProductsAsync(searchTerm);
            sw.Stop();
            Console.WriteLine($"Elasped time (Regular Search): {Math.Round(sw.ElapsedMilliseconds / 1000.0, 2)}s");




            //for (int i = 0; i < 100; i++)
            //{
            //    // Full-text search
            //    sw = Stopwatch.StartNew();
            //    var ssearchResults3s = await repository.FullTextSearchProductsAsync(searchTerm);
            //    sw.Stop();
            //    Console.WriteLine($"Elasped time (Full-text Search): {sw.ElapsedMilliseconds}ms");
            //}
        }
    }


}