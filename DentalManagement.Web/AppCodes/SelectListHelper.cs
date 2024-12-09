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
        public static List<SelectListItem> Medicines(IRepository<Medicine> medicineRepository)
        {
            List<SelectListItem> list = new List<SelectListItem>
                {
                    new SelectListItem()
                    {
                        Value = "0",
                        Text = "-- Chọn loại thuốc --"
                    }
                };

            var medicines = medicineRepository.GetAllAsync().Result;

            foreach (var item in medicines)
            {
                list.Add(new SelectListItem()
                {
                    Value = item.MedicineId.ToString(),
                    Text = item.MedicineName
                });
            }

            return list;
        }
        public static IEnumerable<SelectListItem> GetStatusAppointment()
        {
            return new List<SelectListItem>
        {
            new SelectListItem {Value = "0", Text = "-- Trạng thái--"},
            new SelectListItem { Value = Constants.APPOINTMENT_INIT.ToString(),
                Text = "Lịch hẹn đã đặt. Đang chờ xác nhận." },
            new SelectListItem { Value = Constants.APPOINTMENT_CONFIRMED.ToString(),
                Text = "Lịch hẹn đã xác nhận. Đang chờ đến ngày hẹn." },
            new SelectListItem { Value = Constants.APPOINTMENT_IN_PROGRESS.ToString(),
                Text = "Lịch hẹn đang diễn ra." },
            new SelectListItem { Value = Constants.APPOINTMENT_FINISHED.ToString(),
                Text = "Lịch hẹn đã hoàn tất." },
            new SelectListItem { Value = Constants.APPOINTMENT_CANCELLED.ToString(),
                Text = "Lịch hẹn đã hủy." },
            new SelectListItem { Value = Constants.APPOINTMENT_NO_SHOW.ToString(),
                Text = "Bệnh nhân không đến khám." }
        };
        }
        public static List<SelectListItem> GetStatusInvoice()
        {
            List<SelectListItem> list = new List<SelectListItem>
    {
        new SelectListItem()
        {
            Value = "0",
            Text = "-- Chọn trạng thái --"
        }
    };

            // Adding the invoice statuses
            list.Add(new SelectListItem
            {
                Value = Constants_Invoice.INVOICE_UNPAID.ToString(),
                Text = "Đang chờ thanh toán."
            });

            list.Add(new SelectListItem
            {
                Value = Constants_Invoice.INVOICE_PROCESSING.ToString(),
                Text = "Hoá đơn đang xử lí"
            });

            list.Add(new SelectListItem
            {
                Value = Constants_Invoice.INVOICE_PAID.ToString(),
                Text = "Thanh toán thành công"
            });

            list.Add(new SelectListItem
            {
                Value = Constants_Invoice.INVOICE_CANCELLED.ToString(),
                Text = "Hoá đơn đã huỷ"
            });

            list.Add(new SelectListItem
            {
                Value = Constants_Invoice.INVOICE_FAILED.ToString(),
                Text = "Thanh toán không thành công."
            });

            return list;
        }
        public static List<SelectListItem> GetPatients(IRepository<Patient> patientRepository)
        {
                List<SelectListItem> list = new List<SelectListItem>
                {
                    
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
        public static List<SelectListItem> GetMedicines(IRepository<Medicine> medicineRepository)
        {
            List<SelectListItem> list = new List<SelectListItem>
            {

            };

            var medicines = medicineRepository.GetAllAsync().Result;

            foreach (var item in medicines)
            {
                list.Add(new SelectListItem()
                {
                    Value = item.MedicineId.ToString(),
                    Text = item.MedicineName
                });
            }

            return list;
        }
        public static List<SelectListItem> GetDentists(IRepository<Dentist> dentistRepository)
        {
            List<SelectListItem> list = new List<SelectListItem>
                { 
                };

            var dentists = dentistRepository.GetAllAsync().Result;

            foreach (var item in dentists)
            {
                list.Add(new SelectListItem()
                {
                    Value = item.DentistId.ToString(),
                    Text = item.DentistName
                });
            }

            return list;
        }
        public static List<SelectListItem> GetServices(IRepository<Service> serviceRepository)
        {
            List<SelectListItem> list = new List<SelectListItem>
                {
                   
                };

            var services = serviceRepository.GetAllAsync().Result;

            foreach (var item in services)
            {
                list.Add(new SelectListItem()
                {
                    Value = item.ServiceId.ToString(),
                    Text = item.ServiceName
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
        }
            };

            var employees = repositoryEmployee.GetAllAsync().Result;

            foreach (var item in employees)
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
