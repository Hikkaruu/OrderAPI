using Microsoft.EntityFrameworkCore;
using SimpleProductOrder.Data;
using SimpleProductOrder.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleProductOrder.Services
{
    public class ProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public bool CreateProduct(Product product)
        {
             _context.Add(product);

            return Save();
        }

        public bool DeleteProduct(Product product)
        {
            _context.Remove(product);
            return Save();
        }

        public bool DeleteProduct(int productId)
        {
            var productToDelete = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (productToDelete == null)
            {
                return false;
            }

            var checkDelete = _context.OrderProducts.FirstOrDefault(op => op.ProductId == productId);
            if(checkDelete == null)
            {

                _context.Products.Remove(productToDelete);
                return Save();
            }


            return false;
        }

        public bool DeleteProducts(List<Product> products)
        {
            _context.RemoveRange(products);
            return Save();
        }

        public Product GetProduct(int productId)
        {
            return _context.Products.Where(r => r.Id == productId).FirstOrDefault();
        }

        public ICollection<Product> GetProducts()
        {
            return _context.Products.ToList();
        }

        public bool ProductExists(int productId)
        {
            return _context.Products.Any(r => r.Id == productId);
        }

        public bool OrderProductExists(int productId)
        {
            return _context.OrderProducts.Any(op => op.ProductId == productId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateProduct(Product product)
        {
            _context.Update(product);
            return Save();
        }

        public ICollection<Review> GetReviewsOfProduct(int productId)
        {
            return _context.Reviews.Where(r => r.Product.Id == productId).ToList();
        }

        public ICollection<Product> GetProductsByCategory(int categoryId)
        {
            return _context.Products.Where(p => p.Category.Id == categoryId).ToList();
        }
    }
}
