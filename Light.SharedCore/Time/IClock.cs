﻿using System;

namespace Light.SharedCore.Time;

/// <summary>
/// Represents the abstraction of a clock that retrieves the current time.
/// </summary>
public interface IClock
{
    /// <summary>
    /// Gets the current time.
    /// </summary>
    DateTime GetTime();
}
