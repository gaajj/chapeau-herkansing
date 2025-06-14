using System.Collections.Generic;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.ViewModels.Management;
using ChapeauHerkansing.Services.Interfaces;
using ChapeauHerkansing.Repositories.Interfaces;


namespace ChapeauHerkansing.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepo;

        public StaffService(IStaffRepository staffRepo)
        {
            _staffRepo = staffRepo;
        }

        public List<Staff> GetAllStaff(bool includeDeleted)
        {
            return _staffRepo.GetAllStaff(includeDeleted);
        }



        public Staff GetStaffById(int id)
        {
            return _staffRepo.GetStaffById(id);
        }

        public void AddStaff(StaffCreateViewModel model)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            Staff newStaff = new Staff(0, model.FirstName, model.LastName, model.Username, hashedPassword, model.Role);
            _staffRepo.AddStaff(newStaff);
        }


        public void UpdateStaff(int id, StaffEditViewModel model)
        {
            string passwordToSave = model.Password;

            // Alleen opnieuw hashen als een nieuw wachtwoord is ingevuld
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                passwordToSave = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }

            Staff updated = new Staff(id, model.FirstName, model.LastName, model.Username, passwordToSave, model.Role, model.IsDeleted);
            _staffRepo.UpdateStaff(updated);
        }


        public bool ToggleStaffActive(int id)
        {
            return _staffRepo.ToggleStaffActive(id);
        }

        public Staff? GetByUsername(string username)
        {
            return _staffRepo.GetStaffByUsername(username);
        }


        public bool UsernameExists(string username)
        {
            return _staffRepo.GetStaffByUsername(username) != null;
        }

    }
}
