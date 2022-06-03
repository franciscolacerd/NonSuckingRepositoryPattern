using Microsoft.EntityFrameworkCore;

namespace NSRP.Persistence
{
    public class NsrpContext : DbContext
    {
        public NsrpContext(DbContextOptions options) : base(options)
        {
        }

    }
}
