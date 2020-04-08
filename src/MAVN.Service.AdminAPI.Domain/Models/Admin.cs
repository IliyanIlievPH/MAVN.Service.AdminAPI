using System;
using System.Collections.Generic;
using MAVN.Service.AdminAPI.Domain.Enums;

namespace MAVN.Service.AdminAPI.Domain.Models
{
    public class Admin
    {
        public string Id { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Registered { get; set; }
        public string PhoneNumber { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public List<Permission> Permissions { set; get; }
    }
}
