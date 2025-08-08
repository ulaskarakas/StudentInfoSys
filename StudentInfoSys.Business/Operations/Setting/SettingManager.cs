using StudentInfoSys.Data.Entities;
using StudentInfoSys.Data.Repositories;
using StudentInfoSys.Data.UnitOfWork;

namespace StudentInfoSys.Business.Operations.Setting
{
    public class SettingManager : ISettingService
    {
        private readonly IRepository<SettingEntity> _settingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SettingManager(IRepository<SettingEntity> settingRepository, IUnitOfWork unitOfWork)
        {
            _settingRepository = settingRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ToogleMaintenance()
        {
            var setting = await _settingRepository.GetByIdAsync(1);
            if (setting == null)
                throw new Exception("Setting not found");

            setting.MaintenanceMode = !setting.MaintenanceMode;

            _settingRepository.Update(setting);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("An error was encountered while updating the maintenance status");
            }
        }

        public async Task<bool> GetMaintenanceState()
        {
            var maintenanceState = await _settingRepository.GetByIdAsync(1);
            return maintenanceState?.MaintenanceMode ?? false;
        }
    }
}