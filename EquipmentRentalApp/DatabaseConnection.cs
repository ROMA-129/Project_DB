namespace IndustrialEquipmentRentalSystem
{
    public static class DatabaseConnection
    {
        public static string ConnectionString { get; } =
            "Server=.;Database=EquipmentREntalDB;Integrated Security=True;";
    }
}