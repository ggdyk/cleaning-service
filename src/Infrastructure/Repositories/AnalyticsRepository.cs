using Application.DTOs.Admin;
using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Infrastructure.Repositories;

public class AnalyticsRepository : IAnalyticsRepository
{
    private readonly ApplicationDbContext _context;

    public AnalyticsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AnalyticsSummaryDto> GetSummaryAsync()
    {
        var now = DateTime.UtcNow;
        var monthAgo = now.AddMonths(-1);
        var weekAgo = now.AddDays(-7);

        var totalOrders = await _context.Orders.CountAsync();
        var ordersThisMonth = await _context.Orders.CountAsync(o => o.CreatedAt >= monthAgo);
        var ordersThisWeek = await _context.Orders.CountAsync(o => o.CreatedAt >= weekAgo);
        var totalUsers = await _context.Users.CountAsync();
        var totalCallbackRequests = await _context.CallbackRequests.CountAsync();

        return new AnalyticsSummaryDto(
            TotalOrders: totalOrders,
            OrdersThisMonth: ordersThisMonth,
            OrdersThisWeek: ordersThisWeek,
            TotalUsers: totalUsers,
            TotalCallbackRequests: totalCallbackRequests
        );
    }

    public async Task<IReadOnlyList<PopularServiceDto>> GetPopularServicesAsync(
        DateTime? from,
        DateTime? to,
        int top)
    {
        // Фильтруем заказы по периоду, затем JOIN с OrderServices и группируем по ServiceId
        var ordersQuery = _context.Orders.AsQueryable();
        if (from.HasValue) ordersQuery = ordersQuery.Where(o => o.CreatedAt >= from.Value);
        if (to.HasValue)   ordersQuery = ordersQuery.Where(o => o.CreatedAt <= to.Value);

        var counts = await ordersQuery
            .Join(
                _context.OrderServices,
                o  => o.Id,
                os => os.OrderId,
                (o, os) => os.ServiceId)
            .GroupBy(serviceId => serviceId)
            .Select(g => new { ServiceId = g.Key, OrderCount = g.Count() })
            .OrderByDescending(g => g.OrderCount)
            .Take(top)
            .ToListAsync();

        if (counts.Count == 0)
            return [];

        // Получаем актуальные названия из каталога (Services — Catalog контекст, тот же DbContext)
        var serviceIds = counts.Select(c => c.ServiceId).ToList();
        var serviceNames = await _context.Services
            .Where(s => serviceIds.Contains(s.Id))
            .Select(s => new { s.Id, NameRu = s.Name.Ru, NameKk = s.Name.Kk, NameEn = s.Name.En })
            .ToDictionaryAsync(s => s.Id);

        return counts.Select(c =>
        {
            serviceNames.TryGetValue(c.ServiceId, out var names);
            return new PopularServiceDto(
                c.ServiceId,
                names?.NameRu ?? $"Услуга #{c.ServiceId}",
                names?.NameKk ?? $"Услуга #{c.ServiceId}",
                names?.NameEn ?? $"Service #{c.ServiceId}",
                c.OrderCount
            );
        }).ToList();
    }

    public async Task<IReadOnlyList<OrdersByPeriodItemDto>> GetOrdersByPeriodAsync(
        DateTime startDate,
        DateTime endDate,
        GroupByPeriod groupBy)
    {
        // Загружаем только поле CreatedAt (минимальный трафик из БД)
        var dates = await _context.Orders
            .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
            .Select(o => o.CreatedAt)
            .ToListAsync();

        return groupBy switch
        {
            GroupByPeriod.Day   => GroupByDay(dates, startDate, endDate),
            GroupByPeriod.Week  => GroupByWeek(dates, startDate, endDate),
            GroupByPeriod.Month => GroupByMonth(dates, startDate, endDate),
            _ => []
        };
    }

    // -------------------------------------------------------------------------
    // Группировка с заполнением пустых периодов (нужно для корректного графика)
    // -------------------------------------------------------------------------

    private static IReadOnlyList<OrdersByPeriodItemDto> GroupByDay(
        List<DateTime> dates, DateTime startDate, DateTime endDate)
    {
        var byDay = dates
            .GroupBy(d => d.Date)
            .ToDictionary(g => g.Key, g => g.Count());

        var result = new List<OrdersByPeriodItemDto>();
        for (var day = startDate.Date; day <= endDate.Date; day = day.AddDays(1))
        {
            result.Add(new OrdersByPeriodItemDto(
                day.ToString("yyyy-MM-dd"),
                byDay.GetValueOrDefault(day, 0)));
        }
        return result;
    }

    private static IReadOnlyList<OrdersByPeriodItemDto> GroupByWeek(
        List<DateTime> dates, DateTime startDate, DateTime endDate)
    {
        // Ключ строится по ISO-году и номеру недели, чтобы корректно обрабатывать
        // граничные дни (например, 31 декабря может быть в неделе 1 следующего года)
        var byWeek = dates
            .GroupBy(d => WeekKey(d))
            .ToDictionary(g => g.Key, g => g.Count());

        var result = new List<OrdersByPeriodItemDto>();

        // Начинаем с понедельника ISO-недели, содержащей startDate
        var weekStart = ISOWeek.ToDateTime(
            ISOWeek.GetYear(startDate.Date),
            ISOWeek.GetWeekOfYear(startDate.Date),
            DayOfWeek.Monday);

        while (weekStart <= endDate.Date)
        {
            var key = WeekKey(weekStart);
            result.Add(new OrdersByPeriodItemDto(key, byWeek.GetValueOrDefault(key, 0)));
            weekStart = weekStart.AddDays(7);
        }
        return result;
    }

    private static IReadOnlyList<OrdersByPeriodItemDto> GroupByMonth(
        List<DateTime> dates, DateTime startDate, DateTime endDate)
    {
        var byMonth = dates
            .GroupBy(d => d.ToString("yyyy-MM"))
            .ToDictionary(g => g.Key, g => g.Count());

        var result = new List<OrdersByPeriodItemDto>();
        var month = new DateTime(startDate.Year, startDate.Month, 1);
        var lastMonth = new DateTime(endDate.Year, endDate.Month, 1);

        while (month <= lastMonth)
        {
            var key = month.ToString("yyyy-MM");
            result.Add(new OrdersByPeriodItemDto(key, byMonth.GetValueOrDefault(key, 0)));
            month = month.AddMonths(1);
        }
        return result;
    }

    private static string WeekKey(DateTime d)
        => $"{ISOWeek.GetYear(d)}-W{ISOWeek.GetWeekOfYear(d):D2}";
}
