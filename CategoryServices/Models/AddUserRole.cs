﻿namespace CategoryServices.Models
{
    public class AddUserRole
    {
        public int UserId { get; set; }
        public List<int> RoleIds { get; set; }
    }
}
