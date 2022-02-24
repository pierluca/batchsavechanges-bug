using System.Collections.Generic;
using System.Linq;

namespace BugRepro.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<MyEntityContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(MyEntityContext context)
        {
        }
    }
}
