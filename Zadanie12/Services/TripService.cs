using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Zadanie12.DTOs;

namespace Zadanie12.Services;

public class TripService:ITripService
{
    private readonly MasterContext _context;
    
    public TripService(MasterContext context)
    {
        _context = context;
    }

    public async Task<TripsResponseDto> GetTripsAsync(int page, int pageSize)
    {
        var totalTrips = await _context.Trips.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalTrips / pageSize);
        var trips = await _context.Trips
            .Include(t => t.ClientTrips)
            .ThenInclude(ct => ct.IdClientNavigation)
            .Include(t => t.IdCountries)
            .OrderByDescending(t => t.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TripDto
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries
                    .Select(c => new CountryDto
                    {
                        Name = c.Name
                    })
                    .ToList(),
                Clients = t.ClientTrips
                    .Select(ct => new ClientDto
                    {
                        FirstName = ct.IdClientNavigation.FirstName,
                        LastName = ct.IdClientNavigation.LastName
                    })
                    .ToList()
            })
            .ToListAsync();

        return new TripsResponseDto
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = totalPages,
            Trips = trips
        };
    }
    
    public async Task AssignClientToTripAsync(int idTrip, AssignClientDto dto)
    {
        var trip = await _context.Trips.FindAsync(idTrip);
        if (trip == null)
            throw new InvalidOperationException("Trip not found");

        if (trip.DateFrom <= DateTime.Now)
            throw new InvalidOperationException("Cannot sign up for a trip that already started");

        var client = await _context.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.Pesel == dto.Pesel);

        if (client != null)
        {
            bool alreadyAssigned = client.ClientTrips.Any(ct => ct.IdTrip == idTrip);
            if (alreadyAssigned)
                throw new InvalidOperationException("Client is already assigned to this trip");
        }
        else
        {
            client = new Client
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Telephone = dto.Telephone,
                Pesel = dto.Pesel
            };
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        var newClientTrip = new ClientTrip
        {
            IdClient = client.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = dto.PaymentDate
        };

        _context.ClientTrips.Add(newClientTrip);
        await _context.SaveChangesAsync();
    }

}