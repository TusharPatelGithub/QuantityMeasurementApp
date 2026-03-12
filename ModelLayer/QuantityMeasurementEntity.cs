using System;

namespace ModelLayer.Models;

public class QuantityMeasurementEntity
{
    public string OperationType { get; set; }

    public QuantityDTO? FirstOperand { get; set; }

    public QuantityDTO? SecondOperand { get; set; }

    public QuantityDTO? Result { get; set; }

    public bool HasError { get; set; }

    public string? ErrorMessage { get; set; }

    public QuantityMeasurementEntity(string operationType,
                                     QuantityDTO? firstOperand,
                                     QuantityDTO? secondOperand,
                                     QuantityDTO? result)
    {
        OperationType = operationType;
        FirstOperand = firstOperand;
        SecondOperand = secondOperand;
        Result = result;
        HasError = false;
    }

    public QuantityMeasurementEntity(string operationType,
                                     QuantityDTO? firstOperand,
                                     QuantityDTO? secondOperand,
                                     string errorMessage)
    {
        OperationType = operationType;
        FirstOperand = firstOperand;
        SecondOperand = secondOperand;
        ErrorMessage = errorMessage;
        HasError = true;
    }
}