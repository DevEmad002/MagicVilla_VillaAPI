using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Data
{
    public static class VillaStore
    {
        public static List<VillaDTO>villalist = new List<VillaDTO>
        {
            new VillaDTO { Id = 1, Name = "Royal Villa", CreateDate = DateTime.Now , Occupancy = 4 , Sqft = 100},
            new VillaDTO { Id = 2, Name = "Premium Pool Villa", CreateDate = DateTime.Now, Occupancy = 3 , Sqft = 300 }
        };
    }
}
