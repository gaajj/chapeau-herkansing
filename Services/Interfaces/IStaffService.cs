using System.Collections.Generic;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.ViewModels.Management;

namespace ChapeauHerkansing.Services.Interfaces
{
    public interface IStaffService
    {
        StaffCollection GetAllStaff(bool includeDeleted = false);
        Staff GetStaffById(int id);
        void AddStaff(StaffCreateViewModel model);
        void UpdateStaff(int id, StaffEditViewModel model);
        bool ToggleStaffActive(int id);
        Staff? GetByUsername(string username);
    }
}
