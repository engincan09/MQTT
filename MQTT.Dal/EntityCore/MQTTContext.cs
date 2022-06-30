using Microsoft.EntityFrameworkCore;
using MQTT.Entity.Models.Logs;
using MQTT.Entity.Models.Topics;

namespace MQTT.Dal.EntityCore
{
    public class MQTTContext : DbContext
    {
        public MQTTContext()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-VMT292V\SQLEXPRESS;Database=MQTTExample;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
        //entities
        public DbSet<Topic> Topics { get; set; }
        public DbSet<SeriLog> SeriLogs { get; set; }
    }
}
