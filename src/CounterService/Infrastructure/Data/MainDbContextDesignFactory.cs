using N8T.Infrastructure.EfCore;

namespace CounterService.Infrastructure.Data;

public class MainDbContextDesignFactory : DbContextDesignFactoryBase<MainDbContext>
{
    public MainDbContextDesignFactory() : base("counterdb")
    {
    }
}
