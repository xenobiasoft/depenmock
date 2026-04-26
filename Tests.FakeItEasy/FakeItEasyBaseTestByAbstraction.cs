using DepenMock.FakeItEasy;
using DepenMock.XUnit.V3;

namespace Tests.FakeItEasy;

public class FakeItEasyBaseTestByAbstraction<TType, TInterfaceType>()
    : BaseTestByAbstraction<TType, TInterfaceType>(new FakeItEasyMockFactory())
    where TType : class, TInterfaceType;
