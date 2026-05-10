using SharedKernel.Contracts.Events.DTOs;

namespace SharedKernel.Contracts.Events;

public record ProductsQuantitiesIncreasedEvent(IReadOnlyList<ProductQuantityDTO> Items);
