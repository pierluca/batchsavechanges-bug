using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using FluentAssertions;

namespace BugRepro
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing database");
            Database.SetInitializer(new MyContextInitializer());

            // Using SaveChanges
            using (var ctx = new MyEntityContext())
            {
                UpdateEntity(ctx);

                Console.WriteLine("Try SaveChanges");

                var result = ctx.SaveChanges();

                Console.WriteLine($"SavesChanges OK : {result}");
            }

            AssertData();

            // Using BulkSaveChanges
            using (var ctx = new MyEntityContext())
            {
                UpdateEntity(ctx);

                Console.WriteLine("Try BulkSaveChanges");

                ctx.BulkSaveChanges();

                Console.WriteLine($"BulkSaveChanges OK");
            }

            AssertData();

            // Using BatchSaveChanges
            try
            {
                using (var ctx = new MyEntityContext())
                {
                    UpdateEntity(ctx);

                    Console.WriteLine("Try BatchSaveChanges");

                    var result = ctx.BatchSaveChanges();

                    Console.WriteLine($"BatchSavesChanges OK : {result}");
                }

                AssertData();
            }
            catch (DbUpdateException exception)
            {
                Console.WriteLine();
                Console.WriteLine(exception);
            }

            Console.ReadLine();
        }

        private static void UpdateEntity(MyEntityContext ctx)
        {
            var existingEntity = ctx.MyEntities.Include(entity => entity.MySubEntities).First(entity => entity.Id == 1);
            ctx.MyEntities.Remove(existingEntity);
            ctx.MyEntities.Add(new MyEntity()
            {
                Id = 1,
                Name = nameof(MyEntity),
                MySubEntities = new List<MySubEntity>()
                {
                    new MySubEntity()
                    {
                        Id = 1
                    },
                    new MySubEntity()
                    {
                        Id = 2
                    }
                }
            });
        }

        private static void AssertData()
        {
            try
            {
                using (var ctx = new MyEntityContext())
                {
                    var entities = ctx.MyEntities.Include(e => e.MySubEntities).ToList();

                    entities.Should().HaveCount(1);

                    var entity = entities.First();
                    entity.Id.Should().Be(1);
                    entity.Name.Should().Be(nameof(MyEntity));
                    entity.MySubEntities.Should().HaveCount(2);
                    entity.MySubEntities.Should().Contain(subEntity => subEntity.Id == 1);
                    entity.MySubEntities.Should().Contain(subEntity => subEntity.Id == 2);

                    Console.WriteLine("Data OK");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
