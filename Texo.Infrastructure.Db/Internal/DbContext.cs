﻿using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Texo.Infrastructure.Db.Entity;

namespace Texo.Infrastructure.Db.Internal
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        private readonly DbConnection _connection;

        public DbContext(DbConnection connection)
        {
            _connection = connection;
        }

        public DbSet<ProjectEntity> Projects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connection); //@"Data Source=:memory:");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectEntity>();
        }
    }
}