using Microsoft.EntityFrameworkCore;

namespace Identity.Domain.Models;

[Owned]
public class Address
{
    public required string StreetName { get; set; }
    public required int HouseNumber { get; set; }
    public string? HouseNumberSuffix { get; set; }
    public required string PostalCode { get; set; }
    public required string CountryCode { get; set; }
}