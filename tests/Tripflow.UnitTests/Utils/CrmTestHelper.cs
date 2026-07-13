using Tripflow.Domain.Entities.CRM;
using Tripflow.Domain.Enums;

namespace Tripflow.UnitTests.Utils;

public static class CustomerTestHelper
{
    public static Customer Create(
        Guid? tenantId = null,
        CustomerType type = CustomerType.Person,
        string fullName = "João da Silva",
        string? documentNumber = "12345678900",
        string? email = "joao@teste.com",
        string? phone = "11999999999",
        DateOnly? birthDate = null,
        string? notes = "Notas do cliente.",
        string createdBy = "system")
    {
        return new Customer(
            tenantId ?? Guid.NewGuid(),
            type,
            fullName,
            documentNumber,
            email,
            phone,
            birthDate ?? new DateOnly(1990, 1, 1),
            notes,
            createdBy);
    }
}

public static class TravelerTestHelper
{
    public static Traveler Create(
        Guid? tenantId = null,
        Guid? customerId = null,
        string fullName = "Maria Souza",
        DateOnly? birthDate = null,
        string? nationality = "Brasileira",
        string? documentNumber = "11122233344",
        string? passportNumber = "BR123456",
        DateOnly? passportExpirationDate = null,
        string? notes = "Notas do viajante.",
        string createdBy = "system")
    {
        return new Traveler(
            tenantId ?? Guid.NewGuid(),
            customerId ?? Guid.NewGuid(),
            fullName,
            birthDate ?? new DateOnly(1995, 5, 5),
            nationality,
            documentNumber,
            passportNumber,
            passportExpirationDate ?? DateOnly.FromDateTime(DateTime.UtcNow).AddYears(5),
            notes,
            createdBy);
    }
}

public static class CustomerPreferenceTestHelper
{
    public static CustomerPreference Create(
        Guid? tenantId = null,
        Guid? customerId = null,
        string createdBy = "system")
    {
        return new CustomerPreference(
            tenantId ?? Guid.NewGuid(),
            customerId ?? Guid.NewGuid(),
            createdBy);
    }

    public static CustomerPreference CreateWithData(
        Guid? tenantId = null,
        Guid? customerId = null,
        string? preferredAirlines = "LATAM, GOL",
        string? preferredHotelCategories = "5 estrelas",
        string? seatPreferences = "Janela",
        string? mealRestrictions = "Vegetariano",
        string? travelPreferences = "Viagens internacionais",
        string? generalNotes = "Cliente VIP",
        string createdBy = "system")
    {
        var entity = new CustomerPreference(
            tenantId ?? Guid.NewGuid(),
            customerId ?? Guid.NewGuid(),
            createdBy);

        entity.Update(
            preferredAirlines,
            preferredHotelCategories,
            seatPreferences,
            mealRestrictions,
            travelPreferences,
            generalNotes,
            createdBy);

        return entity;
    }
}
