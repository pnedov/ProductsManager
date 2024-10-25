namespace ProductsManager.Services;

public interface ISystemService
{
    Task InitializeDatabaseAsync(CancellationToken token);
}

