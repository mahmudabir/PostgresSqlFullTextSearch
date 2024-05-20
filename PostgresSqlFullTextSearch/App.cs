using System.Diagnostics;

using Microsoft.Extensions.Hosting;

using PostgresSqlFullTextSearch.Services;

namespace PostgresSqlFullTextSearch
{
    public class App: IHostedService
    {
        private readonly ProductRepository _productRepository;

        public App(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
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
            //sw = Stopwatch.StartNew();
            //long productCount = await _productRepository.GetProductCountAsync();
            //sw.Stop();

            //while (true)
            //{
            //    if (productCount <= 3_000_000)
            //    {
            //        sw = Stopwatch.StartNew();
            //        await _productRepository.SeedProductDataAsync();
            //        sw.Stop();
            //        Console.WriteLine($"Data seed complete");
            //        Console.WriteLine($"Elasped time: {Math.Round(sw.ElapsedMilliseconds / 1000.0, 2)}s");


            //        sw = Stopwatch.StartNew();
            //        productCount = await _productRepository.GetProductCountAsync();
            //        sw.Stop();
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}


            //Console.WriteLine($"Product count: {productCount}");
            //Console.WriteLine($"Elasped time (Count): {Math.Round(sw.ElapsedMilliseconds / 1000.0, 2)}s");

            string searchTerm = "bod";

            // Full-text search
            sw = Stopwatch.StartNew();
            var searchResults2 = await _productRepository.FullTextSearchProductsAsync(searchTerm);
            sw.Stop();
            Console.WriteLine($"Elasped time (Full-text Search): {Math.Round(sw.ElapsedMilliseconds / 1000.0, 2)}s");

            ////Regular search
            //sw = Stopwatch.StartNew();
            //var searchResults1 = await _productRepository.RegularSearchProductsAsync(searchTerm);
            //sw.Stop();
            //Console.WriteLine($"Elasped time (Regular Search): {Math.Round(sw.ElapsedMilliseconds / 1000.0, 2)}s");




            //for (int i = 0; i < 100; i++)
            //{
            //    // Full-text search
            //    sw = Stopwatch.StartNew();
            //    var searchResults3s = await _productRepository.FullTextSearchProductsAsync(searchTerm);
            //    sw.Stop();
            //    Console.WriteLine($"Elasped time (Full-text Search): {sw.ElapsedMilliseconds}ms");
            //}

            await StopAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }


}