using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using ProductsManager.Models;

namespace ProductsManager.Services;

public class SystemService : ISystemService
{
    private readonly string DbTableName = "warehouse_items";
    private readonly string DbName = "warehouse";
    private readonly string DbStoredProcedureName = "warehouse_seed";

    private WarehouseDbContext _context;

    /// <summary>
    /// initialize DbContext object
    /// </summary>
    /// <param name="_context"></param>
    public SystemService(WarehouseDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Initialaze database
    /// </summary>
    /// <param name="token"></param>
    public async Task InitializeDatabaseAsync(CancellationToken token)
    {
        _context.Database.EnsureCreated();
        if (!IsTableExistsAsync(token))
        {
            await CreateTables(token);
        }

        await SeedDatabase(token);
    }

    /// <summary>
    /// Fill database with initial data
    /// </summary>
    /// <param name="token"></param>
    private async Task SeedDatabase(CancellationToken token)
    {
        if (!await _context.WarehouseItems.AnyAsync())
        {
            await CreateStoredProcedure(token);
            await CallStoredProcedure(token);
        }
    }

    private async Task CreateTables(CancellationToken token)
    {
        var dbCreator = _context.GetService<IRelationalDatabaseCreator>();
        await dbCreator.CreateTablesAsync(token);
    }

    private async Task CallStoredProcedure(CancellationToken token)
    {
        var sql = $" EXEC {DbStoredProcedureName}";
        await _context.Database.ExecuteSqlRawAsync(sql, token);
    }

    public bool IsTableExistsAsync(CancellationToken cancellationToken)
    {
        var query = "SELECT COUNT(object_id) as count FROM [sys].[objects] WHERE [name] = @table_name";
        using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
        {
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@table_name", DbTableName);
                connection.Open();
                var count = (int)command.ExecuteScalar();
                connection.Close();

                return count > 0;
            }
        }
    }

    private async Task DropStoredProcedure(CancellationToken token)
    {
        var sql = @$"
        IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.{DbStoredProcedureName}'))
            exec('DROP PROCEDURE [dbo].[{DbStoredProcedureName}];')";

        await _context.Database.ExecuteSqlRawAsync(sql, token);
    }

    private async Task CreateStoredProcedure(CancellationToken token)
    {
        await DropStoredProcedure(token);

        var sql = @$"
        CREATE PROCEDURE dbo.{DbStoredProcedureName}
        AS
        BEGIN
            SET NOCOUNT ON;

            -- Insert suppliers
            INSERT INTO [dbo].[suppliers] (iname, status, add_date, upd_date)
            VALUES 
            ('ABBEYBDS LTD', 0, GETDATE(), GETDATE()),
            ('ABCTRANS', 0, GETDATE(), GETDATE()),
            ('ACADEMY', 0, GETDATE(), GETDATE()),
            ('ACCESSCORP', 0, GETDATE(), GETDATE()),
            ('AFRIPRUD', 0, GETDATE(), GETDATE()),
            ('AIICO', 0, GETDATE(), GETDATE()),
            ('AIRTELAFRI', 0, GETDATE(), GETDATE()),
            ('ALEX ENERGY', 0, GETDATE(), GETDATE()),
            ('ARADEL', 0, GETDATE(), GETDATE()),
            ('AUSTINLAZ', 0, GETDATE(), GETDATE()),
            ('BERGER CLARK', 0, GETDATE(), GETDATE()),
            ('BETAGLAS', 0, GETDATE(), GETDATE()),
            ('BUACEMENT MALL', 0, GETDATE(), GETDATE()),
            ('BUA FOODS', 0, GETDATE(), GETDATE()),
            ('CADBURY', 0, GETDATE(), GETDATE()),
            ('CAP', 0, GETDATE(), GETDATE()),
            ('CAVERTON', 0, GETDATE(), GETDATE()),
            ('CHAMPION', 0, GETDATE(), GETDATE());

            -- Insert warehouse items
            DECLARE @supplierCount INT = (SELECT COUNT(*) FROM [dbo].[suppliers]);
            DECLARE @i INT = 1;

            WHILE @i <= 22
            BEGIN
                INSERT INTO [dbo].[warehouse_items] (iname, unique_code, quantity, price, status, suppliers_id, add_date, upd_date)
                VALUES (
                    CASE @i
                        WHEN 1 THEN 'Food'
                        WHEN 2 THEN 'Rice'
                        WHEN 3 THEN 'Rice and cereals'
                        WHEN 4 THEN 'Bread'
                        WHEN 5 THEN 'Breads and cereals'
                        WHEN 6 THEN 'Pasta products'
                        WHEN 7 THEN 'Buns'
                        WHEN 8 THEN 'Cakes'
                        WHEN 9 THEN 'Biscuits'
                        WHEN 10 THEN 'Crispbread'
                        WHEN 11 THEN 'Puddings'
                        WHEN 12 THEN 'Pastry (savoury)'
                        WHEN 13 THEN 'Beef'
                        WHEN 14 THEN 'Pork'
                        WHEN 15 THEN 'Lamb'
                        WHEN 16 THEN 'Poultry'
                        WHEN 17 THEN 'Bacon and ham'
                        WHEN 18 THEN 'Sausages'
                        WHEN 19 THEN 'Offal'
                        WHEN 20 THEN 'Meat'
                        WHEN 21 THEN 'Frozen meat'
                        WHEN 22 THEN 'Fish'
                    END,
                    LEFT(CONVERT(VARCHAR(36), NEWID()), 5) + RIGHT('000' + CAST(ABS(CHECKSUM(NEWID())) % 1000 AS VARCHAR(3)), 3),
                    ABS(CHECKSUM(NEWID())) % 10000 + 1,
                    CAST(ABS(CHECKSUM(NEWID())) % 10000 / 100.0 AS DECIMAL(19, 4)),
                    0,
                    ABS(CHECKSUM(NEWID())) % @supplierCount + 1,
					GETDATE(),
                    GETDATE()
                );
                SET @i = @i + 1;
            END
        END";      

        await _context.Database.ExecuteSqlRawAsync(sql);
    }
}


