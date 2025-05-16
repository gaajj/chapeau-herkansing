using ChapeauHerkansing.Models;
using System.Collections.Generic;

namespace ChapeauHerkansing.Repositories
{
    public interface IStaffRepository
    {
        List<Staff> GetAllStaff();
        Staff GetStaffById(int id);
        void AddStaff(Staff staff);
        void UpdateStaff(Staff staff);
        void DeleteStaff(int id); 
    }
}
