using Bet.Models;
using Microsoft.EntityFrameworkCore;

namespace MvcBet.Data
{
    public class MvcBetContext : DbContext
    {
        // Конструктор для передачи настроек в базовый класс DbContext
        public MvcBetContext(DbContextOptions<MvcBetContext> options)
            : base(options) { }

        // Определите DbSet для каждой сущности (таблицы) в базе данных
        public DbSet<BetViewModel> Bets { get; set; }

        // При необходимости переопределите метод OnModelCreating для настройки моделей
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Дополнительные настройки моделей (например, индексы, ключи и т.д.)
        }
    }
}
