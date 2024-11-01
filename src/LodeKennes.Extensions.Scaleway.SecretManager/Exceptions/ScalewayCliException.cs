namespace LodeKennes.Extensions.Scaleway.SecretManager.Exceptions;

public sealed class ScalewayCliException(string message, Exception? exception = null) : Exception(message, exception);