// UC10: Global type aliases for backward compatibility
// QuantityLength and QuantityWeight are now type aliases for the generic Quantity<U> class.
// This ensures all existing code (UC1–UC9) works without modification.

global using ModelLayer.Models;

global using QuantityLength = ModelLayer.Models.Quantity<ModelLayer.Models.LengthUnit>;
global using QuantityWeight = ModelLayer.Models.Quantity<ModelLayer.Models.WeightUnit>;

// UC11: QuantityVolume alias for volume measurement support
global using QuantityVolume = ModelLayer.Models.Quantity<ModelLayer.Models.VolumeUnit>;

// UC14: QuantityTemperature alias for temperature measurement support
global using QuantityTemperature = ModelLayer.Models.Quantity<ModelLayer.Models.TemperatureUnit>;