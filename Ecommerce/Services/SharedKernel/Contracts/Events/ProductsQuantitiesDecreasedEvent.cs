using SharedKernel.Contracts.Events.DTOs;

namespace SharedKernel.Contracts.Events;

public record ProductsQuantitiesDecreasedEvent(IReadOnlyList<ProductQuantityDTO> Items);
