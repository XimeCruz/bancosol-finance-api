namespace BancoSol.Finance.Application.Common;

public sealed class NotFoundException(string message) : Exception(message);
public sealed class RequestValidationException(string message) : Exception(message);
public sealed class ExternalServiceException(string message, Exception? inner = null) : Exception(message, inner);
