using System.Collections.Generic;
using HikeCycle.Mvc.Models.db;

namespace HikeCycle.Mvc.Models
{
    public class AccountProfileViewModel
    {
        public User User { get; set; }
        public UserProfile Profile { get; set; }
        public List<Booking> AllBookings { get; set; }
    }
}

