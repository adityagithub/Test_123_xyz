
using Community.Core.Entities.CommunityUsers.Enums;
using Community.Core.Entities.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.Data.Mapping.Gdpr.Enums
{
    /// <summary>
    /// Represents a communityUser p mapping configuration
    /// </summary>
    public class EnumerationLogLevelTypeTableMap : CommunityEntityTypeConfiguration<EnumerationLogLevelTable>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<EnumerationLogLevelTable> builder)
        {
            builder.ToTable("LogLevelType", "lookup");
            builder.Property(p => p.Id).ValueGeneratedNever();
            builder.Property(p => p.Name).HasMaxLength(255).IsRequired();
            builder.Property(p => p.SystemName).HasMaxLength(255);
            builder.HasIndex(p => p.SystemName).IsUnique();

            builder.HasData(Enum.GetValues(typeof(EnumerationLogLevelType)).Cast<EnumerationLogLevelType>().Select(instance => new EnumerationLogLevelTable(instance)));

            base.Configure(builder);
        }

        #endregion
    }
}

