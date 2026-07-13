namespace Tripflow.Domain.Enums;

public enum TenantStatus
{
    Trial = 1,
    Active = 2,
    Inactive = 3,
    Suspended = 4,
    Cancelled = 5
}

public enum UserStatus
{
    Invited = 1,
    Active = 2,
    Blocked = 3,
    Removed = 4
}

public enum CustomerType
{
    Person = 1,
    Company = 2
}

public enum CustomerStatus
{
    Active = 1,
    Inactive = 2,
    Blocked = 3
}

public enum QuoteType
{
    FlightOnly = 1,
    CompleteItinerary = 2,
    Package = 3,
    Miles = 4,
    Custom = 5
}

public enum QuoteStatus
{
    Draft = 1,
    Calculated = 2,
    Sent = 3,
    Viewed = 4,
    Approved = 5,
    Rejected = 6,
    Expired = 7,
    PaymentPending = 8,
    Paid = 9,
    Issued = 10,
    Cancelled = 11
}

public enum QuoteItemType
{
    Flight = 1,
    Hotel = 2,
    Insurance = 3,
    Tour = 4,
    CarRental = 5,
    ServiceFee = 6,
    Other = 99
}

public enum ProposalStatus
{
    Draft = 1,
    Generated = 2,
    Sent = 3,
    Viewed = 4,
    Approved = 5,
    Rejected = 6,
    Expired = 7,
    Cancelled = 8
}

public enum ProposalEventType
{
    Generated = 1,
    Sent = 2,
    Viewed = 3,
    Approved = 4,
    Rejected = 5,
    Expired = 6,
    PaymentLinkCreated = 7
}

public enum PaymentMethod
{
    Manual = 1,
    Pix = 2,
    CreditCard = 3,
    BankSlip = 4
}

public enum PaymentStatus
{
    Pending = 1,
    Authorized = 2,
    Paid = 3,
    Failed = 4,
    Cancelled = 5,
    Refunded = 6,
    Chargeback = 7,
    Expired = 8
}

public enum PaymentProviderStatus
{
    Active = 1,
    Inactive = 2
}

public enum PaymentLinkStatus
{
    Pending = 1,
    Active = 2,
    Paid = 3,
    Expired = 4,
    Cancelled = 5
}

public enum FinancialTransactionType
{
    Sale = 1,
    PaymentFee = 2,
    Refund = 3,
    Chargeback = 4,
    Adjustment = 5
}

public enum LoyaltyProgramStatus
{
    Active = 1,
    Inactive = 2
}

public enum LoyaltyAccountStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3
}

public enum MilesTransactionType
{
    Credit = 1,
    Debit = 2,
    Adjustment = 3,
    Expiration = 4
}

public enum MilesExpirationStatus
{
    Pending = 1,
    Used = 2,
    Expired = 3,
    Cancelled = 4
}

public enum SubscriptionPlanStatus
{
    Active = 1,
    Inactive = 2,
    Deprecated = 3
}

public enum TenantSubscriptionStatus
{
    Trial = 1,
    Active = 2,
    PastDue = 3,
    Cancelled = 4,
    Suspended = 5
}