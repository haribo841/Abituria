using System;
using System.Collections.Generic;

namespace Abituria.Services;

public sealed class ExerciseScratchpadSession
{
    private readonly Dictionary<ScratchpadKey, string> _entries = [];

    public string GetText(Guid profileId, string exerciseId)
    {
        var key = CreateKey(profileId, exerciseId);
        return _entries.TryGetValue(key, out var text) ? text : string.Empty;
    }

    public void SetText(Guid profileId, string exerciseId, string? text)
    {
        var key = CreateKey(profileId, exerciseId);
        if (string.IsNullOrEmpty(text))
        {
            _entries.Remove(key);
            return;
        }

        _entries[key] = text;
    }

    private static ScratchpadKey CreateKey(Guid profileId, string exerciseId)
    {
        if (profileId == Guid.Empty)
            throw new ArgumentException("Identyfikator profilu nie może być pusty.", nameof(profileId));
        ArgumentException.ThrowIfNullOrWhiteSpace(exerciseId);
        return new ScratchpadKey(profileId, exerciseId);
    }

    private readonly record struct ScratchpadKey(Guid ProfileId, string ExerciseId);
}
