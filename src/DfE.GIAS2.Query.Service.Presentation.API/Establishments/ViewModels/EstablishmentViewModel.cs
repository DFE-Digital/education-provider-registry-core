namespace DfE.GIAS2.Query.Service.Presentation.API.Establishments.ViewModels;

/// <summary>
/// Represents the establishment details returned by the API.
/// This view model is designed for client consumption and contains only
/// presentation‑ready fields.
/// </summary>
public sealed record EstablishmentViewModel
{
    /// <summary>
    /// Gets the unique numeric identifier (URN) assigned to the establishment.
    /// </summary>
    public required string URN { get; init; }
}