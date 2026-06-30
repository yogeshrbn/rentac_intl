using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace BAL.Enums
{
    public enum ChallanType
    {
        LIFT_DELIVERY = 1,
        RENT_DELIVERY = 2,
        PURCHASE = 3,
        PURCHASE_RETURN = 4,
        SALE = 5,
        SALE_RETURN = 6,
        WORKORDER = 7,
        SITE_JOB = 8,
        MAT_ADJUST = 9,
        HIRE = 10,
        CONTRACT_RETURN = 11
    }

    public enum DocumentType
    {
        [EnumMember(Value = "LiftChallans")]
        LIFT_DELIVERY = 1,
        [EnumMember(Value = "RentChallans")]
        RENT_DELIVERY = 2,
        PURCHASE = 3,
        PURCHASE_RETURN = 4,
        SALE = 5,
        SALE_RETURN = 6,
        [EnumMember(Value = "WorkOrderChallans")]
        WORKORDER = 7,
        SITE_JOB = 8,
        [EnumMember(Value = "MatAdjustChallans")]
        MAT_ADJUST = 9,
        [EnumMember(Value = "HireChallans")]
        HIRE = 10
    }

    public enum ContainerDocType
    {
        [EnumMember(Value = "LiftChallans")]
        LIFT_DELIVERY = 1,
        [EnumMember(Value = "RentChallans")]
        RENT_DELIVERY = 2,
        [EnumMember(Value = "Bills")]
        Bills = 3,
        [EnumMember(Value = "purchase")]
        Purchase = 4,
    }
}
