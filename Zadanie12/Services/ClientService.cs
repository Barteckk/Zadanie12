using Microsoft.EntityFrameworkCore;

namespace Zadanie12.Services;

public class ClientService : IClientService
{
    private readonly MasterContext _context;

    public ClientService(MasterContext context)
    {
        _context = context;
    }

    public async Task<bool> DeleteClientAsync(int idClient)
    {
        var client = await _context.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null)
            throw new InvalidOperationException("Client not found");

        if (client.ClientTrips.Any())
            throw new InvalidOperationException("Client cannot be deleted because they are assigned to trips");

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return true;
    }
}