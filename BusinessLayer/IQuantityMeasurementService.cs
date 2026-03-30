using ModelLayer.DTOs;

namespace BusinessLayer.Services;

public interface IQuantityMeasurementService
{
    // ─── Core Operations (UC17: return QuantityMeasurementDTO) ───────────────
    QuantityMeasurementDTO Compare(QuantityDTO first, QuantityDTO second);
    QuantityMeasurementDTO Convert(QuantityDTO quantity, string targetUnit);
    QuantityMeasurementDTO Add(QuantityDTO first, QuantityDTO second);
    QuantityMeasurementDTO Subtract(QuantityDTO first, QuantityDTO second);
    QuantityMeasurementDTO Divide(QuantityDTO first, QuantityDTO second);

    // ─── UC17: Data Retrieval Methods ────────────────────────────────────────
    List<QuantityMeasurementDTO> GetMeasurementsByType(string measurementType);
    List<QuantityMeasurementDTO> GetMeasurementsByOperation(string operationType);
    int GetOperationCount(string operationType);
    List<QuantityMeasurementDTO> GetErrorMeasurements();
}