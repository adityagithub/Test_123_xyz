using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Community.Core.Entities.Logging;

namespace Community.Data.Mapping.Logging
{
    /// <summary>
    /// Represents an activity log mapping configuration
    /// </summary>
    public class CommunityActivityLogMap : CommunityEntityTypeConfiguration<CommunityActivityLog>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<CommunityActivityLog> builder)
        {
            builder.ToTable(nameof(CommunityActivityLog));
            builder.HasKey(logItem => logItem.Id);

            builder.Property(logItem => logItem.Comment).IsRequired();
            builder.Property(logItem => logItem.IpAddress).HasMaxLength(200);
            builder.Property(logItem => logItem.EntityName).HasMaxLength(400);

            builder.HasOne(logItem => logItem.ActivityLogType)
                .WithMany()
                .HasForeignKey(logItem => logItem.ActivityLogTypeId)
                .IsRequired();

            builder.HasOne(logItem => logItem.CommunityUser)
                .WithMany()
                .HasForeignKey(logItem => logItem.CommunityUserId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}