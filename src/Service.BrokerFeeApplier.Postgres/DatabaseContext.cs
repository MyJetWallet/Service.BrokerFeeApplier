using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MyJetWallet.Sdk.Postgres;
using MyJetWallet.Sdk.Service;
using Service.BrokerFeeApplier.Domain.Models.FireblocksWithdrawals;
using Service.BrokerFeeApplier.Postgres.Models;

namespace Service.BrokerFeeApplier.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        public const string Schema = "broker_fee_applier";

        private const string FireblocksFeeApplicationTableName = "fireblocks_fee_application";

        private Activity _activity;

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<FireblocksFeeApplicationEntity> FeeApplication { get; set; }


        public static DatabaseContext Create(DbContextOptionsBuilder<DatabaseContext> options)
        {
            var activity = MyTelemetry.StartActivity($"Database context {Schema}")?.AddTag("db-schema", Schema);

            var ctx = new DatabaseContext(options.Options) { _activity = activity };

            return ctx;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            SetFeeApplicationEntry(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetFeeApplicationEntry(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FireblocksFeeApplicationEntity>().ToTable(FireblocksFeeApplicationTableName);
            modelBuilder.Entity<FireblocksFeeApplicationEntity>().Property(e => e.Id).UseIdentityColumn();
            modelBuilder.Entity<FireblocksFeeApplicationEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<FireblocksFeeApplicationEntity>().Property(e => e.TransactionId);
            modelBuilder.Entity<FireblocksFeeApplicationEntity>().Property(e => e.Amount);
            modelBuilder.Entity<FireblocksFeeApplicationEntity>().Property(e => e.AssetSymbol).HasMaxLength(64);
            modelBuilder.Entity<FireblocksFeeApplicationEntity>().Property(e => e.Comment).HasMaxLength(512);
            modelBuilder.Entity<FireblocksFeeApplicationEntity>().Property(e => e.Status).HasDefaultValue(FireblocksFeeApplicationStatus.InProgress);
            modelBuilder.Entity<FireblocksFeeApplicationEntity>().Property(e => e.EventDate).HasDefaultValue(DateTime.MinValue);

            modelBuilder.Entity<FireblocksFeeApplicationEntity>().HasIndex(e => e.Status);
            modelBuilder.Entity<FireblocksFeeApplicationEntity>().HasIndex(e => e.TransactionId).IsUnique();
        }
    }
}