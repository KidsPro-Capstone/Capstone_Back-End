﻿using Application.Configurations;
using Application.ErrorHandlers;
using Application.Interfaces.IRepositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context, ILogger<BaseRepository<Order>> logger) : base(context, logger)
        {
        }

        public async Task<(Order?,string?)> GetByOrderCode(Func<int,string> generateOrderCode, bool decision)
        {
            string orderCode = generateOrderCode(13);
           
            switch (decision)
            {
                // Search By OrderCode
                case true:
                    return (await _dbSet.Include(x=> x.OrderDetails)!.ThenInclude(x=> x.Course)
                            .FirstOrDefaultAsync(x=> x.OrderCode == orderCode),null);
                // tìm kiếm xem order code có tồn tại trong system chưa, truyền vào false, nếu mà tìm không có bằng null thì trả về 
                // theo order code để tạo mới order, còn nếu trùng thì trả về 1 order để check rồi tạo orderCode khác
                case false:
                    return (await _dbSet.FirstOrDefaultAsync(x => x.OrderCode == orderCode),orderCode);
            }
        }

        public async Task<Order?> GetOrderByStatusAsync(int parentId, int orderId,OrderStatus status)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync
                (x=> x.Id == orderId && x.ParentId==parentId && x.Status == status);
        }

        public async Task<List<Order>?> GetListOrderAsync(OrderStatus status,int parentId,string role)
        {
            var query = _dbSet.AsNoTracking();
            if (role == Constant.ParentRole)
            {
                query = query.Where(x => x.ParentId == parentId);
            }
            switch (status)
            {
                case OrderStatus.Payment:
                    return await query.Where(x => x.Status == OrderStatus.Payment)
                        .Include(x=>x.OrderDetails)!.ThenInclude(x=>x.Course).ToListAsync();
                case OrderStatus.Pending:
                    return await query.Where(x => x.Status == OrderStatus.Pending)
                        .Include(x=>x.OrderDetails)!.ThenInclude(x=>x.Course).ToListAsync();
                case OrderStatus.Success:
                    return await query.Where(x => x.Status == OrderStatus.Success)
                        .Include(x=>x.OrderDetails)!.ThenInclude(x=>x.Course).ToListAsync();
                case OrderStatus.RequestRefund:
                    return await query.Where(x => x.Status == OrderStatus.RequestRefund)
                        .Include(x=>x.OrderDetails)!.ThenInclude(x=>x.Course).ToListAsync();
                case OrderStatus.Refunded:
                    return await query.Where(x => x.Status == OrderStatus.Refunded)
                        .Include(x=>x.OrderDetails)!.ThenInclude(x=>x.Course).ToListAsync();
            }
            throw new UnauthorizedException("Parent Id doesn't exist");
        }

        public async Task<Order?> GetOrderDetail(int parentId, int orderId)
        {
            return (await _dbSet.AsNoTracking().Include(x => x.OrderDetails)!
                .ThenInclude(x=> x.Students)!.ThenInclude(x=> x.Account)
                .Include(x => x.OrderDetails)!.ThenInclude(x => x.Course)
                .Include(x => x.Transaction).Include(x => x.Voucher)
                .Include(x=>x.Parent).ThenInclude(x=>x!.Account)
                .FirstOrDefaultAsync(x => x.Id == orderId && x.ParentId == parentId));
        }
        
    }
}
