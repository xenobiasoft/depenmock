﻿namespace DepenMock.NUnit
{
	public abstract class BaseTestByAbstraction<TTestType, TInterfaceType> : BaseTest where TTestType : class, TInterfaceType
	{
		protected TInterfaceType? ResolveSut() => Container?.Resolve<TTestType>();
	}
}
