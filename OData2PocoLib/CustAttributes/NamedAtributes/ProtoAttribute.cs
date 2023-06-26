// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.


namespace OData2Poco.CustAttributes.NamedAtributes;

public class ProtoAttribute : INamedAttribute
{
    public string Name { get; set; } = "proto";
    public string Scope { get; set; } = "dual";
    public bool IsUserDefined { get; set; } = false;
    public bool IsValid { get; set; } = true;
    public List<string> GetAttributes(PropertyTemplate property)
    {
        return new() { $"[ProtoMember({property.Serial})]" };
    }

    public List<string> GetAttributes(ClassTemplate classTemplate)
    {
        return new() { "[ProtoContract]" };
    }
}