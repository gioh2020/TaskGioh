using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainTask = TaskManagement.Domain.Entities.TaskItem;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<DomainTask>
{
    public void Configure(EntityTypeBuilder<DomainTask> builder)
    {
        builder.ToTable("Tasks", t =>
        {
            t.HasCheckConstraint(
                "CK_Tasks_AdditionalInfo_IsJson",
                "AdditionalInfo IS NULL OR ISJSON(AdditionalInfo) = 1");
        });

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("Title");

        builder.Property(t => t.Description)
            .IsRequired(false)
            .HasMaxLength(1000)
            .HasColumnName("Description");

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnName("Status");

        builder.Property(t => t.AssignedUserId)
            .IsRequired()
            .HasColumnName("AssignedUserId");

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasColumnName("CreatedAt");

        builder.Property(t => t.AdditionalInfo)
            .IsRequired(false)
            .HasColumnType("NVARCHAR(MAX)")
            .HasColumnName("AdditionalInfo");

        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_Tasks_Status");

        builder.HasIndex(t => t.AssignedUserId)
            .HasDatabaseName("IX_Tasks_AssignedUserId");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_Tasks_CreatedAt");

        builder.HasOne(t => t.AssignedUser)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.AssignedUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
