// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CustAttributes.NamedAtributes;

public class KeyAttribute : INamedAttribute
{
    public string Name { get; set; } = "key";
    public string Scope { get; set; } = "property";
    public bool IsUserDefined { get; set; } = false;
    public bool IsValid { get; set; } = true;
    public List<string> GetAttributes(PropertyTemplate property)
    {
        return property.IsKey ? new List<string> { "[Key]" } : new List<string>();
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return new();
    }
}