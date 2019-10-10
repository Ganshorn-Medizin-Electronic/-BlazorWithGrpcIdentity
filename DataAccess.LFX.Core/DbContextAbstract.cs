using DataAccess.LFX.Core.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataAccess.LFX.Core
{
    public partial class DbContextAbstract : DbContext
    {
        protected DbContextAbstract()
        {
        }

        protected DbContextAbstract(DbContextOptions options)
            : base(options)
        {
        }
    }
}



