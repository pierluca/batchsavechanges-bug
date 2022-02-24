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

            // The initialization creates the same entities as UpdateEntity, below
            Database.SetInitializer(new MyContextInitializer());

            Console.WriteLine("Try SaveChanges - WORKING");
            using (var ctx = new MyEntityContext())
            {
                UpdateEntity(ctx);

                var result = ctx.SaveChanges();

                Console.WriteLine($"SavesChanges OK : {result}");
            }

            AssertData();

            Console.WriteLine("Using BulkSaveChanges - WORKING");
            using (var ctx = new MyEntityContext())
            {
                UpdateEntity(ctx);

                ctx.BulkSaveChanges();

                Console.WriteLine($"BulkSaveChanges OK");
            }

            AssertData();

            Console.WriteLine("Using BatchSaveChanges - BROKEN");
            try
            {
                using (var ctx = new MyEntityContext())
                {
                    UpdateEntity(ctx);

                    ctx.Database.Log = Console.Write;

                    // This fails
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
            // Doing this would fix: ctx.MyEntities.First(entity => entity.Id == 1);
            var existingEntity = ctx.MyEntities.Include(entity => entity.MySubEntities).First(entity => entity.Id == 1);
            ctx.MyEntities.Remove(existingEntity);

            // Doing this would fix too: ctx.BatchSaveChanges();

            ctx.MyEntities.Add(new MyEntity()
            {
                Id = 1,
                Name = nameof(MyEntity),
                MySubEntities = new List<MySubEntity>()
                {
                    // Adding sub entities is not necessary to trigger the problem
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