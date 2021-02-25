using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PDR.PatientBooking.Data.Models
{
    public class Doctor
    {
        public Doctor()
        {
            Created = DateTime.UtcNow;
            Orders = new List<Order>();
        }
        
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Gender { get; set; }
        public string Email { get; set; }
        public virtual ICollection<Order> Orders { get; }
        public DateTime Created { get; }
    }
}
