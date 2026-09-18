using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartQueue.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartQueue.Persistence.Configurations
{
    internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasIndex(c => c.Name)
                .IsUnique();
            builder.Property(c => c.Version)
                .IsConcurrencyToken();
        }
    }
}
