using System.Text.Json;

using Microsoft.EntityFrameworkCore;

using PostgresSqlFullTextSearch.DataAccess;
using PostgresSqlFullTextSearch.Models;

namespace PostgresSqlFullTextSearch.Services
{
    public class ProductRepository
    {
        private readonly AppDbContext _context;

        public static List<Product> Products = new();

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        // Create
        public async Task SeedProductDataAsync(int count = 5_000_000)
        {
            try
            {
                if (Products.Count == 0)
                {
                    string jsonFilePath = "Models\\product-seed-data.json";
                    Products = await ReadAndDeserializeJsonAsync<List<Product>>(jsonFilePath) ?? new();
                }

                while (Products.Count < count)
                {
                    Products.AddRange(Products);
                }

                Products.RemoveRange(count, Products.Count - count);

                // Add all at once
                //await _context.Products.AddRangeAsync(Products);
                //await _context.SaveChangesAsync();

                int chunkSize = 25_000;
                int partitionCount = Products.Count / chunkSize;
                // save data in chaunks of 25000
                for (int i = 0; i < partitionCount; i++)
                {
                    List<Product> productBatch = Products.Skip(chunkSize * i)
                        .Take(chunkSize)
                        .ToList();

                    await _context.Products.AddRangeAsync(productBatch);
                    await _context.SaveChangesAsync();
                }
                //var productcount = await _context.Products.LongCountAsync();

                Products = new();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task<T?> ReadAndDeserializeJsonAsync<T>(string filePath)
        {
            T? model;

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                model = await JsonSerializer.DeserializeAsync<T>(fs);
            }

            return model;
        }

        // Create
        public async Task AddProductAsync(Product post)
        {
            await _context.Products.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        // Read
        public async Task<Product?> GetProductAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<long> GetProductCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        // Update
        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        // Delete
        public async Task DeleteProductAsync(int id)
        {
            await _context.Products.Where(x => x.Id == id).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
        }

        // Regular search
        public async Task<List<Product>> RegularSearchProductsAsync(string query)
        {
            query = query.ToUpper();
            List<Product> products = await _context.Products
                .Where(p => p.Description.ToUpper().Contains(query))
                .ToListAsync();
            return products;
        }

        public async Task<List<Product>> FullTextSearchProductsAsync(string query, Func<string, string>? mutationFunction = null)
        {
            List<Product> products;

            if (mutationFunction != null)
            {
                string mutatedQuery = mutationFunction(query);

                products = await _context.Products
                .Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(mutatedQuery)))// ? true : EF.Functions.TrigramsAreSimilar(p.Description, query))
                .ToListAsync();
            }
            else
            {
                products = await _context.Products
                    .Where(p => p.SearchVector.Matches(query))// ? true : EF.Functions.TrigramsAreSimilar(p.Description, query))
                    .ToListAsync();
            }

            return products;
        }

        public static Func<string, string> OrMutation = (string query) =>
        {
            IEnumerable<string> words = query.Split(' ');
            query = string.Join(":* | ", words) + ":*";

            return query;
        };

        public static Func<string, string> AndMutation = (string query) =>
        {
            IEnumerable<string> words = query.Split(' ');
            query = string.Join(":* & ", words) + ":*";

            return query;
        };
    }
}