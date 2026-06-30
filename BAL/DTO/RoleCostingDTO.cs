using System;

namespace BAL.DTO
{
    public class RoleCostingDTO : BaseDTO
    {
        public int RoleCostingId { get; set; }
        public byte RoleId { get; set; }
        public string RoleName { get; set; }
        public decimal PerHourCost { get; set; }
        public decimal PerDayCost { get; set; }
        public bool IsActive { get; set; }
    }
}
