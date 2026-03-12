// UC10: Global type aliases for backward compatibility
// Updated to reference models from ModelLayer

global using ModelLayer.Models;

global using QuantityLength = ModelLayer.Models.Quantity<ModelLayer.Models.LengthUnit>;
global using QuantityWeight = ModelLayer.Models.Quantity<ModelLayer.Models.WeightUnit>;

// UC11: QuantityVolume alias
global using QuantityVolume = ModelLayer.Models.Quantity<ModelLayer.Models.VolumeUnit>;

// UC14: QuantityTemperature alias
global using QuantityTemperature = ModelLayer.Models.Quantity<ModelLayer.Models.TemperatureUnit>;