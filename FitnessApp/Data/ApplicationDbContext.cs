namespace FitnessApp.Data
{
    using FitnessApp.Models;
    using FitnessApp.Models.Common;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System.Reflection;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private static readonly MethodInfo SetIsDeletedQueryFilterMethod =
            typeof(ApplicationDbContext).GetMethod(
                nameof(SetIsDeletedQueryFilter),
                BindingFlags.NonPublic | BindingFlags.Static);
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseInWorkoutDay> ExercisesInWorkoutDays { get; set; }
        public DbSet<WorkoutDay> WorkoutDays { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<UserConversations> UserConversations { get; set; }

        public override int SaveChanges() => this.SaveChanges(true);

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            this.SaveChangesAsync(true, cancellationToken);
        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Needed for Identity models configuration
            base.OnModelCreating(builder);

            //Entities
            builder.Entity<ExerciseInWorkoutDay>()
                .HasKey(ew => new { ew.ExerciseId, ew.WorkoutDayId });

            builder.Entity<ExerciseInWorkoutDay>()
                .HasOne(e => e.Exercise)
                .WithMany(w => w.ExerciseInWorkoutDay)
                .HasForeignKey(e => e.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ExerciseInWorkoutDay>()
                .HasOne(w => w.WorkoutDay)
                .WithMany(e => e.ExercisesInWorkoutDays)
                .HasForeignKey(w => w.WorkoutDayId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WorkoutDay>()
                .HasOne(w => w.WorkoutPlan)
                .WithMany(wd => wd.WorkoutDays)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<ExerciseInWorkoutDay>()
                .HasKey(ew => new { ew.ExerciseId, ew.WorkoutDayId });

            builder
                .Entity<Message>()
                .HasOne(m => m.Recipient)
                .WithMany(r => r.RecievedMessages)
                .HasForeignKey(m => m.RecipientId);

            builder
                .Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(s => s.SentMessages)
                .HasForeignKey(m => m.SenderId);

            builder
                .Entity<UserConversations>()
                .HasKey(uc => new { uc.UserId, uc.ConversationId });

            builder
                .Entity<UserConversations>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserConversations)
                .HasForeignKey(uc => uc.UserId);

            builder
                .Entity<UserConversations>()
                .HasOne(uc => uc.Conversation)
                .WithMany(c => c.UserConversations)
                .HasForeignKey(uc => uc.ConversationId);


            var entityTypes = builder.Model.GetEntityTypes().ToList();

            // Set global query filter for not deleted entities only
            var deletableEntityTypes = entityTypes
                .Where(et => et.ClrType != null && typeof(IDeletableEntity).IsAssignableFrom(et.ClrType));
            foreach (var deletableEntityType in deletableEntityTypes)
            {
                var method = SetIsDeletedQueryFilterMethod.MakeGenericMethod(deletableEntityType.ClrType);
                method.Invoke(null, new object[] { builder });
            }

            // Disable cascade delete
            var foreignKeys = entityTypes
                .SelectMany(e => e.GetForeignKeys().Where(f => f.DeleteBehavior == DeleteBehavior.Cascade));
            foreach (var foreignKey in foreignKeys)
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        private static void SetIsDeletedQueryFilter<T>(ModelBuilder builder)
           where T : class, IDeletableEntity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }

        private void ApplyAuditInfoRules()
        {
            var changedEntries = this.ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is IAuditInfo &&
                    (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in changedEntries)
            {
                var entity = (IAuditInfo)entry.Entity;
                if (entry.State == EntityState.Added && entity.CreatedOn == default)
                {
                    entity.CreatedOn = DateTime.Now;
                }
                else
                {
                    entity.ModifiedOn = DateTime.Now;
                }
            }
        }
    }
}