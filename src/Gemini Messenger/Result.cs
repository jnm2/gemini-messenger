namespace Gemini_Messenger;

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

public static class Result
{
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static ErrorResult Error(string message) => new(message);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly struct ErrorResult
    {
        public ErrorResult(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Error message must be specified.", nameof(message));

            ErrorMessage = message;
        }

        public string ErrorMessage { get; }
    }
}

public readonly struct Result<T>
{
    private readonly T? value;
    private readonly string? errorMessage;

    private Result(T? value, string? errorMessage)
    {
        this.value = value;
        this.errorMessage = errorMessage;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, errorMessage: null);
    }

    public static Result<T> Error(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Error message must be specified.", nameof(message));

        return new Result<T>(value: default, message);
    }

    public bool IsSuccess([MaybeNullWhen(false)] out T value)
    {
        value = this.value;
        return errorMessage is null;
    }

    public bool IsError([NotNullWhen(true)] out string? message)
    {
        message = errorMessage;
        return errorMessage != null;
    }

    public T Value
    {
        get
        {
            if (errorMessage != null) throw new InvalidOperationException("The result is not success.");
            return value!;
        }
    }

    public static implicit operator Result<T>(Result.ErrorResult errorResult) => Error(errorResult.ErrorMessage);
}
