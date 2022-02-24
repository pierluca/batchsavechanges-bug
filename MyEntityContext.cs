using System.Data.Entity;

namespace BugRepro
{
    public class MyEntityContext : DbContext
    {
        public MyEntityContext() 
            : base("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=BugRepro;Integrated Security=SSPI;")
        {
        }

        public DbSet<MyEntity> MyEntities { get; set; }
    }
}