using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Test.Models;
using Test.Services;


namespace Test.Data
{
    public class DataContext : IdentityDbContext
    {
        //public DbSet<Customer> Customers { get; set; }
        //public DbSet<Book> Books { get; set; }



        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //var assemblyExportedTypes = typeof(Startup).Assembly.ExportedTypes;
            //foreach (var type in typeof(Startup).Assembly.ExportedTypes.Where(x =>
            //    typeof(IApplicationEntity).IsAssignableFrom(x)
            //    && !x.IsInterface && !x.IsAbstract))
            //{
            //    builder.Entity(type)
            //        .Property(type.GetProperties()
            //            .FirstOrDefault(x => x.GetCustomAttribute<KeyAttribute>() != null)
            //            ?.Name ?? throw new KeyNotFoundException(type.Name))
            //        .ValueGeneratedOnAdd();
            //}
            foreach (var entityType in IncludedEntities.Types)
            {
                builder
                    .Entity(entityType)
                    .HasKey(nameof(IApplicationEntity.Id));

                //builder
                //    .Entity(entityType)
                //    .Property<Guid>(nameof(IApplicationEntity.Id))
                //    .IsRequired()
                //    .ValueGeneratedOnAdd();

                //builder
                //    .Entity(entityType)
                //    .Property<DateTime>("CreatedOn")
                //    .IsRequired()
                //    .ValueGeneratedOnAdd();

                //builder
                //    .Entity(entityType)
                //    .Property<DateTime>("ModifiedOn")
                //    .IsRequired()
                //    .ValueGeneratedOnAddOrUpdate();
            }


        }

    }
}



    
