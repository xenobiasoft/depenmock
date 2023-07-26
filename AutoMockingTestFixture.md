<a name='assembly'></a>
# Insp.AutoMockingUnitTestFramework

## Contents

- [AutoMockingTestFixture](#T-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture 'Insp.AutoMockingUnitTestFramework.AutoMockingTestFixture')
  - [#ctor()](#M-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture-#ctor 'Insp.AutoMockingUnitTestFramework.AutoMockingTestFixture.#ctor')
  - [Build\`\`1()](#M-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture-Build``1 'Insp.AutoMockingUnitTestFramework.AutoMockingTestFixture.Build``1')
  - [CreateMany\`\`1(count)](#M-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture-CreateMany``1-System-Nullable{System-Int32}- 'Insp.AutoMockingUnitTestFramework.AutoMockingTestFixture.CreateMany``1(System.Nullable{System.Int32})')
  - [Create\`\`1()](#M-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture-Create``1 'Insp.AutoMockingUnitTestFramework.AutoMockingTestFixture.Create``1')
  - [Register\`\`1(instance)](#M-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture-Register``1-``0- 'Insp.AutoMockingUnitTestFramework.AutoMockingTestFixture.Register``1(``0)')
  - [ResolveMock\`\`1()](#M-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture-ResolveMock``1 'Insp.AutoMockingUnitTestFramework.AutoMockingTestFixture.ResolveMock``1')
  - [Resolve\`\`1()](#M-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture-Resolve``1 'Insp.AutoMockingUnitTestFramework.AutoMockingTestFixture.Resolve``1')

<a name='T-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture'></a>
## AutoMockingTestFixture `type`

##### Namespace

Insp.AutoMockingUnitTestFramework

##### Summary

Provides object creation functionality, as well as acting as a pseudo dependency injection container

<a name='M-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture-#ctor'></a>
### #ctor() `constructor`

##### Summary

Initializes a new instance of [AutoMockingTestFixture](#T-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture 'Insp.AutoMockingUnitTestFramework.AutoMockingTestFixture')

##### Parameters

This constructor has no parameters.

<a name='M-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture-Build``1'></a>
### Build\`\`1() `method`

##### Summary

Customizes the creation process for a single object.

##### Returns

A [ICustomizationComposer\`1](#T-AutoFixture-Dsl-ICustomizationComposer`1 'AutoFixture.Dsl.ICustomizationComposer`1') that can be used to customize the creation
algorithm before creating the object.

##### Parameters

This method has no parameters.

##### Generic Types

| Name | Description |
| ---- | ----------- |
| TType | Any object type |

<a name='M-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture-CreateMany``1-System-Nullable{System-Int32}-'></a>
### CreateMany\`\`1(count) `method`

##### Summary

Creates a list of specified type. This acts as a data generator for
primitive types and strings. For object types, it will generate a list of instances
of the specified type with all properties set with generated data.

##### Returns

A list of instances of the requested type.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| count | [System.Nullable{System.Int32}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Nullable 'System.Nullable{System.Int32}') | The number of instances to create |

##### Generic Types

| Name | Description |
| ---- | ----------- |
| TType | Any primitive type, string, or object |

<a name='M-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture-Create``1'></a>
### Create\`\`1() `method`

##### Summary

Creates a new instance of the specified type. This acts as a data generator for
primitive types and strings. For object types, it will generate a new instance
of the class with all properties set with generated data.

##### Returns

A new instance of the requested type.

##### Parameters

This method has no parameters.

##### Generic Types

| Name | Description |
| ---- | ----------- |
| TType | Any primitive type, string, or object |

<a name='M-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture-Register``1-``0-'></a>
### Register\`\`1(instance) `method`

##### Summary

Registers an instance of TInstanceType in the Fixture. The fixture always returns this same instance when the instance
of the type is requested either directly, or indirectly as a nested value of other types.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| instance | [\`\`0](#T-``0 '``0') | The instance of the type that will be registered with the Fixture |

##### Generic Types

| Name | Description |
| ---- | ----------- |
| TInstanceType | The interface type |

<a name='M-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture-ResolveMock``1'></a>
### ResolveMock\`\`1() `method`

##### Summary

Creates a mock of the requested type and registers it in the Fixture. As a default, all properties and methods
are automatically mocked, but can be overridden using a setup on the returned mocked object.
The fixture always returns the same instance of the mock whenever the instance of the type is requested
either directly, or indirectly as a nested value of other types.

##### Returns

A mock instance of the requested type

##### Parameters

This method has no parameters.

##### Generic Types

| Name | Description |
| ---- | ----------- |
| TType | Any object type |

<a name='M-Insp-AutoMockingUnitTestFramework-AutoMockingTestFixture-Resolve``1'></a>
### Resolve\`\`1() `method`

##### Summary

Creates a new instance of the requested type and registers it in the Fixture.
The fixture always returns the same instance whenever the instance of the type is requested
either directly, or indirectly as a nested value of other types.

##### Returns

A new instance of the requested type.

##### Parameters

This method has no parameters.

##### Generic Types

| Name | Description |
| ---- | ----------- |
| TType | Any object type |
