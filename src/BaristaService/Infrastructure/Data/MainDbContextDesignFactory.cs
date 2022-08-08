using N8T.Infrastructure.EfCore;

namespace BaristaService.Infrastructure.Data;

public class MainDbContextDesignFactory : DbContextDesignFactoryBase<MainDbContext>
{
    public MainDbContextDesignFactory() : base("baristadb")
    {
    }
}
