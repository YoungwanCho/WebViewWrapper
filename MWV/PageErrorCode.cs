namespace MWV
{
    public enum PageErrorCode
    {
        ERROR_TOO_MANY_REQUESTS = -15,
        ERROR_FILE_NOT_FOUND = -14,
        ERROR_FILE = -13,
        ERROR_BAD_URL = -12,
        ERROR_FAILED_SSL_HANDSHAKE = -11,
        ERROR_UNSUPPORTED_SCHEME = -10,
        ERROR_REDIRECT_LOOP = -9,
        ERROR_TIMEOUT = -8,
        ERROR_IO = -7,
        ERROR_CONNECT = -6,
        ERROR_PROXY_AUTHENTICATION = -5,
        ERROR_AUTHENTICATION = -4,
        ERROR_UNSUPPORTED_AUTH_SCHEME = -3,
        ERROR_HOST_LOOKUP = -2,
        ERROR_UNKNOWN = -1
    }
}