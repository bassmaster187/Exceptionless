﻿using Exceptionless.Core.Models;
using Foundatio.Utility;
using McSherry.SemanticVersioning;

namespace Exceptionless.Core.Extensions;

public static class StackExtensions {
    public static void MarkFixed(this Stack stack, SemanticVersion version = null) {
        stack.Status = StackStatus.Fixed;
        stack.DateFixed = SystemClock.UtcNow;
        stack.FixedInVersion = version?.ToString();
        stack.SnoozeUntilUtc = null;
    }

    public static void MarkOpen(this Stack stack) {
        stack.Status = StackStatus.Open;
        stack.DateFixed = null;
        stack.FixedInVersion = null;
        stack.SnoozeUntilUtc = null;
    }

    public static Stack ApplyOffset(this Stack stack, TimeSpan offset) {
        if (stack == null)
            return null;

        if (stack.DateFixed.HasValue)
            stack.DateFixed = stack.DateFixed.Value.Add(offset);

        if (stack.FirstOccurrence != DateTime.MinValue)
            stack.FirstOccurrence = stack.FirstOccurrence.Add(offset);

        if (stack.LastOccurrence != DateTime.MinValue)
            stack.LastOccurrence = stack.LastOccurrence.Add(offset);

        return stack;
    }

    public static string GetTypeName(this Stack stack) {
        if (stack.SignatureInfo.TryGetValue("ExceptionType", out string type) && !String.IsNullOrEmpty(type))
            return type.TypeName();

        return type;
    }

    public static bool IsFixed(this Stack stack) {
        if (stack == null)
            return false;

        return stack.Status == StackStatus.Fixed;
    }

    public static bool Is404(this Stack stack) {
        if (stack?.SignatureInfo == null)
            return false;

        return stack.SignatureInfo.ContainsKey("HttpMethod") && stack.SignatureInfo.ContainsKey("Path");
    }
}
