﻿// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.V4;

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;

/// <summary>
///     Read Vocabulary
/// </summary>
internal static class VocabularyHelpersV4
{
    [Serializable]
    [Flags]
    public enum CorePermission
    {
        None = 0,
        Read = 1,
        Write = 2,
        ReadWrite = 3
    }

    public static CorePermission? GetPermissions(this IEdmModel model, IEdmProperty property)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        if (property == null) throw new ArgumentNullException(nameof(property));

        return model.GetEnum<CorePermission>(property, CoreVocabularyModel.PermissionsTerm);
    }

    public static bool IsReadOnly(this IEdmModel model, IEdmProperty property)
    {
        var result = model.GetComputed(property)
                     ?? model.GetPermissions(property) == CorePermission.Read;
        return result;
    }

    internal static bool? GetImmutable(this IEdmModel model, IEdmProperty property)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        if (property == null) throw new ArgumentNullException(nameof(property));

        return model.GetBoolean(property, CoreVocabularyModel.ImmutableTerm);
    }

    internal static bool? GetComputed(this IEdmModel model, IEdmProperty property)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        if (property == null) throw new ArgumentNullException(nameof(property));

        return model.GetBoolean(property, CoreVocabularyModel.ComputedTerm);
    }

    private static IEdmVocabularyAnnotation? FindVocabularyAnnotation(
        this IEdmModel model,
        IEdmVocabularyAnnotatable target,
        IEdmTerm term)
    {
        var annotations = model.FindVocabularyAnnotations(target);
        if (annotations == null) return null;
        var annotation =
            annotations.FirstOrDefault(a => a.Term.Namespace == term.Namespace && a.Term.Name == term.Name);
        var result = annotation;

        return result;
    }

    private static T? GetEnum<T>(this IEdmModel model, IEdmProperty property, IEdmTerm term)
        where T : struct
    {
        var annotation = model.FindVocabularyAnnotation(property, term);
        if (annotation == null) return null;
        var enumMemberReference = (IEdmEnumMemberExpression)annotation.Value;
        var enumMember = enumMemberReference.EnumMembers.Single();
        return (T)Enum.Parse(typeof(T), enumMember.Name);
    }

    private static bool? GetBoolean(this IEdmModel model, IEdmProperty property, IEdmTerm term)
    {
        var annotation = model.FindVocabularyAnnotation(property, term);
        var booleanExpression = annotation?.Value as IEdmBooleanConstantExpression;
        return booleanExpression?.Value;
    }
}
