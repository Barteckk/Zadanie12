namespace Zadanie12.Services;

public interface IClientService
{
    Task<bool> DeleteClientAsync(int idClient);
}