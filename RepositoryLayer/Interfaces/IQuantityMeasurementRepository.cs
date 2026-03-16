using ModelLayer.Entities;
using System.Collections.Generic;

namespace RepositoryLayer.Interfaces
{
    public interface IQuantityMeasurementRepository
    {
        void SaveMeasurement(QuantityMeasurementEntity entity);
        List<QuantityMeasurementEntity> GetAllMeasurements();
        List<QuantityMeasurementEntity> GetMeasurementsByOperation(string operationType);
        List<QuantityMeasurementEntity> GetMeasurementsByType(string measurementType);
        int GetTotalCount();
        void DeleteAll();
    }
}
