using AutoMapper;
using ECommerce.Api.Orders.Db;
using ECommerce.Api.Orders.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Orders.Providers
{
    public class OrdersProvider : IOrdersProvider 
    {
        private readonly OrdersDbContext dbContext;
        private readonly ILogger logger;
        private readonly IMapper mapper;

        public OrdersProvider(OrdersDbContext dbContext, ILogger<OrdersProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;

            SeedData();
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Order> Orders, string ErrorMessage)> GetOrderAsync(int id)
        {
            try
            {
                var orders = await dbContext.Orders.Where(o => o.CustomerId == id).ToListAsync();
                if (orders != null)
                {
                    var result = mapper.Map<IEnumerable<Db.Order>, IEnumerable<Models.Order>>(orders);

                    return (true, result, null);
                }
                return (false, null, "Not found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);

            }
        }

        private void SeedData()
        {
            if (!dbContext.Orders.Any())
            {
                dbContext.Orders.Add(new Db.Order() { Id = 1, CustomerId = 1, OrderDate = DateTime.Now, Total = 100, Items = null });
                dbContext.Orders.Add(new Db.Order() { Id = 2, CustomerId = 1, OrderDate = new DateTime(2022, 11, 15), Total = 40.48m, Items = new List<Db.OrderItem> { new Db.OrderItem() { Id = 2, ProductId = 1, Quantity = 3, UnitPrice = 100 }, new Db.OrderItem() { Id = 6, ProductId = 2, Quantity = 5, UnitPrice = 9.99m } } });
                dbContext.SaveChanges();
            }
        }
    }
}
