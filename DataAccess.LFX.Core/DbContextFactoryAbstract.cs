using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.LFX.Core
{
    public abstract class DbContextFactoryAbstract
    {        
        public abstract DbContextAbstract CreateDbContext();
    }
}
