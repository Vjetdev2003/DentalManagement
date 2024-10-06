using DentalManagement.DomainModels;
using DentalManagement.Web.Models;
using DentalManagement.Web.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DentalManagement.Web
{
    public static class SelectListHelper
    {
       
        

        /// <summary>
        /// Loại hàng 
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> Medicines()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "0",
                Text = "-- Chọn loại thuốc --"
            });
            return list;
        }

        public static List<SelectListItem> Patients(IRepository<Patient> patientRepository)
        {
                        List<SelectListItem> list = new List<SelectListItem>
                {
                    new SelectListItem()
                    {
                        Value = "0",
                        Text = "-- Chọn bệnh nhân --"
                    }
                };

            var patients = patientRepository.GetAllAsync().Result;

            foreach (var item in patients)
            {
                list.Add(new SelectListItem()
                {
                    Value = item.PatientId.ToString(),
                    Text = item.PatientName
                });
            }

            return list;
        }

       

        /// <summary>
        /// Nhân viên
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> Employees(IRepository<Employee>repositoryEmployee)
        {
            List<SelectListItem> list = new List<SelectListItem>
    {
        new SelectListItem()
        {
            Value = "0",
            Text = "-- Chọn nhân viên --"
        }
            };

            var patients = repositoryEmployee.GetAllAsync().Result;

            foreach (var item in patients)
            {
                list.Add(new SelectListItem()
                {
                    Value = item.EmployeeId.ToString(),
                    Text = item.FullName
                });
            }

            return list;
        }
        /// <summary>
        /// Vai trò
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> RoleNames()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "employee",
                Text = "Nhân viên"
            });

            list.Add(new SelectListItem()
            {
                Value = "admin",
                Text = "Quản trị hệ thống"
            });
            list.Add(new SelectListItem()
            {
                Value = "patient",
                Text = "Bệnh nhân"
            });
            list.Add(new SelectListItem()
            {
                Value = "dentist",
                Text = "Nha sĩ"
            });

            list.Add(new SelectListItem()
            {
                Value = "employee,admin,patient,dentist",
                Text = "Nhân viên, Quản trị hệ thống,Bệnh nhân,Nha sĩ"
            });
            return list;
        }
       
     

    }

}
