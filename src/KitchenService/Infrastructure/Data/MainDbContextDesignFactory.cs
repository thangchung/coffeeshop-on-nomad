using N8T.Infrastructure.EfCore;

namespace KitchenService.Infrastructure.Data;

public class MainDbContextDesignFactory : DbContextDesignFactoryBase<MainDbContext>
{
    public MainDbContextDesignFactory() : base("kitchendb")
    {
    }
}
