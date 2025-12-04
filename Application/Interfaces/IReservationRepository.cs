using Domain.Entities;

namespace Application.Interfaces;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(string reservationId, CancellationToken cancellationToken = default);
    Task<List<Reservation>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<string> AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default);
}