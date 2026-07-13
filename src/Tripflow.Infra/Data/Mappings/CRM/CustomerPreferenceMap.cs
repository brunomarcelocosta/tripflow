using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripflow.Domain.Entities.CRM;

namespace Tripflow.Infra.Data.Mappings.CRM;

public sealed class CustomerPreferenceMap : AuditableEntityMap<CustomerPreference>
{
    public override void Configure(EntityTypeBuilder<CustomerPreference> builder)
    {
        base.Configure(builder);

        builder.ToTable("customer_preferences");

        builder.Property(x => x.PreferredAirlines).HasMaxLength(1000);
        builder.Property(x => x.PreferredHotelCategories).HasMaxLength(1000);
        builder.Property(x => x.SeatPreferences).HasMaxLength(1000);
        builder.Property(x => x.MealRestrictions).HasMaxLength(1000);
        builder.Property(x => x.TravelPreferences).HasMaxLength(2000);
        builder.Property(x => x.GeneralNotes).HasMaxLength(4000);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Customer)
            .WithOne(x => x.Preference)
            .HasForeignKey<CustomerPreference>(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TenantId, x.CustomerId })
            .IsUnique();
    }
}
