﻿using System.Text.Json;

namespace CourseLibrary.API.Models;

public record ErrorDetails
{
    public int StatusCode { get; init; }
    public string Message { get; init; }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
