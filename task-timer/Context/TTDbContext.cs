using Microsoft.EntityFrameworkCore;
using System;
using task_timer.Models;

namespace task_timer.Context
{
    public class TTDbContext: DbContext
    {
        public TTDbContext(DbContextOptions<TTDbContext> options) : base(options)
        {

        }


    }
}
