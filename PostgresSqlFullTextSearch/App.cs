using System.Diagnostics;

using Microsoft.Extensions.Hosting;

using PostgresSqlFullTextSearch.Services;

namespace PostgresSqlFullTextSearch
{
    public class App : IHostedService
    {
        private readonly ProductRepository _productRepository;
        private readonly bool _toSecond = true;

        public App(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {            
            Stopwatch sw;

            #region CRUD Operations
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

            #endregion CRUD Operations

            #region Seed Data
            sw = Stopwatch.StartNew();
            long productCount = await _productRepository.GetProductCountAsync();
            sw.Stop();

            while (true)
            {
                if (productCount <= 1_000_000)
                {
                    sw = Stopwatch.StartNew();
                    await _productRepository.SeedProductDataAsync();
                    sw.Stop();
                    Console.WriteLine($"Data seed complete");
                    Console.WriteLine($"Elasped time: {ToTimeString(sw.ElapsedMilliseconds)}\n");


                    sw = Stopwatch.StartNew();
                    productCount = await _productRepository.GetProductCountAsync();
                    sw.Stop();
                }
                else
                {
                    break;
                }
            }

            //Console.WriteLine($"Product count: {productCount}");
            //Console.WriteLine($"Elasped time (Count): {ToTimeString(sw.ElapsedMilliseconds)}\n");
            #endregion Seed Data

            string searchTerm = "ody";

            // Full-text search
            sw = Stopwatch.StartNew();
            var fullTextSearchResult = await _productRepository.FullTextSearchProductsAsync(searchTerm);//, ProductRepository.OrMutation
            sw.Stop();
            Console.WriteLine($"Data Count (Full-text Search): {fullTextSearchResult.Count}");
            Console.WriteLine($"Elasped time (Full-text Search): {ToTimeString(sw.ElapsedMilliseconds)}\n");


            ////Regular search
            //sw = Stopwatch.StartNew();
            //var regularSearchResult = await _productRepository.RegularSearchProductsAsync(searchTerm);
            //sw.Stop();
            //Console.WriteLine($"Data Count (Regular Search): {regularSearchResult.Count}");
            //Console.WriteLine($"Elasped time (Regular Search): {ToTimeString(sw.ElapsedMilliseconds)}\n");


            //for (int i = 0; i < 10; i++)
            //{
            //    sw = Stopwatch.StartNew();
            //    fullTextSearchResult = await _productRepository.FullTextSearchProductsAsync(searchTerm, ProductRepository.OrMutation);
            //    sw.Stop();
            //    Console.WriteLine($"Data Count (Full-text Search): {fullTextSearchResult.Count}");
            //    Console.WriteLine($"Elasped time (Full-text Search): {ToTimeString(sw.ElapsedMilliseconds)}\n");
            //}


            await StopAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private string ToTimeString(long milliseconds)
        {
            return _toSecond ? Math.Round(milliseconds / 1000.0, 2).ToString() + "s" : milliseconds.ToString() + "ms";
        }
    }


}