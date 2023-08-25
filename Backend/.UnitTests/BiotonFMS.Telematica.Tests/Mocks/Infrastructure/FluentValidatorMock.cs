using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace BiotonFMS.Telematica.Tests.Mocks;

public static class FluentValidatorMock
{
    public static IValidator<T> GetStub<T>()
    {
        var mock = new Mock<IValidator<T>>();
        mock.Setup(v => v.Validate(It.IsAny<T>())).Returns(new ValidationResult());
        return mock.Object;
    }
    
}
