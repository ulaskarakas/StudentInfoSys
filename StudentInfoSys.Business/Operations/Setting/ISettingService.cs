namespace StudentInfoSys.Business.Operations.Setting
{
    public interface ISettingService
    {
        Task ToogleMaintenance();
        Task<bool> GetMaintenanceState();
    }
}