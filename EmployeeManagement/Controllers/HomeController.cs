﻿using EmployeeManagement.Models;
using EmployeeManagement.Security;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger logger;
        private readonly IAuthorizationService authorizationService;
        private readonly IDataProtector protector;

        public HomeController(IEmployeeRepository employeeRepository,
                              IHostingEnvironment hostingEnvironment,
                              ILogger<HomeController> logger,
                              IDataProtectionProvider dataProtectionProvider,
                              DataProtectionPurposeStrings dataProtectionPurposeStrings,
                              IAuthorizationService authorizationService)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
            this.authorizationService = authorizationService;
            protector = dataProtectionProvider
                .CreateProtector(dataProtectionPurposeStrings.EmployeeIdRouteValue);

        }

        [AllowAnonymous]
        public ViewResult Index()
        {

            var model = _employeeRepository.GetAllEmployee()
                            .Select(e =>
                            {
                                e.EncryptedId = protector.Protect(e.Id.ToString());
                                return e;
                            });
            return View(model);
        }

        [AllowAnonymous]
        public ViewResult Details(string id)
        {
            //throw new Exception("Error in Details View");

            int employeeId;

            if (id.All(char.IsDigit))
            {
                employeeId = int.Parse(id);
            }
            else
            {
                employeeId = Convert.ToInt32(protector.Unprotect(id));
            }


            Employee employee = _employeeRepository.GetEmployee(employeeId);

            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", employeeId);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
                PageTitle = "Employee Details"
            };

            return View(homeDetailsViewModel);
        }

        [HttpGet]
        //[Authorize(Policy = "CreateRolePolicy")]
        public ViewResult Create()
        {
            return View();
        }

        [HttpGet]
        //[Authorize(Policy = "SuperAdminOrAdminRolePolicy")]
        //[Authorize(Policy = "EditRolePolicy")]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(employeeEditViewModel);
        }

        [HttpPost]
        //[Authorize(Policy = "SuperAdminOrAdminRolePolicy")]
        //[Authorize(Policy = "EditRolePolicy")]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if (model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath,
                            "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = ProcessUploadedFile(null, model);

                }

                _employeeRepository.Update(employee);
                return RedirectToAction("index");
            }

            return View();
        }

        private string ProcessUploadedFile(Employee employee = null, EmployeeCreateViewModel model = null)
        {
            string uniqueFileName = null;
            if (model?.Photo != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + (model?.Photo?.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);


                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.Photo.CopyTo(fileStream);
                    }
                return uniqueFileName;
            }
            else
            {
                string fullPath = Path.Combine(hostingEnvironment.WebRootPath, "images", employee.PhotoPath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                return fullPath;

            }

        }

        [HttpPost]
        //[Authorize(Policy = "CreateRolePolicy")]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(null, model);

                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };

                _employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }

            return View();
        }

        [HttpPost]
        //[Authorize(Policy = "SuperAdminOrAdminRolePolicy")]
        //[Authorize(Policy = "DeleteRolePolicy")]
        public IActionResult Delete(string id)
        {
            int employeeId;

            if (id.All(char.IsDigit))
            {
                employeeId = int.Parse(id);
            }
            else
            {
                employeeId = Convert.ToInt32(protector.Unprotect(id));
            }

            Employee employee = _employeeRepository.GetEmployee(employeeId);

            ProcessUploadedFile(employee, null);

            if (employee == null)
            {
                ViewBag.ErrorMessage = $"User cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = _employeeRepository.Delete(employeeId);

                if (result == null)
                {
                    ViewBag.ErrorTitle = $"Something went wrong";
                    ViewBag.ErrorMessage = $"{employee.Name} cannot be deleted";
                    return View("Error");

                }
                return RedirectToAction("index");
            }

        }
    }
}
