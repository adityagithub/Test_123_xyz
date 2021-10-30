using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Community.Core.Entities.Logging;
using System;

namespace Community.Data.Mapping.Logging
{
    /// <summary>
    /// Represents a log mapping configuration
    /// </summary>
    public class CommunityLogMap : CommunityEntityTypeConfiguration<CommunityLog>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<CommunityLog> builder)
        {
            builder.ToTable(nameof(CommunityLog));
            builder.HasKey(logItem => logItem.Id);

            builder.HasIndex(p => p.CreatedOnUtc);

            builder.Property(logItem => logItem.ShortMessage).IsRequired();
            builder.Property(logItem => logItem.IpAddress).HasMaxLength(200);
            
            builder.HasOne(logItem => logItem.CommunityUser)
                .WithMany()
                .HasForeignKey(logItem => logItem.CommunityUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(p => p.LogLevel).HasConversion(x => (int)x, x => (EnumerationLogLevelType)x);

            base.Configure(builder);
        }

        #endregion
    }
}