namespace Tripflow.Application.DTOs.Requests.Travelers;

public sealed record CreateTravelerRequest(
    string FullName,
    DateOnly? BirthDate,
    string? Nationality,
    string? DocumentNumber,
    string? PassportNumber,
    DateOnly? PassportExpirationDate,
    string? Notes);
