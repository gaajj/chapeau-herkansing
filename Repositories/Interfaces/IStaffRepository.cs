using System.Collections.Generic;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.Repositories.Interfaces
{
    public interface IStaffRepository
    {
        List<Staff> GetAllStaff();
        Staff GetStaffById(int id);
        void AddStaff(Staff staff);
        void UpdateStaff(Staff staff);
        bool ToggleStaffActive(int id);
        Staff? GetStaffByUsername(string username);
    }
}
