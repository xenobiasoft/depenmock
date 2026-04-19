using DepenMock.NSubstitute;
using DepenMock.XUnit.V3;

namespace Tests.NSubstitute;

public class NSubstituteBaseTestByAbstraction<TType, TInterfaceType>()
    : BaseTestByAbstraction<TType, TInterfaceType>(new NSubstituteMockFactory())
    where TType : class, TInterfaceType;