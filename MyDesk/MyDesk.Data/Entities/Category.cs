#nullable disable

namespace MyDesk.Data.Entities
{
    public partial class Category
    {
        public Category()
        {
            Desks = new HashSet<Desk>();
        }

        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool? DoubleMonitor { get; set; }
        public bool? NearWindow { get; set; }
        public bool? SingleMonitor { get; set; }
        public bool? Unavailable { get; set; }

        public virtual ICollection<Desk> Desks { get; set; }
    }
}