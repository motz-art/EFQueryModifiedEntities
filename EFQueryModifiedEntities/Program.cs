using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFQueryModifiedEntities
{
    class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    class BlogContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Test");
            base.OnConfiguring(optionsBuilder);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Seed
            using (var ctx = new BlogContext())
            {
                ctx.Blogs.Add(new Blog {Name = "Blog 1"});
                ctx.Blogs.Add(new Blog {Name = "Blog 2" });
                ctx.Blogs.Add(new Blog {Name = "Blog 3" });
                ctx.SaveChanges();
            }

            // Steps to reproduce
            using (var context = new BlogContext())
            {
                {
                    var modifiedItem = context.Blogs.First(x => x.Name == "Blog 2");
                    modifiedItem.Name = "Other 2";
                }

                // Later in code.
                {
                    var items = context.Blogs.Where(x => x.Name.StartsWith("Blog")).ToList();

                    foreach (var item in items)
                    {
                        Debug.Assert(item.Name.StartsWith("Blog")); // Not all Names starts with "Blog"!
                    }
                }
            }
        }
    }
}
