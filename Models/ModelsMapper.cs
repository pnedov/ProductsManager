using AutoMapper;

namespace ProductsManager.Models;

public class ModelsMapper : Profile
{
    public ModelsMapper()
    {
        CreateMap<GetWarehouseItemRequest, WarehouseItem>();
    }
}

