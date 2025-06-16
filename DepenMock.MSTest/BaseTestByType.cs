using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DepenMock.MSTest
{
    
    public abstract class BaseTestByType<TTestType> : BaseTest where TTestType : class
    {
        protected TTestType ResolveSut() => Container?.Resolve<TTestType>();
    }
}