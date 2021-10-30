using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Community.Core.Entities.Logging;

namespace Community.Data.Mapping.Logging
{
    /// <summary>
    /// Represents an activity log type mapping configuration
    /// </summary>
    public class CommunityActivityLogTypeMap : CommunityEntityTypeConfiguration<CommunityActivityLogType>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<CommunityActivityLogType> builder)
        {
            builder.ToTable(nameof(CommunityActivityLogType));
            builder.HasKey(logType => logType.Id);

            builder.Property(logType => logType.SystemKeyword).HasMaxLength(100).IsRequired();
            builder.Property(logType => logType.Name).HasMaxLength(200).IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}