using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace BugRepro
{
    public class MyContextInitializer : DropCreateDatabaseAlways<MyEntityContext>
    {
        protected override void Seed(MyEntityContext context)
        {
            context.MyEntities.AddOrUpdate(new MyEntity
            {
                Id = 1,
                Name = nameof(MyEntity),
                MySubEntities = new List<MySubEntity>
                {
                    new MySubEntity
                    {
                        Id = 1
                    },
                    new MySubEntity
                    {
                        Id = 2
                    }
                }
            });
            context.SaveChanges();
        }
    }
}