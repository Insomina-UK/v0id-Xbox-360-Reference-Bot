using System.Text.Json;
using XboxHresultBot.Entries;

namespace XboxHresultBot.Database;

public sealed class HresultDatabase
{
    private readonly string _path;
    private readonly List<HresultEntry> _entries = new();

    public HresultDatabase(string path)
    {
        _path = path;
        Load();
    }

    public IReadOnlyList<HresultEntry> All => _entries;

    public HresultEntry? Find(string input)
    {
        string query = Normalize(input);

        return _entries.FirstOrDefault(x =>
            Normalize(x.Code) == query ||
            x.Name.Equals(input, StringComparison.OrdinalIgnoreCase));
    }

    public List<HresultEntry> Search(string query)
    {
        query = query.Trim();

        return _entries
            .Where(x =>
                x.Code.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Category.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.Meaning.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.CommonCause.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                x.FixSuggestion.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Take(25)
            .ToList();
    }

    private void Load()
    {
        if (!File.Exists(_path))
        {
            _entries.AddRange(DefaultEntries());
            Save();
            return;
        }

        string json = File.ReadAllText(_path);
        var loaded = JsonSerializer.Deserialize<List<HresultEntry>>(json);

        if (loaded != null)
            _entries.AddRange(loaded);
    }

    private void Save()
    {
        File.WriteAllText(_path, JsonSerializer.Serialize(_entries, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }

    private static string Normalize(string code)
    {
        return code
            .Trim()
            .Replace("0x", "", StringComparison.OrdinalIgnoreCase)
            .Replace("HRESULT:", "", StringComparison.OrdinalIgnoreCase)
            .Replace(" ", "")
            .ToUpperInvariant()
            .PadLeft(8, '0');
    }

    private static List<HresultEntry> DefaultEntries()
    {
        return new List<HresultEntry>
        {
            new()
        {
            Code = "0x0",
            Name = "S_OK",
            Category = "Success",
            Meaning = "Ok",
            CommonCause = "Operation completed successfully or returned an informational status.",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x1",
            Name = "S_FALSE",
            Category = "Success",
            Meaning = "False",
            CommonCause = "Operation completed successfully or returned an informational status.",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80004005",
            Name = "E_FAIL",
            Category = "General Error",
            Meaning = "Fail",
            CommonCause = "Generic application, service, or system failure.",
            FixSuggestion = "Restart the title/console, reproduce the issue, and check logs for a more specific inner error."
        },

        new()
        {
            Code = "0x80070057",
            Name = "E_INVALIDARG",
            Category = "Validation",
            Meaning = "Invalidarg",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80004004",
            Name = "E_ABORT",
            Category = "General Error",
            Meaning = "Abort",
            CommonCause = "Generic application, service, or system failure.",
            FixSuggestion = "Restart the title/console, reproduce the issue, and check logs for a more specific inner error."
        },

        new()
        {
            Code = "0x8007000D",
            Name = "E_INVALID_DATA",
            Category = "Validation",
            Meaning = "Invalid Data",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8007000E",
            Name = "E_OUTOFMEMORY",
            Category = "Memory",
            Meaning = "Outofmemory",
            CommonCause = "Insufficient memory or allocation failure.",
            FixSuggestion = "Restart the application/console and reduce memory usage before retrying."
        },

        new()
        {
            Code = "0x80070005",
            Name = "E_ACCESSDENIED",
            Category = "Security",
            Meaning = "Accessdenied",
            CommonCause = "Access denied, blocked request, enforcement state, certificate, or trust validation issue.",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x80070524",
            Name = "E_USER_EXISTS",
            Category = "Xbox Live",
            Meaning = "User Exists",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80070525",
            Name = "E_NO_SUCH_USER",
            Category = "Xbox Live",
            Meaning = "No Such User",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8000FFFF",
            Name = "E_UNEXPECTED",
            Category = "General Error",
            Meaning = "Unexpected",
            CommonCause = "Generic application, service, or system failure.",
            FixSuggestion = "Restart the title/console, reproduce the issue, and check logs for a more specific inner error."
        },

        new()
        {
            Code = "0x80150001",
            Name = "XONLINE_E_OVERFLOW",
            Category = "Xbox Live",
            Meaning = "Overflow",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150002",
            Name = "XONLINE_E_NO_SESSION",
            Category = "Xbox Live",
            Meaning = "No Session",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150003",
            Name = "XONLINE_E_USER_NOT_LOGGED_ON",
            Category = "Xbox Live",
            Meaning = "User Not Logged On",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150004",
            Name = "XONLINE_E_NO_GUEST_ACCESS",
            Category = "Security",
            Meaning = "No Guest Access",
            CommonCause = "Access denied, blocked request, enforcement state, certificate, or trust validation issue.",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x80150005",
            Name = "XONLINE_E_NOT_INITIALIZED",
            Category = "Xbox Live",
            Meaning = "Not Initialized",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150006",
            Name = "XONLINE_E_NO_USER",
            Category = "Xbox Live",
            Meaning = "No User",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150007",
            Name = "XONLINE_E_INTERNAL_ERROR",
            Category = "Xbox Live",
            Meaning = "Internal Error",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150008",
            Name = "XONLINE_E_OUT_OF_MEMORY",
            Category = "Memory",
            Meaning = "Out Of Memory",
            CommonCause = "Insufficient memory or allocation failure.",
            FixSuggestion = "Restart the application/console and reduce memory usage before retrying."
        },

        new()
        {
            Code = "0x80150009",
            Name = "XONLINE_E_TASK_BUSY",
            Category = "Xbox Live",
            Meaning = "Task Busy",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015000A",
            Name = "XONLINE_E_SERVER_ERROR",
            Category = "Xbox Live",
            Meaning = "Server Error",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015000B",
            Name = "XONLINE_E_IO_ERROR",
            Category = "Storage",
            Meaning = "Io Error",
            CommonCause = "Memory unit, profile storage, mounted device, or storage I/O issue.",
            FixSuggestion = "Reconnect storage, clear cache, redownload profile/content, and test another storage device."
        },

        new()
        {
            Code = "0x8015000C",
            Name = "XONLINE_E_BAD_CONTENT_TYPE",
            Category = "Marketplace",
            Meaning = "Bad Content Type",
            CommonCause = "Offer, license, entitlement, content availability, or region restriction issue.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015000D",
            Name = "XONLINE_E_USER_NOT_PRESENT",
            Category = "Xbox Live",
            Meaning = "User Not Present",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015000E",
            Name = "XONLINE_E_PROTOCOL_MISMATCH",
            Category = "Xbox Live",
            Meaning = "Protocol Mismatch",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015000F",
            Name = "XONLINE_E_INVALID_SERVICE_ID",
            Category = "Validation",
            Meaning = "Invalid Servicid",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80150010",
            Name = "XONLINE_E_INVALID_REQUEST",
            Category = "Validation",
            Meaning = "Invalid Request",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80150011",
            Name = "XONLINE_E_TASK_THROTTLED",
            Category = "Xbox Live",
            Meaning = "Task Throttled",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150012",
            Name = "XONLINE_E_TASK_ABORTED_BY_DUPLICATE",
            Category = "Xbox Live",
            Meaning = "Task Aborted By Duplicate",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150013",
            Name = "XONLINE_E_INVALID_TITLE_ID",
            Category = "Validation",
            Meaning = "Invalid Titlid",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80150014",
            Name = "XONLINE_E_SERVER_CONFIG_ERROR",
            Category = "Xbox Live",
            Meaning = "Server Config Error",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150015",
            Name = "XONLINE_E_END_OF_STREAM",
            Category = "Xbox Live",
            Meaning = "End Of Stream",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150016",
            Name = "XONLINE_E_ACCESS_DENIED",
            Category = "Security",
            Meaning = "Accesdenied",
            CommonCause = "Access denied, blocked request, enforcement state, certificate, or trust validation issue.",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x80150017",
            Name = "XONLINE_E_GEO_DENIED",
            Category = "Security",
            Meaning = "Geo Denied",
            CommonCause = "Access denied, blocked request, enforcement state, certificate, or trust validation issue.",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x80150018",
            Name = "XONLINE_E_UNSUPPORTED_METHOD",
            Category = "Network",
            Meaning = "XRL has wiredata request for an old/deprecated API-call received from new flash/client (or viceversa)",
            CommonCause = "XRL has wiredata request for an old/deprecated API-call received from new flash/client (or viceversa)",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80150019",
            Name = "XONLINE_E_RESOURCE_UNAVAILABLE",
            Category = "Xbox Live",
            Meaning = "FastFail blocked the call",
            CommonCause = "FastFail blocked the call",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150020",
            Name = "XONLINE_E_AUTHDATA_MISMATCH",
            Category = "Authentication",
            Meaning = "values in authdata don’t match those in request",
            CommonCause = "values in authdata don’t match those in request",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80150021",
            Name = "XONLINE_E_ACCESS_TOKEN_ERROR",
            Category = "Authentication",
            Meaning = "generic AccessToken error",
            CommonCause = "generic AccessToken error",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80150022",
            Name = "XONLINE_E_HEALTH_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch all) health error",
            CommonCause = "non-specific (catch all) health error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150023",
            Name = "XONLINE_E_RESPONSE_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch all) response error",
            CommonCause = "non-specific (catch all) response error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150024",
            Name = "XONLINE_E_ACTIVE_AUTH_ERROR",
            Category = "Authentication",
            Meaning = "non-specific (catch all) active auth error",
            CommonCause = "non-specific (catch all) active auth error",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80150025",
            Name = "XONLINE_E_MULTIPLE_USERS",
            Category = "Xbox Live",
            Meaning = "received multiple users when only one is expected",
            CommonCause = "received multiple users when only one is expected",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150026",
            Name = "XONLINE_E_CALL_SOURCE_INVALID",
            Category = "Validation",
            Meaning = "Call Sourcinvalid",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80150027",
            Name = "XONLINE_E_DECRYPTION_ERROR",
            Category = "Xbox Live",
            Meaning = "error decrypting incoming request",
            CommonCause = "error decrypting incoming request",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150028",
            Name = "XONLINE_E_DESERIALIZATION_ERROR",
            Category = "Xbox Live",
            Meaning = "error deserializing incoming request",
            CommonCause = "error deserializing incoming request",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150029",
            Name = "XONLINE_E_SERVICE_KEY_ERROR",
            Category = "Xbox Live",
            Meaning = "error accessing service key (generic, look for inner exception, hr)",
            CommonCause = "error accessing service key (generic, look for inner exception, hr)",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150030",
            Name = "XONLINE_E_MASTER_KEY_ERROR",
            Category = "Xbox Live",
            Meaning = "error accessing master key (generic, look for inner exception, hr)",
            CommonCause = "error accessing master key (generic, look for inner exception, hr)",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80150100",
            Name = "XONLINE_E_DATABASE_ERROR",
            Category = "Database",
            Meaning = "unspecified database error",
            CommonCause = "unspecified database error",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x80150101",
            Name = "XONLINE_E_DATABASE_EXECUTE_ERROR",
            Category = "Database",
            Meaning = "database query failed, typically a sql exception",
            CommonCause = "database query failed, typically a sql exception",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x80150102",
            Name = "XONLINE_E_DATABASE_RESULT_ERROR",
            Category = "Database",
            Meaning = "too many, too few results, or unexpected return value",
            CommonCause = "too many, too few results, or unexpected return value",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x80150103",
            Name = "XONLINE_E_DATABASE_TRANSACTION_ERROR",
            Category = "Database",
            Meaning = "transaction operation has no transaction",
            CommonCause = "transaction operation has no transaction",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x80150104",
            Name = "XONLINE_E_DATABASE_PARAMETER_NOT_FOUND",
            Category = "Database",
            Meaning = "parameter name not found in GetXxxParameter",
            CommonCause = "parameter name not found in GetXxxParameter",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x80150105",
            Name = "XONLINE_E_DATABASE_DEADLOCK",
            Category = "Database",
            Meaning = "sql detected a deadlock and terminated call",
            CommonCause = "sql detected a deadlock and terminated call",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x80150106",
            Name = "XONLINE_E_DATABASE_PARAMETER_INVALID",
            Category = "Database",
            Meaning = "failed sproc specific parameter validation",
            CommonCause = "failed sproc specific parameter validation",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x80150107",
            Name = "XONLINE_E_DATABASE_FOREIGN_KEY_VIOLATION",
            Category = "Database",
            Meaning = "a SQL operation would have resulted in a foreign key exception",
            CommonCause = "a SQL operation would have resulted in a foreign key exception",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x80150108",
            Name = "XONLINE_E_DATABASE_DUPLICATE_KEY",
            Category = "Database",
            Meaning = "duplicate primary or unique key",
            CommonCause = "duplicate primary or unique key",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x80150109",
            Name = "XONLINE_E_DATABASE_TIMEOUT",
            Category = "Database",
            Meaning = "what it says, the query did not response within a specified time",
            CommonCause = "what it says, the query did not response within a specified time",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x80151000",
            Name = "XONLINE_E_LOGON_NO_NETWORK_CONNECTION",
            Category = "Authentication",
            Meaning = "Logon No Network Connection",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x001510F0",
            Name = "XONLINE_S_LOGON_CONNECTION_ESTABLISHED",
            Category = "Success",
            Meaning = "Logon Connection Established",
            CommonCause = "Operation completed successfully or returned an informational status.",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x001510F1",
            Name = "XONLINE_S_LOGON_DISCONNECTED",
            Category = "Success",
            Meaning = "Logon Disconnected",
            CommonCause = "Operation completed successfully or returned an informational status.",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80151001",
            Name = "XONLINE_E_LOGON_CANNOT_ACCESS_SERVICE",
            Category = "Authentication",
            Meaning = "Logon Cannot Accesservice",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151002",
            Name = "XONLINE_E_LOGON_UPDATE_REQUIRED",
            Category = "Authentication",
            Meaning = "Logon Updatrequired",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151003",
            Name = "XONLINE_E_LOGON_SERVERS_TOO_BUSY",
            Category = "Authentication",
            Meaning = "Logon Servertoo Busy",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151004",
            Name = "XONLINE_E_LOGON_CONNECTION_LOST",
            Category = "Authentication",
            Meaning = "Logon Connection Lost",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151005",
            Name = "XONLINE_E_LOGON_KICKED_BY_DUPLICATE_LOGON",
            Category = "Authentication",
            Meaning = "Logon Kicked By Duplicatlogon",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151006",
            Name = "XONLINE_E_LOGON_INVALID_USER",
            Category = "Authentication",
            Meaning = "Logon Invalid User",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151007",
            Name = "XONLINE_E_LOGON_FLASH_UPDATE_REQUIRED",
            Category = "Authentication",
            Meaning = "Logon Flash Updatrequired",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151008",
            Name = "XONLINE_E_LOGON_TITLE_ACTIVATION_REQUIRED",
            Category = "Authentication",
            Meaning = "Logon Titlactivation Required",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151009",
            Name = "XONLINE_E_LOGON_USER_TITLE_ACTIVATION_REQUIRED",
            Category = "Authentication",
            Meaning = "Logon User Titlactivation Required",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015100A",
            Name = "XONLINE_E_LOGON_OTHER_TITLE_ACTIVATED",
            Category = "Authentication",
            Meaning = "Logon Other Titlactivated",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015100B",
            Name = "XONLINE_E_LOGON_SG_CONNECTION_TERMINATED",
            Category = "Authentication",
            Meaning = "Logon Sg Connection Terminated",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015100C",
            Name = "XONLINE_E_LOGON_SG_CONNECTION_TIMEDOUT",
            Category = "Authentication",
            Meaning = "Logon Sg Connection Timedout",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015100D",
            Name = "XONLINE_E_LOGON_SG_CONNECTION_RESET",
            Category = "Authentication",
            Meaning = "Logon Sg Connection Reset",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015100E",
            Name = "XONLINE_E_LOGON_SG_CONNECTION_FAILED",
            Category = "Authentication",
            Meaning = "Logon Sg Connection Failed",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015100F",
            Name = "XONLINE_E_LOGON_USER_NOT_TRUSTED",
            Category = "Authentication",
            Meaning = "Logon User Not Trusted",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151010",
            Name = "XONLINE_E_LOGON_USER_RPS_EXPIRED",
            Category = "Authentication",
            Meaning = "Logon User Rpexpired",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151011",
            Name = "XONLINE_E_LOGON_PPLOGIN_PASSWORD_PROBLEM",
            Category = "Authentication",
            Meaning = "Logon Pplogin Password Problem",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151012",
            Name = "XONLINE_E_LOGON_PPLOGIN_VERIFICATION_REQUIRED",
            Category = "Authentication",
            Meaning = "Logon Pplogin Verification Required",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151013",
            Name = "XONLINE_E_LOGON_USER_UNKNOWN_TRUST",
            Category = "Authentication",
            Meaning = "Logon User Unknown Trust",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151014",
            Name = "XONLINE_E_LOGON_PPLOGIN_MISMATCH",
            Category = "Authentication",
            Meaning = "Logon Pplogin Mismatch",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151015",
            Name = "XONLINE_E_LOGON_SETTINGS_SYNC_FAILED",
            Category = "Authentication",
            Meaning = "Logon Settingsync Failed",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151016",
            Name = "XONLINE_E_LOGON_SETTINGS_SYNC_CONFLICT",
            Category = "Authentication",
            Meaning = "Logon Settingsync Conflict",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151017",
            Name = "XONLINE_E_LOGON_BLOCKED_BY_CURFEW",
            Category = "Authentication",
            Meaning = "Logon Blocked By Curfew",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151080",
            Name = "XONLINE_E_SILENT_LOGON_DISABLED",
            Category = "Authentication",
            Meaning = "Silent Logon Disabled",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151081",
            Name = "XONLINE_E_SILENT_LOGON_NO_ACCOUNTS",
            Category = "Authentication",
            Meaning = "Silent Logon No Accounts",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151082",
            Name = "XONLINE_E_SILENT_LOGON_PASSCODE_REQUIRED",
            Category = "Authentication",
            Meaning = "Silent Logon Passcodrequired",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151100",
            Name = "XONLINE_E_LOGON_SERVICE_NOT_REQUESTED",
            Category = "Authentication",
            Meaning = "Logon Servicnot Requested",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151101",
            Name = "XONLINE_E_LOGON_SERVICE_NOT_AUTHORIZED",
            Category = "Authentication",
            Meaning = "Logon Servicnot Authorized",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151102",
            Name = "XONLINE_E_LOGON_SERVICE_TEMPORARILY_UNAVAILABLE",
            Category = "Authentication",
            Meaning = "Logon Servictemporarily Unavailable",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151103",
            Name = "XONLINE_E_LOGON_PPLOGIN_FAILED",
            Category = "Authentication",
            Meaning = "Logon Pplogin Failed",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151104",
            Name = "XONLINE_E_LOGON_SPONSOR_TOKEN_INVALID",
            Category = "Authentication",
            Meaning = "Logon Sponsor Token Invalid",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151105",
            Name = "XONLINE_E_LOGON_SPONSOR_TOKEN_BANNED",
            Category = "Authentication",
            Meaning = "Logon Sponsor Token Banned",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151106",
            Name = "XONLINE_E_LOGON_SPONSOR_TOKEN_USAGE_EXCEEDED",
            Category = "Authentication",
            Meaning = "Logon Sponsor Token Usagexceeded",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151107",
            Name = "XONLINE_E_LOGON_FLASH_UPDATE_NOT_DOWNLOADED",
            Category = "Authentication",
            Meaning = "Logon Flash Updatnot Downloaded",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151108",
            Name = "XONLINE_E_LOGON_UPDATE_NOT_DOWNLOADED",
            Category = "Authentication",
            Meaning = "Logon Updatnot Downloaded",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x001512F0",
            Name = "XONLINE_S_LOGON_USER_HAS_MESSAGE",
            Category = "Success",
            Meaning = "Logon User Hamessage",
            CommonCause = "Operation completed successfully or returned an informational status.",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x001512F1",
            Name = "XONLINE_S_LOGON_USER_MESSAGE_ENUMERATION_NEEDED",
            Category = "Success",
            Meaning = "Logon User Messagenumeration Needed",
            CommonCause = "Operation completed successfully or returned an informational status.",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80151200",
            Name = "XONLINE_E_LOGON_USER_ACCOUNT_REQUIRES_MANAGEMENT",
            Category = "Authentication",
            Meaning = "Logon User Account Requiremanagement",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x001513F0",
            Name = "XONLINE_S_LOGON_COMMIT_USER_CHANGE",
            Category = "Success",
            Meaning = "Logon Commit User Change",
            CommonCause = "Operation completed successfully or returned an informational status.",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x001513F1",
            Name = "XONLINE_S_LOGON_USER_CHANGE_COMPLETE",
            Category = "Success",
            Meaning = "Logon User Changcomplete",
            CommonCause = "Operation completed successfully or returned an informational status.",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80151300",
            Name = "XONLINE_E_LOGON_CHANGE_USER_FAILED",
            Category = "Authentication",
            Meaning = "Logon Changuser Failed",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151800",
            Name = "XONLINE_E_LOGON_MU_NOT_MOUNTED",
            Category = "Authentication",
            Meaning = "Logon Mu Not Mounted",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151801",
            Name = "XONLINE_E_LOGON_MU_IO_ERROR",
            Category = "Authentication",
            Meaning = "Logon Mu Io Error",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151802",
            Name = "XONLINE_E_LOGON_NOT_LOGGED_ON",
            Category = "Authentication",
            Meaning = "Logon Not Logged On",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151901",
            Name = "XONLINE_E_LOGON_NO_IP_ADDRESS",
            Category = "Authentication",
            Meaning = "Logon No Ip Address",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151902",
            Name = "XONLINE_E_LOGON_NO_DNS_SERVICE",
            Category = "Authentication",
            Meaning = "Logon No Dnservice",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151903",
            Name = "XONLINE_E_LOGON_DNS_LOOKUP_FAILED",
            Category = "Authentication",
            Meaning = "No DNS results were returned for the provided domain. This is also the expected result for Xbox Live related lookups when LiveBlock is enabled in Dashlaunch.",
            CommonCause = "No DNS results were returned for the provided domain. This is also the expected result for Xbox Live related lookups when LiveBlock is enabled in Dashlaunch.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151904",
            Name = "XONLINE_E_LOGON_DNS_LOOKUP_TIMEDOUT",
            Category = "Authentication",
            Meaning = "Logon Dnlookup Timedout",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151905",
            Name = "XONLINE_E_LOGON_INVALID_XBOX_ONLINE_INFO",
            Category = "Authentication",
            Meaning = "Logon Invalid Xbox Onlininfo",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151906",
            Name = "XONLINE_E_LOGON_MACS_FAILED",
            Category = "Authentication",
            Meaning = "Logon Macfailed",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151907",
            Name = "XONLINE_E_LOGON_MACS_TIMEDOUT",
            Category = "Authentication",
            Meaning = "Usually indicates the servers are experiencing problems but can also be an issue with the KV.bin",
            CommonCause = "Usually indicates the servers are experiencing problems but can also be an issue with the KV.bin",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151908",
            Name = "XONLINE_E_LOGON_AUTHENTICATION_FAILED",
            Category = "Authentication",
            Meaning = "Logon Authentication Failed",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151909",
            Name = "XONLINE_E_LOGON_AUTHENTICATION_TIMEDOUT",
            Category = "Authentication",
            Meaning = "Logon Authentication Timedout",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015190A",
            Name = "XONLINE_E_LOGON_AUTHORIZATION_FAILED",
            Category = "Authentication",
            Meaning = "Logon Authorization Failed",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015190B",
            Name = "XONLINE_E_LOGON_AUTHORIZATION_TIMEDOUT",
            Category = "Authentication",
            Meaning = "Logon Authorization Timedout",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015190C",
            Name = "XONLINE_E_LOGON_XBOX_ACCOUNT_INVALID",
            Category = "Authentication",
            Meaning = "Logon Xbox Account Invalid",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015190D",
            Name = "XONLINE_E_LOGON_XBOX_ACCOUNT_BANNED",
            Category = "Authentication",
            Meaning = "Logon Xbox Account Banned",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015190E",
            Name = "XONLINE_E_LOGON_SG_SERVICE_FAILED",
            Category = "Authentication",
            Meaning = "Logon Sg Servicfailed",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015190F",
            Name = "XONLINE_E_LOGON_PRESENCE_SERVICE_FAILED",
            Category = "Authentication",
            Meaning = "Logon Presencservicfailed",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151910",
            Name = "XONLINE_E_LOGON_PRESENCE_SERVICE_TIMEDOUT",
            Category = "Authentication",
            Meaning = "Logon Presencservictimedout",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151911",
            Name = "XONLINE_E_LOGON_TIMEDOUT",
            Category = "Authentication",
            Meaning = "Logon Timedout",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151912",
            Name = "XONLINE_E_LOGON_UNKNOWN_TITLE",
            Category = "Authentication",
            Meaning = "Logon Unknown Title",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151913",
            Name = "XONLINE_E_LOGON_INTERNAL_ERROR",
            Category = "Authentication",
            Meaning = "Logon Internal Error",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151914",
            Name = "XONLINE_E_LOGON_MACHINE_AUTHENTICATION_FAILED",
            Category = "Authentication",
            Meaning = "Logon Machinauthentication Failed",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151915",
            Name = "XONLINE_E_LOGON_TGT_REVOKED",
            Category = "Authentication",
            Meaning = "Logon Tgt Revoked",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151916",
            Name = "XONLINE_E_LOGON_CACHE_MISS",
            Category = "Authentication",
            Meaning = "Logon Cachmiss",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151917",
            Name = "XONLINE_E_LOGON_NOT_UPNP_NAT",
            Category = "Authentication",
            Meaning = "Logon Not Upnp Nat",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151918",
            Name = "XONLINE_E_LOGON_INCONCLUSIVE_UPNP_NAT",
            Category = "Authentication",
            Meaning = "Logon Inconclusivupnp Nat",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151919",
            Name = "XONLINE_E_LOGON_UPNP_NAT_HARD_FAILURE",
            Category = "Authentication",
            Meaning = "Logon Upnp Nat Hard Failure",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015191A",
            Name = "XONLINE_E_LOGON_UPNP_PORT_UNAVAILABLE",
            Category = "Authentication",
            Meaning = "Logon Upnp Port Unavailable",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015191B",
            Name = "XONLINE_E_LOGON_PPLOGIN_OFFLINE",
            Category = "Authentication",
            Meaning = "Logon Pplogin Offline",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015191C",
            Name = "XONLINE_E_LOGON_KERBEROS_BIND_FAILURE",
            Category = "Authentication",
            Meaning = "Logon Kerberobind Failure",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015191D",
            Name = "XONLINE_E_LOGON_LIVE_PORT_UNAVAILABLE",
            Category = "Authentication",
            Meaning = "Logon Livport Unavailable",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015191E",
            Name = "XONLINE_E_LOGON_LIVE_PORT_OVERRIDE_UNAVAILABLE",
            Category = "Authentication",
            Meaning = "Logon Livport Overridunavailable",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015191F",
            Name = "XONLINE_E_LOGON_SG_SERVICE_NIC_MISMATCH",
            Category = "Authentication",
            Meaning = "Logon Sg Servicnic Mismatch",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151920",
            Name = "XONLINE_E_LOGON_WLID_XUID_MISMATCH",
            Category = "Authentication",
            Meaning = "Logon Wlid Xuid Mismatch",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151921",
            Name = "XONLINE_E_LOGON_SU_MANIFEST_MISMATCH",
            Category = "Authentication",
            Meaning = "Logon Su Manifest Mismatch",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151922",
            Name = "XONLINE_E_LOGON_SU_FLASH_MISMATCH",
            Category = "Authentication",
            Meaning = "Logon Su Flash Mismatch",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151923",
            Name = "XONLINE_E_LOGON_SYSTEM_UPDATE_REQUIRED",
            Category = "Authentication",
            Meaning = "Logon System Updatrequired",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151924",
            Name = "XONLINE_E_LOGON_UPDATE_NOT_PROPPED",
            Category = "Authentication",
            Meaning = "Logon Updatnot Propped",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151925",
            Name = "XONLINE_E_LOGON_LIVEHIVE_FAILED",
            Category = "Authentication",
            Meaning = "Logon Livehivfailed",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151926",
            Name = "XONLINE_E_LOGON_INVALID_CONSOLE_ID",
            Category = "Authentication",
            Meaning = "Logon Invalid Consolid",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151927",
            Name = "XONLINE_E_LOGON_XBOX_ACCOUNT_BANNED_TEMP",
            Category = "Authentication",
            Meaning = "Logon Xbox Account Banned Temp",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151928",
            Name = "XONLINE_E_LOGON_XBOX_ACCOUNT_BANNED_REPAIR",
            Category = "Authentication",
            Meaning = "Logon Xbox Account Banned Repair",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80151929",
            Name = "XONLINE_E_LOGON_DUPLICATE_CONSOLE_ID",
            Category = "Authentication",
            Meaning = "Logon Duplicatconsolid",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015192A",
            Name = "XONLINE_E_LOGON_MISSING_CONSOLE_ID",
            Category = "Authentication",
            Meaning = "Logon Missing Consolid",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80152000",
            Name = "XONLINE_E_NOTIFICATION_ERROR",
            Category = "Notification",
            Meaning = "Notification Error",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80152001",
            Name = "XONLINE_E_NOTIFICATION_SERVER_BUSY",
            Category = "Notification",
            Meaning = "Notification Server Busy",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80152002",
            Name = "XONLINE_E_NOTIFICATION_LIST_FULL",
            Category = "Notification",
            Meaning = "Notification List Full",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80152003",
            Name = "XONLINE_E_NOTIFICATION_BLOCKED",
            Category = "Notification",
            Meaning = "Notification Blocked",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80152004",
            Name = "XONLINE_E_NOTIFICATION_FRIEND_PENDING",
            Category = "Notification",
            Meaning = "Notification Friend Pending",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80152005",
            Name = "XONLINE_E_NOTIFICATION_FLUSH_TICKETS",
            Category = "Notification",
            Meaning = "Notification Flush Tickets",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80152006",
            Name = "XONLINE_E_NOTIFICATION_TOO_MANY_REQUESTS",
            Category = "Notification",
            Meaning = "Notification Too Many Requests",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80152007",
            Name = "XONLINE_E_NOTIFICATION_USER_ALREADY_EXISTS",
            Category = "Notification",
            Meaning = "Notification User Already Exists",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80152008",
            Name = "XONLINE_E_NOTIFICATION_USER_NOT_FOUND",
            Category = "Notification",
            Meaning = "Notification User Not Found",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80152009",
            Name = "XONLINE_E_NOTIFICATION_OTHER_LIST_FULL",
            Category = "Notification",
            Meaning = "Notification Other List Full",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x8015200A",
            Name = "XONLINE_E_NOTIFICATION_SELF",
            Category = "Notification",
            Meaning = "Notification Self",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x8015200B",
            Name = "XONLINE_E_NOTIFICATION_SAME_TITLE",
            Category = "Notification",
            Meaning = "Notification Samtitle",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x8015200C",
            Name = "XONLINE_E_NOTIFICATION_NO_TASK",
            Category = "Notification",
            Meaning = "Notification No Task",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x8015200D",
            Name = "XONLINE_E_NOTIFICATION_NO_DATA",
            Category = "Notification",
            Meaning = "Notification No Data",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x0015200E",
            Name = "XONLINE_E_NOTIFICATION_NO_PEER_SUBSCRIBE",
            Category = "Success",
            Meaning = "Notification No Peer Subscribe",
            CommonCause = "Operation completed successfully or returned an informational status.",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x8015200F",
            Name = "XONLINE_E_NOTIFICATION_THREAD_ERROR",
            Category = "Notification",
            Meaning = "thread wait timeout",
            CommonCause = "thread wait timeout",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80152010",
            Name = "XONLINE_E_NOTIFICATION_STATE_ERROR",
            Category = "Notification",
            Meaning = "Notification Staterror",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x0015200E",
            Name = "XONLINE_S_NOTIFICATION_NO_PEER_SUBSCRIBE",
            Category = "Success",
            Meaning = "Notification No Peer Subscribe",
            CommonCause = "Operation completed successfully or returned an informational status.",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80152100",
            Name = "XONLINE_E_TEAMS_SERVER_BUSY",
            Category = "Teams",
            Meaning = "Teamserver Busy",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x80152101",
            Name = "XONLINE_E_TEAMS_TEAM_FULL",
            Category = "Teams",
            Meaning = "Teamteam Full",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x80152102",
            Name = "XONLINE_E_TEAMS_MEMBER_PENDING",
            Category = "Teams",
            Meaning = "Teammember Pending",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x80152103",
            Name = "XONLINE_E_TEAMS_TOO_MANY_REQUESTS",
            Category = "Teams",
            Meaning = "Teamtoo Many Requests",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x80152104",
            Name = "XONLINE_E_TEAMS_USER_ALREADY_EXISTS",
            Category = "Teams",
            Meaning = "Teamuser Already Exists",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x80152105",
            Name = "XONLINE_E_TEAMS_USER_NOT_FOUND",
            Category = "Teams",
            Meaning = "Teamuser Not Found",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x80152106",
            Name = "XONLINE_E_TEAMS_USER_TEAMS_FULL",
            Category = "Teams",
            Meaning = "Teamuser Teamfull",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x80152107",
            Name = "XONLINE_E_TEAMS_SELF",
            Category = "Teams",
            Meaning = "Teamself",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x80152108",
            Name = "XONLINE_E_TEAMS_NO_TASK",
            Category = "Teams",
            Meaning = "Teamno Task",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x80152109",
            Name = "XONLINE_E_TEAMS_TOO_MANY_TEAMS",
            Category = "Teams",
            Meaning = "Teamtoo Many Teams",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x8015210A",
            Name = "XONLINE_E_TEAMS_TEAM_ALREADY_EXISTS",
            Category = "Teams",
            Meaning = "Teamteam Already Exists",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x8015210B",
            Name = "XONLINE_E_TEAMS_TEAM_NOT_FOUND",
            Category = "Teams",
            Meaning = "Teamteam Not Found",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x8015210C",
            Name = "XONLINE_E_TEAMS_INSUFFICIENT_PRIVILEGES",
            Category = "Teams",
            Meaning = "Teaminsufficient Privileges",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x8015210D",
            Name = "XONLINE_E_TEAMS_NAME_CONTAINS_BAD_WORDS",
            Category = "Teams",
            Meaning = "Teamnamcontainbad Words",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x8015210E",
            Name = "XONLINE_E_TEAMS_DESCRIPTION_CONTAINS_BAD_WORDS",
            Category = "Teams",
            Meaning = "Teamdescription Containbad Words",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x8015210F",
            Name = "XONLINE_E_TEAMS_MOTTO_CONTAINS_BAD_WORDS",
            Category = "Teams",
            Meaning = "Teammotto Containbad Words",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x80152110",
            Name = "XONLINE_E_TEAMS_URL_CONTAINS_BAD_WORDS",
            Category = "Teams",
            Meaning = "Teamurl Containbad Words",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x80152111",
            Name = "XONLINE_E_TEAMS_NOT_A_MEMBER",
            Category = "Teams",
            Meaning = "Teamnot A Member",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x80152112",
            Name = "XONLINE_E_TEAMS_NO_ADMIN",
            Category = "Teams",
            Meaning = "Teamno Admin",
            CommonCause = "Team membership, capacity, permission, or moderation issue.",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x80153001",
            Name = "XOFF_E_BAD_REQUEST",
            Category = "Validation",
            Meaning = "Bad Request",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80153002",
            Name = "XOFF_E_INVALID_USER",
            Category = "Validation",
            Meaning = "Invalid User",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80153003",
            Name = "XOFF_E_INVALID_OFFER_ID",
            Category = "Marketplace",
            Meaning = "Invalid Offer Id",
            CommonCause = "Offer, license, entitlement, content availability, or region restriction issue.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153004",
            Name = "XOFF_E_INELIGIBLE_FOR_OFFER",
            Category = "Marketplace",
            Meaning = "Ineligiblfor Offer",
            CommonCause = "Offer, license, entitlement, content availability, or region restriction issue.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153005",
            Name = "XOFF_E_OFFER_EXPIRED",
            Category = "Marketplace",
            Meaning = "Offer Expired",
            CommonCause = "Offer, license, entitlement, content availability, or region restriction issue.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153006",
            Name = "XOFF_E_SERVICE_UNREACHABLE",
            Category = "Xbox Live",
            Meaning = "Servicunreachable",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80153007",
            Name = "XOFF_E_PURCHASE_BLOCKED",
            Category = "Billing",
            Meaning = "Purchasblocked",
            CommonCause = "Payment method, purchase state, billing provider, points balance, or transaction issue.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153008",
            Name = "XOFF_E_PURCHASE_DENIED",
            Category = "Billing",
            Meaning = "Purchasdenied",
            CommonCause = "Payment method, purchase state, billing provider, points balance, or transaction issue.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153009",
            Name = "XOFF_E_BILLING_SERVER_ERROR",
            Category = "Billing",
            Meaning = "Billing Server Error",
            CommonCause = "Payment method, purchase state, billing provider, points balance, or transaction issue.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015300A",
            Name = "XOFF_E_OFFER_NOT_CANCELABLE",
            Category = "Marketplace",
            Meaning = "Offer Not Cancelable",
            CommonCause = "Offer, license, entitlement, content availability, or region restriction issue.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015300B",
            Name = "XOFF_E_NOTHING_TO_CANCEL",
            Category = "Xbox Live",
            Meaning = "Nothing To Cancel",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015300C",
            Name = "XOFF_E_ALREADY_OWN_MAX",
            Category = "Xbox Live",
            Meaning = "Already Own Max",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015300D",
            Name = "XOFF_E_NO_CHARGE",
            Category = "Xbox Live",
            Meaning = "No Charge",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015300E",
            Name = "XOFF_E_PERMISSION_DENIED",
            Category = "Security",
            Meaning = "Permission Denied",
            CommonCause = "Access denied, blocked request, enforcement state, certificate, or trust validation issue.",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x8015300F",
            Name = "XOFF_E_INVALID_PRODUCT",
            Category = "Validation",
            Meaning = "Invalid Product",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80153000",
            Name = "XONLINE_E_BILLING_ERROR",
            Category = "Billing",
            Meaning = "server received incorrectly formatted request",
            CommonCause = "server received incorrectly formatted request",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153001",
            Name = "XONLINE_E_OFFERING_BAD_REQUEST",
            Category = "Marketplace",
            Meaning = "server received incorrectly formatted request",
            CommonCause = "server received incorrectly formatted request",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153002",
            Name = "XONLINE_E_OFFERING_INVALID_USER",
            Category = "Marketplace",
            Meaning = "cannot find account for this user",
            CommonCause = "cannot find account for this user",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153003",
            Name = "XONLINE_E_OFFERING_INVALID_OFFER_ID",
            Category = "Marketplace",
            Meaning = "offer does not exist",
            CommonCause = "offer does not exist",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153004",
            Name = "DEPRECATED_XONLINE_E_OFFERING_INELIGIBLE_FOR_OFFER",
            Category = "Marketplace",
            Meaning = ")] private /title not allowed to purchase offer",
            CommonCause = ")] private /title not allowed to purchase offer",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153005",
            Name = "XONLINE_E_OFFERING_OFFER_EXPIRED",
            Category = "Marketplace",
            Meaning = "offer no longer available",
            CommonCause = "offer no longer available",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153006",
            Name = "XONLINE_E_OFFERING_SERVICE_UNREACHABLE",
            Category = "Marketplace",
            Meaning = "apparent connectivity problems",
            CommonCause = "apparent connectivity problems",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153007",
            Name = "XONLINE_E_OFFERING_PURCHASE_BLOCKED",
            Category = "Billing",
            Meaning = "this user is not allowed to make purchases",
            CommonCause = "this user is not allowed to make purchases",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153008",
            Name = "XONLINE_E_OFFERING_PURCHASE_DENIED",
            Category = "Billing",
            Meaning = "this user’s payment is denied by billing provider",
            CommonCause = "this user’s payment is denied by billing provider",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153009",
            Name = "XONLINE_E_OFFERING_BILLING_SERVER_ERROR",
            Category = "Billing",
            Meaning = "nonspecific billing provider error",
            CommonCause = "nonspecific billing provider error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015300A",
            Name = "XONLINE_E_OFFERING_OFFER_NOT_CANCELABLE",
            Category = "Marketplace",
            Meaning = "either this offer doesn’t exist, or it’s marked as un-cancelable",
            CommonCause = "either this offer doesn’t exist, or it’s marked as un-cancelable",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015300B",
            Name = "XONLINE_E_OFFERING_NOTHING_TO_CANCEL",
            Category = "Marketplace",
            Meaning = "this user doesn’t have one of these anyways",
            CommonCause = "this user doesn’t have one of these anyways",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015300C",
            Name = "XONLINE_E_OFFERING_ALREADY_OWN_MAX",
            Category = "Marketplace",
            Meaning = "this user already owns the maximum allowed",
            CommonCause = "this user already owns the maximum allowed",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015300D",
            Name = "XONLINE_E_OFFERING_NO_CHARGE",
            Category = "Marketplace",
            Meaning = "this is a free offer; no purchase is necessary",
            CommonCause = "this is a free offer; no purchase is necessary",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015300E",
            Name = "XONLINE_E_OFFERING_PERMISSION_DENIED",
            Category = "Marketplace",
            Meaning = "permission denied",
            CommonCause = "permission denied",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015300F",
            Name = "XONLINE_E_OFFERING_NAME_TAKEN",
            Category = "Marketplace",
            Meaning = "Name given to XOnlineVerifyNickname is taken (dosen’t vet)",
            CommonCause = "Name given to XOnlineVerifyNickname is taken (dosen’t vet)",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153010",
            Name = "XONLINE_E_OFFERING_BASE_OFFER_NOT_CANCELABLE",
            Category = "Marketplace",
            Meaning = "Base subscription not cancelable due to dependent subscriptions",
            CommonCause = "Base subscription not cancelable due to dependent subscriptions",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153011",
            Name = "XONLINE_E_AUTOUPD_MACHINE_BLOCKED",
            Category = "Security",
            Meaning = "No autoupd referrals given because machine puid is blocked",
            CommonCause = "No autoupd referrals given because machine puid is blocked",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x80153012",
            Name = "XONLINE_E_OFFERING_INVALID_OFFER_TYPE",
            Category = "Marketplace",
            Meaning = "PurchaseSubscription called with non-subscription offer or PurchaseContent called with non-content offer",
            CommonCause = "PurchaseSubscription called with non-subscription offer or PurchaseContent called with non-content offer",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153013",
            Name = "XONLINE_E_OFFERING_INVALID_CONSUME_ITEMS",
            Category = "Marketplace",
            Meaning = "Consume was called with an asset that had insufficient quantity owned by the user.",
            CommonCause = "Consume was called with an asset that had insufficient quantity owned by the user.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153014",
            Name = "XONLINE_E_MULTI_PURCHASE_INVALID_OFFER_TYPE",
            Category = "Billing",
            Meaning = "A multi purchase was requested, but not all of the offers were of the allowed type for a multi-purchase",
            CommonCause = "A multi purchase was requested, but not all of the offers were of the allowed type for a multi-purchase",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153015",
            Name = "XONLINE_E_MULTI_PURCHASE_INVALID_PAYMENT_TYPE",
            Category = "Billing",
            Meaning = "A multi purchase was requested with an unsupported payment type",
            CommonCause = "A multi purchase was requested with an unsupported payment type",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153016",
            Name = "XONLINE_E_OFFERING_PRICE_CHANGED",
            Category = "Marketplace",
            Meaning = "The client expected a certain price but that price didn’t match the server-defined price",
            CommonCause = "The client expected a certain price but that price didn’t match the server-defined price",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153017",
            Name = "DEPRECATED_XONLINE_E_OFFERING_NOT_ACQUIRABLE",
            Category = "Marketplace",
            Meaning = "A purchase was attempted on a video offer which is currently not Acquirable",
            CommonCause = "A purchase was attempted on a video offer which is currently not Acquirable",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153018",
            Name = "XONLINE_E_PENDING_POINTS_PURCHASE",
            Category = "Billing",
            Meaning = "A points purchase is already pending, and so a new points purchase cannot be started",
            CommonCause = "A points purchase is already pending, and so a new points purchase cannot be started",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153019",
            Name = "XONLINE_E_OFFERING_SUBSCRIPTION_NOT_FOUND",
            Category = "Marketplace",
            Meaning = "User has no subscription for given service component or service id",
            CommonCause = "User has no subscription for given service component or service id",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015301A",
            Name = "XONLINE_E_OFFERING_UNKNOWN_OFFER_TYPE",
            Category = "Marketplace",
            Meaning = "Unknown offer type.",
            CommonCause = "Unknown offer type.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015301B",
            Name = "XONLINE_E_OFFERING_UNKNOWN_PAYMENT_TYPE",
            Category = "Billing",
            Meaning = "Unknown payment type.",
            CommonCause = "Unknown payment type.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015301C",
            Name = "DEPRECATED_XONLINE_E_OFFERING_INVALID_SOURCE_MACHINE",
            Category = "Marketplace",
            Meaning = "The source machine is not allowed to participate in license transfers of this type",
            CommonCause = "The source machine is not allowed to participate in license transfers of this type",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015301D",
            Name = "DEPRECATED_XONLINE_E_OFFERING_INVALID_DEST_MACHINE",
            Category = "Marketplace",
            Meaning = "The destination machine is not allowed to participate in license transfers of this type",
            CommonCause = "The destination machine is not allowed to participate in license transfers of this type",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x0015301E",
            Name = "DEPRECATED_XONLINE_S_OTHER_LICENSE_TRANSFER_FAILED",
            Category = "Success",
            Meaning = "Success code indicating that the license transfer succeeded for the calling user, but one or more licenses on the box belonging to other users could not be migrated.",
            CommonCause = "Success code indicating that the license transfer succeeded for the calling user, but one or more licenses on the box belonging to other users could not be migrated.",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x0015301F",
            Name = "XONLINE_S_USER_OWNS_NO_LICENSES",
            Category = "Success",
            Meaning = "Success code indicating that the license transfer aborted for the calling user because he does not own any licenses on the old console.",
            CommonCause = "Success code indicating that the license transfer aborted for the calling user because he does not own any licenses on the old console.",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80153020",
            Name = "XONLINE_E_MEDIA_INSTANCE_NOT_ACQUIRABLE",
            Category = "Xbox Live",
            Meaning = "Success code indicating that the license transfer aborted for the calling user because he does not own any licenses on the old console.",
            CommonCause = "Success code indicating that the license transfer aborted for the calling user because he does not own any licenses on the old console.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80153021",
            Name = "XONLINE_E_PURCHASE_PENDING",
            Category = "Billing",
            Meaning = "The user has a pending purchase transaction already. Please wait and try again later.",
            CommonCause = "The user has a pending purchase transaction already. Please wait and try again later.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153022",
            Name = "XONLINE_E_OFFERING_OFFER_MISCONFIGURED",
            Category = "Marketplace",
            Meaning = "offer exists but is misconfigured in some way",
            CommonCause = "offer exists but is misconfigured in some way",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153023",
            Name = "XONLINE_E_OFFERING_OFFER_NOT_VISIBLE",
            Category = "Marketplace",
            Meaning = "offer exists but has a visibilityDate in the future",
            CommonCause = "offer exists but has a visibilityDate in the future",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153024",
            Name = "XONLINE_E_OFFERING_VISIBLITY_STATUS_NOT_ALLOWED",
            Category = "Marketplace",
            Meaning = "offer exists but has a visibilityStatusId that does not allow purchase",
            CommonCause = "offer exists but has a visibilityStatusId that does not allow purchase",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153025",
            Name = "XONLINE_E_OFFERING_GRANTS_NOTHING",
            Category = "Marketplace",
            Meaning = "offer exists but currently grants no mediaInstances, so purchase was blocked",
            CommonCause = "offer exists but currently grants no mediaInstances, so purchase was blocked",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153026",
            Name = "XONLINE_E_OFFERING_INVALID_PAYMENT_TYPE",
            Category = "Billing",
            Meaning = "offer does not allow the given payment type",
            CommonCause = "offer does not allow the given payment type",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153027",
            Name = "XONLINE_E_OFFERING_MEDIA_TYPE_MISMATCH",
            Category = "Marketplace",
            Meaning = "offer media type in the catalog does not match the given media type",
            CommonCause = "offer media type in the catalog does not match the given media type",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153028",
            Name = "XONLINE_E_MULTI_PURCHASE_INVALID_PLATFORM_TYPE",
            Category = "Billing",
            Meaning = "A multi purchase was requested from a platform type that either could not be determed or is unsupported.",
            CommonCause = "A multi purchase was requested from a platform type that either could not be determed or is unsupported.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153029",
            Name = "XONLINE_E_BILLING_NOT_CONNECTED",
            Category = "Billing",
            Meaning = "connection to billing system is not configured",
            CommonCause = "connection to billing system is not configured",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015302A",
            Name = "XONLINE_E_OFFERING_MISSING_RESULTS",
            Category = "Marketplace",
            Meaning = "expected additional results from sproc",
            CommonCause = "expected additional results from sproc",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015302B",
            Name = "XONLINE_E_OFFERING_PAYMENT_INFO_TOO_LONG",
            Category = "Billing",
            Meaning = "length of payment info exceeds width of database columsn",
            CommonCause = "length of payment info exceeds width of database columsn",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015302C",
            Name = "XONLINE_E_OFFERING_MIGRATION_ERROR",
            Category = "Marketplace",
            Meaning = "error migrating xbox1 offer to xenon offer",
            CommonCause = "error migrating xbox1 offer to xenon offer",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015302D",
            Name = "XONLINE_E_GEOFENCING_LOOKUP_ERROR",
            Category = "Xbox Live",
            Meaning = "geofencing returned an exception during lookup",
            CommonCause = "geofencing returned an exception during lookup",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015302E",
            Name = "XONLINE_E_GEOFENCING_RETURNED_NULL",
            Category = "Xbox Live",
            Meaning = "returned null from IP lookup",
            CommonCause = "returned null from IP lookup",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015302F",
            Name = "XONLINE_E_OFFERING_VERIFY_TOKEN_ERROR",
            Category = "Authentication",
            Meaning = "exception while verifying token",
            CommonCause = "exception while verifying token",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80153030",
            Name = "XONLINE_E_BILLING_USER_QUEUED",
            Category = "Billing",
            Meaning = "operation not allowed because user is queued",
            CommonCause = "operation not allowed because user is queued",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153031",
            Name = "XONLINE_E_BILLING_KEY_NOT_FOUND",
            Category = "Billing",
            Meaning = "billing provider key is not configured",
            CommonCause = "billing provider key is not configured",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153032",
            Name = "XONLINE_E_BILLING_COUNTRY_ID_NOT_FOUND",
            Category = "Billing",
            Meaning = "invalid country id",
            CommonCause = "invalid country id",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153033",
            Name = "XONLINE_E_BILLING_LOCALE_NOT_FOUND",
            Category = "Billing",
            Meaning = "locale not found",
            CommonCause = "locale not found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153034",
            Name = "XONLINE_E_BILLING_QUEUED_ACCOUNT_NOT_FOUND",
            Category = "Billing",
            Meaning = "error loading billing queue item",
            CommonCause = "error loading billing queue item",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153035",
            Name = "XONLINE_E_OFFERING_AUTOUPD_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153036",
            Name = "XONLINE_E_OFFERING_BANNER_LIST_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153037",
            Name = "XONLINE_E_OFFERING_CONTENT_AVAILABLE_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153038",
            Name = "XONLINE_E_OFFERING_CONTENT_DETAILS_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153039",
            Name = "XONLINE_E_OFFERING_CONTENT_ENUMERATE_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015303A",
            Name = "XONLINE_E_OFFERING_CONTENT_HISTORY_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015303B",
            Name = "XONLINE_E_OFFERING_CONTENT_REFERRAL_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015303C",
            Name = "XONLINE_E_OFFERING_CONTENT_UPDATE_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015303D",
            Name = "XONLINE_E_OFFERING_ENUMERATE_GENRES_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015303E",
            Name = "XONLINE_E_OFFERING_ENUMERATE_TITLES_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015303F",
            Name = "XONLINE_E_OFFERING_FIND_MEDIA_INSTANCE_URLS_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153040",
            Name = "XONLINE_E_OFFERING_GET_TITLE_ACTIVATION_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153041",
            Name = "XONLINE_E_OFFERING_GET_TITLE_DETAILS_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153042",
            Name = "XONLINE_E_OFFERING_IN_GAME_CONTENT_AVAILABLE_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153043",
            Name = "XONLINE_E_OFFERING_IN_GAME_CONTENT_ENUMERATE_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153044",
            Name = "XONLINE_E_OFFERING_DETAILS_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153045",
            Name = "XONLINE_E_OFFERING_DETAILS_NO_USER_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153046",
            Name = "XONLINE_E_OFFERING_SUBSCRIPTION_DETAILS_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153047",
            Name = "XONLINE_E_OFFERING_CANCEL_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153048",
            Name = "XONLINE_E_OFFERING_PURCHASE_OFFERS_ERROR",
            Category = "Billing",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153049",
            Name = "XONLINE_E_OFFERING_SUBSCRIPTION_ENUMERATE_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015304A",
            Name = "XONLINE_E_OFFERING_VERIFY_NICKNAME_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015304B",
            Name = "XONLINE_E_OFFERING_CONTENT_REFRESH_LICENSE_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015304C",
            Name = "XONLINE_E_OFFERING_GET_POINTS_PURCHASE_STATUS_ERROR",
            Category = "Billing",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015304D",
            Name = "XONLINE_E_OFFERING_GET_REVOCATION_LIST_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015304E",
            Name = "XONLINE_E_OFFERING_OFFER_PURCHASE_ERROR",
            Category = "Billing",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015304F",
            Name = "XONLINE_E_OFFERING_PURCHASE_MUSIC_ERROR",
            Category = "Billing",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153050",
            Name = "XONLINE_E_OFFERING_CREATE_CERTIFICATE_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153051",
            Name = "XONLINE_E_OFFERING_PURCHASE_GAMERTAG_ERROR",
            Category = "Billing",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153052",
            Name = "XONLINE_E_BILLING_PAYMENT_INFO_NOT_FOUND_ERROR",
            Category = "Billing",
            Meaning = "payment info not found",
            CommonCause = "payment info not found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153053",
            Name = "XONLINE_E_BILLING_FRIENDLY_NAME_NOT_FOUND_ERROR",
            Category = "Billing",
            Meaning = "friendly name not found",
            CommonCause = "friendly name not found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153054",
            Name = "XONLINE_E_BILLING_CANNOT_SPECIFY_ANNIVERSARY_DATE_ERROR",
            Category = "Billing",
            Meaning = "anniversary date was specified",
            CommonCause = "anniversary date was specified",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153055",
            Name = "XONLINE_E_BILLING_CANNOT_SPECIFY_ACCOUNT_ID_ERROR",
            Category = "Billing",
            Meaning = "account id was specified",
            CommonCause = "account id was specified",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153056",
            Name = "XONLINE_E_BILLING_CANNOT_SPECIFY_PAYMENT_ID_ERROR",
            Category = "Billing",
            Meaning = "payment instrument id was specified",
            CommonCause = "payment instrument id was specified",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153057",
            Name = "XONLINE_E_BILLING_AREA_CODE_NOT_FOUND_ERROR",
            Category = "Billing",
            Meaning = "area code not found",
            CommonCause = "area code not found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153058",
            Name = "XONLINE_E_BILLING_PHONE_NUMBER_NOT_FOUND_ERROR",
            Category = "Billing",
            Meaning = "phone number not found",
            CommonCause = "phone number not found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153059",
            Name = "XONLINE_E_BILLING_ADDRESS_NOT_FOUND_ERROR",
            Category = "Billing",
            Meaning = "address not found",
            CommonCause = "address not found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015305A",
            Name = "XONLINE_E_BILLING_ACCOUNT_HOLDER_NAME_NOT_FOUND_ERROR",
            Category = "Billing",
            Meaning = "account holder name not found",
            CommonCause = "account holder name not found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015305B",
            Name = "XONLINE_E_BILLING_ACCOUNT_NUMBER_NOT_FOUND_ERROR",
            Category = "Billing",
            Meaning = "account number not found",
            CommonCause = "account number not found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015305C",
            Name = "XONLINE_E_BILLING_EXPIRATION_DATE_NOT_FOUND_ERROR",
            Category = "Billing",
            Meaning = "expiration date not found",
            CommonCause = "expiration date not found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015305D",
            Name = "XONLINE_E_BILLING_BAD_CREDIT_CARD_TYPE_ERROR",
            Category = "Billing",
            Meaning = "bad credit card type",
            CommonCause = "bad credit card type",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015305E",
            Name = "XONLINE_E_BILLING_BRANCH_CODE_NOT_FOUND_ERROR",
            Category = "Billing",
            Meaning = "expiration date not found",
            CommonCause = "expiration date not found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015305F",
            Name = "XONLINE_E_BILLING_EXTERNAL_REFERENCE_ID_NOT_FOUND",
            Category = "Billing",
            Meaning = "external reference id not found",
            CommonCause = "external reference id not found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153060",
            Name = "XONLINE_E_BILLING_WHOLESALE_PARTNER_NOT_FOUND",
            Category = "Billing",
            Meaning = "wholesale partner id not found",
            CommonCause = "wholesale partner id not found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153061",
            Name = "XONLINE_E_BILLING_ENCRYPTED_PASSWORD_NOT_FOUND",
            Category = "Billing",
            Meaning = "encrypted password id not found",
            CommonCause = "encrypted password id not found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153062",
            Name = "XONLINE_E_BILLING_STREET_1_NOT_FOUND_ERROR",
            Category = "Billing",
            Meaning = "street 1 id not found",
            CommonCause = "street 1 id not found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153063",
            Name = "XONLINE_E_BILLING_CITY_NOT_FOUND_ERROR",
            Category = "Billing",
            Meaning = "city id not found",
            CommonCause = "city id not found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153064",
            Name = "XONLINE_E_BILLING_COUNTRY_CODE_NOT_FOUND_ERROR",
            Category = "Billing",
            Meaning = "country code not found",
            CommonCause = "country code not found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153065",
            Name = "XONLINE_E_BILLING_CHILD_CONTENT_PURCHASE_NOT_ALLOWED",
            Category = "Billing",
            Meaning = "child accounts not allowed to purchase content in the user’s country",
            CommonCause = "child accounts not allowed to purchase content in the user’s country",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153066",
            Name = "XONLINE_E_BILLING_CALCULATE_TAX_ERROR",
            Category = "Billing",
            Meaning = "non-specific error calling SCS CalculateTax",
            CommonCause = "non-specific error calling SCS CalculateTax",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153067",
            Name = "XONLINE_E_BILLING_SUBMIT_ORDER_ERROR",
            Category = "Billing",
            Meaning = "non-specific error calling SCS SubmitOrder",
            CommonCause = "non-specific error calling SCS SubmitOrder",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153068",
            Name = "XONLINE_E_BILLING_SCS_SQL_TRANSACTION_FAILED",
            Category = "Database",
            Meaning = "a SQL transaction failed during an SCS SubmitOrder purchase",
            CommonCause = "a SQL transaction failed during an SCS SubmitOrder purchase",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x80153069",
            Name = "XONLINE_E_BILLING_SCS_TRANSACTION_NOT_FOUND",
            Category = "Billing",
            Meaning = "the SCS SubmitOrder tracking guid could not be found",
            CommonCause = "the SCS SubmitOrder tracking guid could not be found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015306A",
            Name = "XONLINE_E_BILLING_SCS_TRANSACTION_CANCELLED",
            Category = "Billing",
            Meaning = "the SCS SubmitOrder tracking guid has already been cancelled",
            CommonCause = "the SCS SubmitOrder tracking guid has already been cancelled",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015306B",
            Name = "XONLINE_E_BILLING_SCS_TRANSACTION_UNEXPECTED_STATE",
            Category = "Billing",
            Meaning = "the SCS SubmitOrder tracking guid was found with an unexpected status id",
            CommonCause = "the SCS SubmitOrder tracking guid was found with an unexpected status id",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015306C",
            Name = "XONLINE_E_BILLING_CHILD_OFFER_PURCHASE_NOT_ALLOWED",
            Category = "Billing",
            Meaning = "child accounts not allowed to purchase offer in user’s country",
            CommonCause = "child accounts not allowed to purchase offer in user’s country",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015306D",
            Name = "XONLINE_E_OFFERING_ASSET_CONSUME_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015306E",
            Name = "XONLINE_E_OFFERING_ASSET_ENUMERATE_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015306F",
            Name = "XONLINE_E_OFFERING_SIGN_ASSETS_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153070",
            Name = "XONLINE_E_BILLING_FINBUS_MISSING_XNA_CREATOR_ID_ERROR",
            Category = "Billing",
            Meaning = "Media purchased is of type XNA Community Game but catalog looking did not find an XNA Creator Id",
            CommonCause = "Media purchased is of type XNA Community Game but catalog looking did not find an XNA Creator Id",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153075",
            Name = "XONLINE_E_BILLING_CTP_COMMUNICATION_ERROR",
            Category = "Billing",
            Meaning = "Error raised when we communication exception is received from CTP",
            CommonCause = "Error raised when we communication exception is received from CTP",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153076",
            Name = "XONLINE_E_BILLING_CTP_VALIDATION_RENEWAL_ERROR",
            Category = "Billing",
            Meaning = "Renewal Path Error",
            CommonCause = "Renewal Path Error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153077",
            Name = "XONLINE_E_BILLING_CTP_INVALIDRESPONSE",
            Category = "Billing",
            Meaning = "Multiple nodes returned for Purchase call.Should not be happening.",
            CommonCause = "Multiple nodes returned for Purchase call.Should not be happening.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153078",
            Name = "XONLINE_E_BILLING_CTP_AUTHORIZATION",
            Category = "Authentication",
            Meaning = "CTP Auth error",
            CommonCause = "CTP Auth error",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80153079",
            Name = "XONLINE_E_BILLING_CTP_INVALID_PAYMENT",
            Category = "Billing",
            Meaning = "Payment instrument errors",
            CommonCause = "Payment instrument errors",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015307A",
            Name = "XONLINE_E_BILLING_CTP_INVALID_TRACKING_GUID",
            Category = "Billing",
            Meaning = "Invalid Tracking Guid",
            CommonCause = "Invalid Tracking Guid",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015307B",
            Name = "XONLINE_E_BILLING_CTP_INVALID_PAYMENTPROVIDER",
            Category = "Billing",
            Meaning = "Invalid Payment Provider",
            CommonCause = "Invalid Payment Provider",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015307C",
            Name = "XONLINE_E_BILLING_CTP_INVALID_CALLINGPARTNER",
            Category = "Billing",
            Meaning = "Invalid Calling partner",
            CommonCause = "Invalid Calling partner",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015307D",
            Name = "XONLINE_E_BILLING_CTP_INVALID_ITEM",
            Category = "Billing",
            Meaning = "Invalid Purchase Item passed",
            CommonCause = "Invalid Purchase Item passed",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015307E",
            Name = "XONLINE_E_BILLING_CTP_INVALID_TRANSACTIONSTATUS",
            Category = "Billing",
            Meaning = "Invalid Transaction status",
            CommonCause = "Invalid Transaction status",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015307F",
            Name = "XONLINE_E_BILLING_CTP_UNKNOWN",
            Category = "Billing",
            Meaning = "Unknown Error",
            CommonCause = "Unknown Error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153080",
            Name = "XONLINE_E_BILLING_CTP_INVALID_OFFER_NOTFOUND",
            Category = "Billing",
            Meaning = "OfferId not found for Billing OfferId",
            CommonCause = "OfferId not found for Billing OfferId",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153081",
            Name = "XONLINE_E_BILLING_CTP_NOTSUPPORTED_MEDIATYPE",
            Category = "Network",
            Meaning = "Mediatype not supported",
            CommonCause = "Mediatype not supported",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80153082",
            Name = "XONLINE_E_BILLING_CTP_INVALID_TOKEN",
            Category = "Authentication",
            Meaning = "Token is invalid",
            CommonCause = "Token is invalid",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80153083",
            Name = "XONLINE_E_BILLING_CTP_INVALID_SUB_OFFER_COUNT",
            Category = "Billing",
            Meaning = "More than one offer found for subscription purchase",
            CommonCause = "More than one offer found for subscription purchase",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153084",
            Name = "XONLINE_E_OFFERING_GET_LEGACYOFFER_ERROR",
            Category = "Marketplace",
            Meaning = "Error encountered looking up legacy offerId",
            CommonCause = "Error encountered looking up legacy offerId",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153085",
            Name = "XONLINE_E_OFFERING_GET_OFFERINSTANCE_ERROR",
            Category = "Marketplace",
            Meaning = "Error looking up the mapping for OfferInstance and Offer",
            CommonCause = "Error looking up the mapping for OfferInstance and Offer",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153086",
            Name = "XONLINE_E_OFFERING_INVALID_RENEWAL",
            Category = "Marketplace",
            Meaning = "No renewal path existing",
            CommonCause = "No renewal path existing",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153087",
            Name = "XONLINE_E_OFFERING_MULTIPLE_RENEWAL",
            Category = "Marketplace",
            Meaning = "Multiple renewal path existing",
            CommonCause = "Multiple renewal path existing",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153088",
            Name = "XONLINE_E_BILLING_CTP_INVALID_TIMEEXTENSION",
            Category = "Billing",
            Meaning = "Time Extension passed is invalid",
            CommonCause = "Time Extension passed is invalid",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153089",
            Name = "XONLINE_E_BILLING_CTP_RATING_RULES_ERROR",
            Category = "Billing",
            Meaning = "Invalid rating rules",
            CommonCause = "Invalid rating rules",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015308A",
            Name = "XONLINE_E_BILLING_CTP_INVALID_MODE",
            Category = "Billing",
            Meaning = "CTP Convert mode is invalid.Renewal not supported",
            CommonCause = "CTP Convert mode is invalid.Renewal not supported",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015308B",
            Name = "XONLINE_E_BILLING_CTP_NO_RETRYABLE_PURCHASE",
            Category = "Billing",
            Meaning = "No retryable items in CTP queue",
            CommonCause = "No retryable items in CTP queue",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015308C",
            Name = "XONLINE_E_BILLING_CTP_TRANSACTION_NOT_FOUND",
            Category = "Billing",
            Meaning = "No transaction found update",
            CommonCause = "No transaction found update",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015308D",
            Name = "XONLINE_E_BILLING_CTP_RETRY_INTERVAL_EXCEEDED",
            Category = "Billing",
            Meaning = "Retry interval exceeded",
            CommonCause = "Retry interval exceeded",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015308E",
            Name = "XONLINE_E_BILLING_CTP_TRANSACTION_SQL_ERROR",
            Category = "Database",
            Meaning = "Unknown SQL error",
            CommonCause = "Unknown SQL error",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x8015308F",
            Name = "XONLINE_E_BILLING_CTP_QUEUE_ITERATOR_ERROR",
            Category = "Billing",
            Meaning = "Cannot build Queue Iterator",
            CommonCause = "Cannot build Queue Iterator",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153090",
            Name = "XONLINE_E_BILLING_CTP_QUEUE_PURCHASE_INTENT_FAILURE",
            Category = "Billing",
            Meaning = "Error creating CTP Purchase intent",
            CommonCause = "Error creating CTP Purchase intent",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153091",
            Name = "XONLINE_E_BILLING_SUBSCRIPTION_CREDIT_VALUE_MAPPING_ERROR",
            Category = "Billing",
            Meaning = "Error loading CTP SubscriptionCreditValue mapping",
            CommonCause = "Error loading CTP SubscriptionCreditValue mapping",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153092",
            Name = "XONLINE_E_BILLING_BAD_CREDIT_VALUE",
            Category = "Billing",
            Meaning = "Negative credit value configured",
            CommonCause = "Negative credit value configured",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153093",
            Name = "XONLINE_E_OFFERING_GET_OFFERINGGUID_ERROR",
            Category = "Marketplace",
            Meaning = "Error encountered looking up mapping for legacy id to guid",
            CommonCause = "Error encountered looking up mapping for legacy id to guid",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153094",
            Name = "XONLINE_E_OFFERING_GET_LEGACYID_ERROR",
            Category = "Marketplace",
            Meaning = "Error encountered looking up mapping for guid to legacy id",
            CommonCause = "Error encountered looking up mapping for guid to legacy id",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153095",
            Name = "XONLINE_E_BILLING_CTP_PARTNERCONFIG_UNKNOWN_ERROR",
            Category = "Billing",
            Meaning = "Error encountered during CTP Partner Configuration",
            CommonCause = "Error encountered during CTP Partner Configuration",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153096",
            Name = "XONLINE_E_BILLING_CTP_INVALID_OVERRIDDEN_PRICE",
            Category = "Billing",
            Meaning = "Overridden Price is negative",
            CommonCause = "Overridden Price is negative",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153097",
            Name = "XONLINE_E_BILLING_NOUNLIST_NOT_CONFIGURED",
            Category = "Billing",
            Meaning = "Noun List not configured for country id",
            CommonCause = "Noun List not configured for country id",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153098",
            Name = "XONLINE_E_BILLING_ADJECTIVELIST_NOT_CONFIGURED",
            Category = "Billing",
            Meaning = "Adjective List not configured for country id",
            CommonCause = "Adjective List not configured for country id",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153099",
            Name = "XONLINE_E_OFFERING_FAMILY_TO_CLASSIC_DOWNGRADE_NOT_ALLOWED",
            Category = "Marketplace",
            Meaning = "Family Gold users cannot purchase a Classic Gold subscription. They must wait for their subscription to expire to Silver before they can downgrade to Gold.",
            CommonCause = "Family Gold users cannot purchase a Classic Gold subscription. They must wait for their subscription to expire to Silver before they can downgrade to Gold.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015309A",
            Name = "XONLINE_E_BILLING_CTP_DIRECT_DEBIT_NOT_SUPPORTED",
            Category = "Network",
            Meaning = "Direct Debit operation like convert is not supported in Mega API",
            CommonCause = "Direct Debit operation like convert is not supported in Mega API",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015309B",
            Name = "XONLINE_E_OFFERING_FAMILY_DEPENDENT_PURCHASE_NOT_ALLOWED",
            Category = "Billing",
            Meaning = "Family Gold dependents cannot purchase classic gold or family gold subscriptions",
            CommonCause = "Family Gold dependents cannot purchase classic gold or family gold subscriptions",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015309C",
            Name = "XONLINE_E_OFFERING_FAMILY_CHILD_PURCHASE_NOT_ALLOWED",
            Category = "Billing",
            Meaning = "Child/Juvenille users cannot purchase family gold subscriptions",
            CommonCause = "Child/Juvenille users cannot purchase family gold subscriptions",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015309D",
            Name = "XONLINE_E_OFFERING_ALREADY_OWN_MAX_MACHINE",
            Category = "Marketplace",
            Meaning = "PPV offer has already been purchased within the past 24 hours on the same machine/device i.e. this user already owns the maximum allowed on the machine – see also XONLINE_E_OFFERING_ALREADY_OWN_MAX",
            CommonCause = "PPV offer has already been purchased within the past 24 hours on the same machine/device i.e. this user already owns the maximum allowed on the machine – see also XONLINE_E_OFFERING_ALREADY_OWN_MAX",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015309E",
            Name = "XONLINE_E_OFFERING_LICENSE_AVAILABLE",
            Category = "Marketplace",
            Meaning = "There is an existing license available that has not been downloaded and acknowledged for that offer – see also XONLINE_E_OFFERING_ALREADY_OWN_MAX",
            CommonCause = "There is an existing license available that has not been downloaded and acknowledged for that offer – see also XONLINE_E_OFFERING_ALREADY_OWN_MAX",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015309F",
            Name = "XONLINE_E_BILLING_CTP_INVALID_PRICE",
            Category = "Billing",
            Meaning = "Price passed from console does not match compute only",
            CommonCause = "Price passed from console does not match compute only",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x801530A0",
            Name = "XONLINE_E_BILLING_INVALID_TRACKING_GUID",
            Category = "Billing",
            Meaning = "Transaction id passed is not valid for this API or user",
            CommonCause = "Transaction id passed is not valid for this API or user",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x00153101",
            Name = "XONLINE_S_OFFERING_NEW_CONTENT",
            Category = "Success",
            Meaning = "Offering New Content",
            CommonCause = "Operation completed successfully or returned an informational status.",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x00153102",
            Name = "XONLINE_S_OFFERING_NO_NEW_CONTENT",
            Category = "Success",
            Meaning = "Offering No New Content",
            CommonCause = "Operation completed successfully or returned an informational status.",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80153200",
            Name = "XONLINE_E_DMP_TRANSACTION_CANCELLED",
            Category = "DMP",
            Meaning = "the DMP transaction was cancelled",
            CommonCause = "the DMP transaction was cancelled",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153201",
            Name = "XONLINE_E_DMP_CANT_GRANT_LICENSE",
            Category = "Marketplace",
            Meaning = "there was an error adding a row to t_licenses",
            CommonCause = "there was an error adding a row to t_licenses",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153202",
            Name = "XONLINE_E_DMP_CANT_REMOVE_LICENSE",
            Category = "Marketplace",
            Meaning = "there was an error removing a row from t_licenses",
            CommonCause = "there was an error removing a row from t_licenses",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153203",
            Name = "XONLINE_E_DMP_CANT_MARK_PURCHASE_CANCELLED",
            Category = "Billing",
            Meaning = "there was an error marking a DMP transaction as cancelled",
            CommonCause = "there was an error marking a DMP transaction as cancelled",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153204",
            Name = "XONLINE_E_DMP_CANCEL_SUCCESSFULL",
            Category = "DMP",
            Meaning = "the call to CancelPurchaseItem returned successfully",
            CommonCause = "the call to CancelPurchaseItem returned successfully",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153205",
            Name = "XONLINE_E_DMP_CANCEL_TRANS_NOT_FOUND",
            Category = "DMP",
            Meaning = "the call to CancelPurchaseItem returned “transaction not found”",
            CommonCause = "the call to CancelPurchaseItem returned “transaction not found”",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153206",
            Name = "XONLINE_E_DMP_CANT_MARK_PURCHASE_SUCCESS",
            Category = "Billing",
            Meaning = "there was an error marking a DMP transaction as successfull",
            CommonCause = "there was an error marking a DMP transaction as successfull",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153207",
            Name = "XONLINE_E_DMP_UNEXPECTED_STATE",
            Category = "DMP",
            Meaning = "the DMP transaction service encountered an unexpected state",
            CommonCause = "the DMP transaction service encountered an unexpected state",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153208",
            Name = "XONLINE_E_DMP_TRANSACTION_NOT_FOUND",
            Category = "DMP",
            Meaning = "the DMP transaction guid could not be found in the table",
            CommonCause = "the DMP transaction guid could not be found in the table",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x00153209",
            Name = "XONLINE_S_DMP_TRANSACTION_ALREADY_CANCELLED",
            Category = "Success",
            Meaning = "the DMP transaction has already been cancelled",
            CommonCause = "the DMP transaction has already been cancelled",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x0015320A",
            Name = "XONLINE_S_DMP_NO_CANCELABLE_TRANSACTIONS",
            Category = "Success",
            Meaning = "there are no DMP purchase transactions to be cancelled",
            CommonCause = "there are no DMP purchase transactions to be cancelled",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x8015320B",
            Name = "XONLINE_E_DMP_CANT_MARK_PURCHASE_CANCEL_FAILED",
            Category = "Billing",
            Meaning = "there was an error marking a DMP transaction as cancelFailed",
            CommonCause = "there was an error marking a DMP transaction as cancelFailed",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x0015320C",
            Name = "XONLINE_S_DMP_NO_RETRYABLE_REWARD",
            Category = "Success",
            Meaning = "there are no DMP reward transactions to be retriedg",
            CommonCause = "there are no DMP reward transactions to be retriedg",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x8015320D",
            Name = "XONLINE_E_DMP_INVALID_REWARD",
            Category = "DMP",
            Meaning = "the reward does not exist or is misconfigured",
            CommonCause = "the reward does not exist or is misconfigured",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x8015320E",
            Name = "XONLINE_E_DMP_SQL_TRANSACTION_FAILED",
            Category = "Database",
            Meaning = "a transaction failed to complete during a DMP operation",
            CommonCause = "a transaction failed to complete during a DMP operation",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x0015320F",
            Name = "XONLINE_S_DMP_RETRY_INTERVAL_EXCEEDED",
            Category = "Success",
            Meaning = "a transaction was moved more to failure state because the retry interval was exceeded",
            CommonCause = "a transaction was moved more to failure state because the retry interval was exceeded",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80153210",
            Name = "XONLINE_E_DMP_E_DELEGATE_NOT_SUPPORTED",
            Category = "Network",
            Meaning = "delegation not supported",
            CommonCause = "delegation not supported",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80153211",
            Name = "XONLINE_E_DMP_E_DESTINATION_ZERO",
            Category = "DMP",
            Meaning = "error in destination puid’s account",
            CommonCause = "error in destination puid’s account",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153212",
            Name = "XONLINE_E_DMP_E_INVALID_DESTINATION_ACCOUNT",
            Category = "DMP",
            Meaning = "destination billing account is invalid",
            CommonCause = "destination billing account is invalid",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153213",
            Name = "XONLINE_E_DMP_E_MAX_BALANCE_TRANSFER_PROVISIONAL_EXCEEDED",
            Category = "DMP",
            Meaning = "transfer maximum exceeded",
            CommonCause = "transfer maximum exceeded",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153214",
            Name = "XONLINE_E_DMP_E_SOURCE_TRANSFER_PUIDS_SAME",
            Category = "DMP",
            Meaning = "source and destination puids are the same",
            CommonCause = "source and destination puids are the same",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153215",
            Name = "XONLINE_E_DMP_E_SOURCE_ZERO",
            Category = "DMP",
            Meaning = "error in source puid’s account",
            CommonCause = "error in source puid’s account",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153216",
            Name = "XONLINE_E_DMP_E_ZERO_NEGTIVE_TRANSFER",
            Category = "DMP",
            Meaning = "error when source has zero points in account",
            CommonCause = "error when source has zero points in account",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153217",
            Name = "XONLINE_E_DMP_E_RISK_UNEXPECTED_RESULT",
            Category = "DMP",
            Meaning = "Unexpected error in Risk data",
            CommonCause = "Unexpected error in Risk data",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x00153300",
            Name = "XONLINE_S_NO_RETRYABLE_SCS_PURCHASES",
            Category = "Success",
            Meaning = "there are no retryable SCS::PurchaseItem transactions",
            CommonCause = "there are no retryable SCS::PurchaseItem transactions",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80153301",
            Name = "XONLINE_E_SCS_TRANSACTION_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "the SCS transaction guid could not be found in the table",
            CommonCause = "the SCS transaction guid could not be found in the table",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80153302",
            Name = "XONLINE_E_CONTENT_NOT_FOUND",
            Category = "Marketplace",
            Meaning = "Content not found",
            CommonCause = "Content not found",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153303",
            Name = "XONLINE_E_MACHINE_ID_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "Machine id not found",
            CommonCause = "Machine id not found",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80153400",
            Name = "XONLINE_E_DMP_E_STATUS_UNREGISTERED",
            Category = "DMP",
            Meaning = "User is not registered with DMP.",
            CommonCause = "User is not registered with DMP.",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153401",
            Name = "XONLINE_E_DMP_E_ORIGINAL_TRANSACTION_NOT_FOUND",
            Category = "DMP",
            Meaning = "The original record was not found, possibly archived",
            CommonCause = "The original record was not found, possibly archived",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153402",
            Name = "XONLINE_E_DMP_E_DUPLICATE_EXTERNAL_ORDER_ID",
            Category = "DMP",
            Meaning = "returned from OrderItems if a duplicate orderId was passed into wcmusic. Calling GetOrderByExternalOrderId should be called to get the updated status on that transaction, if needed. Otherwise a new externalOrderID needs to be generated.",
            CommonCause = "returned from OrderItems if a duplicate orderId was passed into wcmusic. Calling GetOrderByExternalOrderId should be called to get the updated status on that transaction, if needed. Otherwise a new externalOrderID needs to be generated.",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153410",
            Name = "XONLINE_E_DMP_E_UNKNOWN_ERROR",
            Category = "DMP",
            Meaning = "Generic DMP error. See server event log for specific details about what went wrong.",
            CommonCause = "Generic DMP error. See server event log for specific details about what went wrong.",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153411",
            Name = "XONLINE_E_DMP_E_REQUEST_CANNOT_BE_COMPLETED",
            Category = "DMP",
            Meaning = "The request cannot be completed due to user state. If the user is disabled then AddPromotionalBalance cannot be called upon that user.",
            CommonCause = "The request cannot be completed due to user state. If the user is disabled then AddPromotionalBalance cannot be called upon that user.",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153412",
            Name = "XONLINE_E_DMP_E_INSUFFICIENT_BALANCE",
            Category = "DMP",
            Meaning = "There is not sufficient balance to support this transaction",
            CommonCause = "There is not sufficient balance to support this transaction",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153413",
            Name = "XONLINE_E_DMP_E_MAX_BALANCE_EXCEEDED",
            Category = "DMP",
            Meaning = "The result point balance will exceed the policy max balance",
            CommonCause = "The result point balance will exceed the policy max balance",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153414",
            Name = "XONLINE_E_DMP_E_MAX_ACQUISITION_EXCEEDED",
            Category = "DMP",
            Meaning = "The point amount exceeds the policy max acquisition limit per transaction",
            CommonCause = "The point amount exceeds the policy max acquisition limit per transaction",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153415",
            Name = "XONLINE_E_DMP_E_MAX_CONSUMPTION_EXCEEDED",
            Category = "DMP",
            Meaning = "The user consumption per period of time would exceed the policy limit",
            CommonCause = "The user consumption per period of time would exceed the policy limit",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153416",
            Name = "XONLINE_E_DMP_E_NO_MORE_PROMO_POINTS",
            Category = "Billing",
            Meaning = "There are no more points to distribute for this tenant sku combination. All the promo points for this partner’s promotional SKU are gone already!",
            CommonCause = "There are no more points to distribute for this tenant sku combination. All the promo points for this partner’s promotional SKU are gone already!",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153417",
            Name = "XONLINE_E_DMP_E_PROMOTION_LIMIT_LIFE_MAX",
            Category = "DMP",
            Meaning = "The promotional points user obtained would exceed the sku limit. There is a configurable per user limit on each promotion. The partner sets it in offer modeling time",
            CommonCause = "The promotional points user obtained would exceed the sku limit. There is a configurable per user limit on each promotion. The partner sets it in offer modeling time",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153418",
            Name = "XONLINE_E_DMP_E_PROMO_EXPIRED",
            Category = "DMP",
            Meaning = "The promotional SKU has expired. The enddate for a promo is a configured limit set by the partner. If a request comes in after that configured deadline then the request is rejected",
            CommonCause = "The promotional SKU has expired. The enddate for a promo is a configured limit set by the partner. If a request comes in after that configured deadline then the request is rejected",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153419",
            Name = "XONLINE_E_DMP_E_MAX_ACQUISITIONPERSPAN_EXCEEDED",
            Category = "DMP",
            Meaning = "User attempt to acquire more points than is allowed in a given time span",
            CommonCause = "User attempt to acquire more points than is allowed in a given time span",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x8015341A",
            Name = "XONLINE_E_DMP_E_TARGETTRANSFER_INITIATED_WITHIN_NO_TRANSFER_WINDOW",
            Category = "DMP",
            Meaning = "TransferBalance initiated within the no transfer window for the target account",
            CommonCause = "TransferBalance initiated within the no transfer window for the target account",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x8015341B",
            Name = "XONLINE_E_DMP_E_COUNTRY_CODE_MISMATCH",
            Category = "DMP",
            Meaning = "TransferBalance failed because DMP source and destination accounts are in different countries",
            CommonCause = "TransferBalance failed because DMP source and destination accounts are in different countries",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x8015341C",
            Name = "XONLINE_E_DMP_E_USER_REGISTERED",
            Category = "DMP",
            Meaning = "RegisterUser called for user already registered with DMP",
            CommonCause = "RegisterUser called for user already registered with DMP",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x8015341D",
            Name = "XONLINE_E_DMP_E_UNKNOWNSERVER_ERROR",
            Category = "DMP",
            Meaning = "Unknown Server Error.",
            CommonCause = "Unknown Server Error.",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x8015341E",
            Name = "XONLINE_E_DMP_E_SYSTEM_INTERNAL_ERROR",
            Category = "DMP",
            Meaning = "A system internal error has occurred.",
            CommonCause = "A system internal error has occurred.",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x8015341F",
            Name = "XONLINE_E_DMP_E_INVALID_SOURCE_ACCOUNT",
            Category = "DMP",
            Meaning = "Source Account is not in Active State for the Transaction.",
            CommonCause = "Source Account is not in Active State for the Transaction.",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153420",
            Name = "XONLINE_E_DMP_E_USER_DISABLED",
            Category = "DMP",
            Meaning = "User is disabled.",
            CommonCause = "User is disabled.",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153421",
            Name = "XONLINE_E_DMP_E_PROMO_POINTS_UNAVAILIABLE",
            Category = "Billing",
            Meaning = "Promo points is unavailable",
            CommonCause = "Promo points is unavailable",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153422",
            Name = "XONLINE_E_DMP_E_INVALID_SKU",
            Category = "DMP",
            Meaning = "Sku passed for DMP purchase is invalid",
            CommonCause = "Sku passed for DMP purchase is invalid",
            FixSuggestion = "Check points/balance/transaction state and retry later if the service is queued or unavailable."
        },

        new()
        {
            Code = "0x80153500",
            Name = "XCBK_E_INVALID_SVC_COMPONENT",
            Category = "Validation",
            Meaning = "Invalid Svc Component",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80153501",
            Name = "XCBK_E_SUBSCRIPTION_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "Subscription Not Found",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80153502",
            Name = "XCBK_E_KEY_NOT_FOUND_IN_MESSAGE",
            Category = "Notification",
            Meaning = "Key Not Found In Message",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80153503",
            Name = "XCBK_E_SETTING_KEY_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "Setting Key Not Found",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80153504",
            Name = "XCBK_E_UNAUTHORIZED_REQUEST",
            Category = "Authentication",
            Meaning = "Unauthorized Request",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80153505",
            Name = "XCBK_E_UNKNOWN_ERROR",
            Category = "Xbox Live",
            Meaning = "Unknown Error",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80153506",
            Name = "XCBK_E_SUBSCRIPTION_ACCOUNT_MISMATCH",
            Category = "Accounts",
            Meaning = "Subscription Account Mismatch",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80153601",
            Name = "XONLINE_E_ORANGE_INVALID_REQUEST",
            Category = "Validation",
            Meaning = "Oranginvalid Request",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80153700",
            Name = "XONLINE_E_MUSICNET_BEGIN_ERROR_RANGE",
            Category = "MusicNet",
            Meaning = "marks beginning of musicnet-related error codes",
            CommonCause = "marks beginning of musicnet-related error codes",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80153700",
            Name = "XONLINE_E_MUSICNET_UNKNOWN_ERROR",
            Category = "MusicNet",
            Meaning = "Generic MusicNet error. See server event log for specific details about what went wrong.",
            CommonCause = "Generic MusicNet error. See server event log for specific details about what went wrong.",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x00153701",
            Name = "XONLINE_S_MUSICNET_NO_RETRYABLE_PURCHASE_ASSETS",
            Category = "Success",
            Meaning = "no retryable MusicNet.PurchaseAssets transactions were found",
            CommonCause = "no retryable MusicNet.PurchaseAssets transactions were found",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80153702",
            Name = "XONLINE_E_MUSICNET_TRANSACTION_NOT_FOUND",
            Category = "MusicNet",
            Meaning = "the transaction guid could not be found in the table",
            CommonCause = "the transaction guid could not be found in the table",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80153703",
            Name = "XONLINE_E_MUSICNET_ORDERITEMS_ITEMS_WITH_ERRORS",
            Category = "MusicNet",
            Meaning = "a musicnet.orderitems call returned a non-empty itemsWithErrors",
            CommonCause = "a musicnet.orderitems call returned a non-empty itemsWithErrors",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80153704",
            Name = "XONLINE_E_MUSICNET_INVALID_ORDER_ID",
            Category = "MusicNet",
            Meaning = "musicnet already has processed the transaction with the specified order id (209)",
            CommonCause = "musicnet already has processed the transaction with the specified order id (209)",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80153705",
            Name = "XONLINE_E_MUSICNET_ACCOUNT_SUSPENDED_OR_CLOSED",
            Category = "MusicNet",
            Meaning = "the musicnet account referenced has been suspended or is closed (424)",
            CommonCause = "the musicnet account referenced has been suspended or is closed (424)",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80153706",
            Name = "XONLINE_E_MUSICNET_ACCOUNT_ALREADY_SUBSCRIBER",
            Category = "MusicNet",
            Meaning = "the musicnet account already has an active subscription offer. (216)",
            CommonCause = "the musicnet account already has an active subscription offer. (216)",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80153707",
            Name = "XONLINE_E_MUSICNET_INVALID_CANCEL_REQUEST",
            Category = "MusicNet",
            Meaning = "Invalid cancelation request. This account is already canceled. (204)",
            CommonCause = "Invalid cancelation request. This account is already canceled. (204)",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80153708",
            Name = "XONLINE_E_MUSICNET_SUBSCRIPTION_ACCOUNT_CLOSED",
            Category = "MusicNet",
            Meaning = "Subscription account closed. (233)",
            CommonCause = "Subscription account closed. (233)",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80153709",
            Name = "XONLINE_E_MUSICNET_INVALID_SKU_NUMBER",
            Category = "MusicNet",
            Meaning = "Invalid SKU number. (207)",
            CommonCause = "Invalid SKU number. (207)",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x8015370A",
            Name = "XONLINE_E_MUSICNET_INVALID_TRANSACTION_DATE",
            Category = "MusicNet",
            Meaning = "Transaction date is in the future. Request denied. (206)",
            CommonCause = "Transaction date is in the future. Request denied. (206)",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x8015370B",
            Name = "XONLINE_E_MUSICNET_ACCOUNT_CANCELED",
            Category = "MusicNet",
            Meaning = "This account is canceled. Requested operation not allowed. (205)",
            CommonCause = "This account is canceled. Requested operation not allowed. (205)",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x8015370C",
            Name = "XONLINE_E_MUSICNET_INVALID_RESUME_REQUEST",
            Category = "MusicNet",
            Meaning = "Invalid resume request. This account is already active. (203)",
            CommonCause = "Invalid resume request. This account is already active. (203)",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x8015370D",
            Name = "XONLINE_E_MUSICNET_CANNOT_CHANGE_OFFER",
            Category = "Marketplace",
            Meaning = "Cannot change to an offer that is the same as the current offer. (202)",
            CommonCause = "Cannot change to an offer that is the same as the current offer. (202)",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015370E",
            Name = "XONLINE_E_MUSICNET_ACCOUNT_HAS_EXPIRED_TRIAL",
            Category = "MusicNet",
            Meaning = "This account has an expired trial. Cannot change the queued offer. (201)",
            CommonCause = "This account has an expired trial. Cannot change the queued offer. (201)",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x8015370F",
            Name = "XONLINE_E_MUSICNET_ACCOUNT_ALREADY_HAS_TRIAL",
            Category = "MusicNet",
            Meaning = "Already have a trial. (228)",
            CommonCause = "Already have a trial. (228)",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80153710",
            Name = "XONLINE_E_MUSICNET_TRIAL_OFFER_EXPIRED",
            Category = "Marketplace",
            Meaning = "Trial offer has expired. (210)",
            CommonCause = "Trial offer has expired. (210)",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153711",
            Name = "XONLINE_E_MUSICNET_MAXIMUM_PURCHASE_COMPONENTS_EXCEEDED",
            Category = "Billing",
            Meaning = "Maximum number of purchase components per order exceeded. (229)",
            CommonCause = "Maximum number of purchase components per order exceeded. (229)",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153712",
            Name = "XONLINE_E_MUSICNET_DUPLICATE_USER_NAME",
            Category = "MusicNet",
            Meaning = "Duplicate user name found (25)",
            CommonCause = "Duplicate user name found (25)",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80153713",
            Name = "XONLINE_E_MUSICNET_INSUFFICIENT_FREE_TRACKS",
            Category = "MusicNet",
            Meaning = "Insufficient free tracks remain to complete purchase (432)",
            CommonCause = "Insufficient free tracks remain to complete purchase (432)",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80153714",
            Name = "XONLINE_E_MUSICNET_ACCOUNT_ALREADY_HAS_QUEUED_OFFER",
            Category = "Marketplace",
            Meaning = "Account already has a queued subscription offer (212)",
            CommonCause = "Account already has a queued subscription offer (212)",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153715",
            Name = "XONLINE_E_MUSICNET_INVALID_SUBSCRIPTION_FREE_PURCHASE",
            Category = "Billing",
            Meaning = "Invalid subscription free purchase type(434)",
            CommonCause = "Invalid subscription free purchase type(434)",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153716",
            Name = "XONLINE_E_MUSICNET_CONTENT_RIGHT_UNAVAILABLE",
            Category = "Marketplace",
            Meaning = "Content right not available for requested component(312)",
            CommonCause = "Content right not available for requested component(312)",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x801537FF",
            Name = "XONLINE_E_MUSICNET_END_ERROR_RANGE",
            Category = "MusicNet",
            Meaning = "marks end of musicnet-related error codes",
            CommonCause = "marks end of musicnet-related error codes",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80153800",
            Name = "XONLINE_E_WMIS_UNKNOWN_ERROR",
            Category = "Media",
            Meaning = "Generic WMIS error. See server event log for specific details about what went wrong.",
            CommonCause = "Generic WMIS error. See server event log for specific details about what went wrong.",
            FixSuggestion = "Verify media license, device authorization, purchase status, and retry playback or license refresh."
        },

        new()
        {
            Code = "0x80153801",
            Name = "XONLINE_E_WMIS_PURCHASE_DETAILS_NULL",
            Category = "Billing",
            Meaning = "WMIS is returning null on a GetVideoPurchaseDetails call.",
            CommonCause = "WMIS is returning null on a GetVideoPurchaseDetails call.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80153802",
            Name = "XONLINE_E_WMIS_UNKNOWN_ERROR_CONSOLE",
            Category = "Media",
            Meaning = "Generic WMIS error on the console side. The server should never return this error.",
            CommonCause = "Generic WMIS error on the console side. The server should never return this error.",
            FixSuggestion = "Verify media license, device authorization, purchase status, and retry playback or license refresh."
        },

        new()
        {
            Code = "0x80153803",
            Name = "DEPRECATED_XONLINE_E_WMIS_EMPTY_TITLE_TEXT",
            Category = "Media",
            Meaning = "WMIS GetVideoPurchaseDetails returned an invalid empty title text field.",
            CommonCause = "WMIS GetVideoPurchaseDetails returned an invalid empty title text field.",
            FixSuggestion = "Verify media license, device authorization, purchase status, and retry playback or license refresh."
        },

        new()
        {
            Code = "0x80153900",
            Name = "DEPRECATED_XONLINE_E_SYNCCAST_UNKNOWN_ERROR",
            Category = "Media",
            Meaning = "Generic SyncCast error. See server event log for specific details about what went wrong.",
            CommonCause = "Generic SyncCast error. See server event log for specific details about what went wrong.",
            FixSuggestion = "Verify media license, device authorization, purchase status, and retry playback or license refresh."
        },

        new()
        {
            Code = "0x80153901",
            Name = "DEPRECATED_XONLINE_E_SYNCCAST_LICENSE_TOO_LARGE",
            Category = "Marketplace",
            Meaning = "SyncCast has returned a license too large to return to the client.",
            CommonCause = "SyncCast has returned a license too large to return to the client.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153902",
            Name = "DEPRECATED_XONLINE_E_SYNCCAST_LICENSE_EMPTY",
            Category = "Marketplace",
            Meaning = "SyncCast has returned an empty license when an exception was expected",
            CommonCause = "SyncCast has returned an empty license when an exception was expected",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153903",
            Name = "DEPRECATED_XONLINE_E_SYNCCAST_CLIENT_CERT_INVALID",
            Category = "Media",
            Meaning = "Client cert is missing or invalid",
            CommonCause = "Client cert is missing or invalid",
            FixSuggestion = "Verify media license, device authorization, purchase status, and retry playback or license refresh."
        },

        new()
        {
            Code = "0x80153904",
            Name = "DEPRECATED_XONLINE_E_SYNCCAST_CLIENT_CERT_ACCESS_DENIED",
            Category = "Media",
            Meaning = "Client cert access denied on the web service or the offer",
            CommonCause = "Client cert access denied on the web service or the offer",
            FixSuggestion = "Verify media license, device authorization, purchase status, and retry playback or license refresh."
        },

        new()
        {
            Code = "0x80153905",
            Name = "DEPRECATED_XONLINE_E_SYNCCAST_INVALID_OFFER_ACTION",
            Category = "Marketplace",
            Meaning = "An invalid action was requested for the given offer",
            CommonCause = "An invalid action was requested for the given offer",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153906",
            Name = "DEPRECATED_XONLINE_E_SYNCCAST_REACHED_COUNTER_LIMIT",
            Category = "Media",
            Meaning = "Attempted to fulfill a request beyond the counter limit",
            CommonCause = "Attempted to fulfill a request beyond the counter limit",
            FixSuggestion = "Verify media license, device authorization, purchase status, and retry playback or license refresh."
        },

        new()
        {
            Code = "0x80153907",
            Name = "DEPRECATED_XONLINE_E_SYNCCAST_REACHED_EXPIRATION_DATE",
            Category = "Media",
            Meaning = "Attempted to fulfill a request beyond the expiration date",
            CommonCause = "Attempted to fulfill a request beyond the expiration date",
            FixSuggestion = "Verify media license, device authorization, purchase status, and retry playback or license refresh."
        },

        new()
        {
            Code = "0x80153908",
            Name = "DEPRECATED_XONLINE_E_SYNCCAST_EMPTY_DEVICE_NAME",
            Category = "Media",
            Meaning = "An empty device name was provided",
            CommonCause = "An empty device name was provided",
            FixSuggestion = "Verify media license, device authorization, purchase status, and retry playback or license refresh."
        },

        new()
        {
            Code = "0x80153A00",
            Name = "DEPRECATED_XONLINE_E_SYNCCAST_PLAYER_APP_REVOKED",
            Category = "Media",
            Meaning = "The player application has been revoked",
            CommonCause = "The player application has been revoked",
            FixSuggestion = "Verify media license, device authorization, purchase status, and retry playback or license refresh."
        },

        new()
        {
            Code = "0x80153A01",
            Name = "DEPRECATED_XONLINE_E_SYNCCAST_PLAYER_APP_SECURITY_UPGRADE",
            Category = "Media",
            Meaning = "A security upgrade id required for the player application",
            CommonCause = "A security upgrade id required for the player application",
            FixSuggestion = "Verify media license, device authorization, purchase status, and retry playback or license refresh."
        },

        new()
        {
            Code = "0x80153A02",
            Name = "DEPRECATED_XONLINE_E_SYNCCAST_DEVICE_REVOKED",
            Category = "Media",
            Meaning = "The device has been revoked",
            CommonCause = "The device has been revoked",
            FixSuggestion = "Verify media license, device authorization, purchase status, and retry playback or license refresh."
        },

        new()
        {
            Code = "0x80153B01",
            Name = "DEPRECATED_XONLINE_E_NO_VALID_LICENSE",
            Category = "Marketplace",
            Meaning = "No license was available to acquire.",
            CommonCause = "No license was available to acquire.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153B02",
            Name = "DEPRECATED_XONLINE_E_VOD_LICENSE_EXPIRED",
            Category = "Marketplace",
            Meaning = "Could not issue license as the VOD license acquisition window has closed",
            CommonCause = "Could not issue license as the VOD license acquisition window has closed",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80153B03",
            Name = "DEPRECATED_XONLINE_E_NO_SCOID",
            Category = "Xbox Live",
            Meaning = "Could not retrieve SyncCast OfferId from WMIS",
            CommonCause = "Could not retrieve SyncCast OfferId from WMIS",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80153B04",
            Name = "DEPRECATED_XONLINE_E_VOD_INVALID_MACHINE",
            Category = "Media",
            Meaning = "Invalid machine. Can only issue licenses to the purchasing machine.",
            CommonCause = "Invalid machine. Can only issue licenses to the purchasing machine.",
            FixSuggestion = "Verify media license, device authorization, purchase status, and retry playback or license refresh."
        },

        new()
        {
            Code = "0x80153B05",
            Name = "DEPRECATED_XONLINE_E_VOD_LICENSE_DELIVERED",
            Category = "Marketplace",
            Meaning = "License has already been delivered and acknowledged",
            CommonCause = "License has already been delivered and acknowledged",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x00153C00",
            Name = "XONLINE_S_FINBUS_NO_RETRYABLE_TRANSACTIONS",
            Category = "Success",
            Meaning = "there are no retryable FinBus transactions in the t_finbus_transactions table",
            CommonCause = "there are no retryable FinBus transactions in the t_finbus_transactions table",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80153C01",
            Name = "XONLINE_E_FINBUS_TRANSACTION_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "the FinBus transation does not exist in the t_finbus_transactions table",
            CommonCause = "the FinBus transation does not exist in the t_finbus_transactions table",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80153C02",
            Name = "XONLINE_E_FINBUS_SQL_TRANSACTION_FAILED",
            Category = "Database",
            Meaning = "a SQL transaction using the t_finbus_transactions_table failed",
            CommonCause = "a SQL transaction using the t_finbus_transactions_table failed",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x80153C03",
            Name = "XONLINE_E_FINBUS_REDEMPTION_TRANSACTION_FAILED",
            Category = "Xbox Live",
            Meaning = "there was an error updating the FinBus item’s status to Pending from PrePending",
            CommonCause = "there was an error updating the FinBus item’s status to Pending from PrePending",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x00153C04",
            Name = "XONLINE_S_FINBUS_RETRY_INTERVAL_EXCEEDED",
            Category = "Success",
            Meaning = "a FinBus transaction was moved more to failure state becuase the retry interval was exceeded",
            CommonCause = "a FinBus transaction was moved more to failure state becuase the retry interval was exceeded",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80153C05",
            Name = "XONLINE_E_FINBUS_HEALTH_CHECK_ERROR",
            Category = "Xbox Live",
            Meaning = "Error calling the finbus health check",
            CommonCause = "Error calling the finbus health check",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80153C06",
            Name = "XONLINE_E_FINBUS_FAST_FAIL_ERROR",
            Category = "Xbox Live",
            Meaning = "Fast fail error when calling FinBus endpoint",
            CommonCause = "Fast fail error when calling FinBus endpoint",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80153C07",
            Name = "XONLINE_E_FINBUS_PUBLISH_MESSAGE_ERROR",
            Category = "Notification",
            Meaning = "non-specific (catch all) FinBus PublishMessage error",
            CommonCause = "non-specific (catch all) FinBus PublishMessage error",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80153C08",
            Name = "XONLINE_E_FINBUS_IS_SERVICE_ALIVE_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch all) FinBus IsServiceAlive error",
            CommonCause = "non-specific (catch all) FinBus IsServiceAlive error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80153D00",
            Name = "XONLINE_E_TRACKED_API_DUPLICATE_TRACKING_GUID",
            Category = "Xbox Live",
            Meaning = "duplicate tracking guid found",
            CommonCause = "duplicate tracking guid found",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80153D01",
            Name = "XONLINE_E_TRACKED_API_DATABASE_ERROR",
            Category = "Database",
            Meaning = "error recording tracked api to database",
            CommonCause = "error recording tracked api to database",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x00000010",
            Name = "XONLINE_S_ACCOUNTS_NAME_TAKEN",
            Category = "Success",
            Meaning = "Accountnamtaken",
            CommonCause = "Operation completed successfully or returned an informational status.",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80154000",
            Name = "XONLINE_E_ACCOUNTS_NAME_TAKEN",
            Category = "Accounts",
            Meaning = "Accountnamtaken",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154001",
            Name = "XONLINE_E_ACCOUNTS_INVALID_KINGDOM",
            Category = "Accounts",
            Meaning = "Accountinvalid Kingdom",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154002",
            Name = "XONLINE_E_ACCOUNTS_INVALID_USER",
            Category = "Accounts",
            Meaning = "Accountinvalid User",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154003",
            Name = "XONLINE_E_ACCOUNTS_BAD_CREDIT_CARD",
            Category = "Billing",
            Meaning = "Accountbad Credit Card",
            CommonCause = "Payment method, purchase state, billing provider, points balance, or transaction issue.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80154004",
            Name = "XONLINE_E_ACCOUNTS_BAD_BILLING_ADDRESS",
            Category = "Billing",
            Meaning = "Accountbad Billing Address",
            CommonCause = "Payment method, purchase state, billing provider, points balance, or transaction issue.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80154005",
            Name = "XONLINE_E_ACCOUNTS_ACCOUNT_BANNED",
            Category = "Accounts",
            Meaning = "Accountaccount Banned",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154006",
            Name = "XONLINE_E_ACCOUNTS_PERMISSION_DENIED",
            Category = "Accounts",
            Meaning = "Accountpermission Denied",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154007",
            Name = "XONLINE_E_ACCOUNTS_INVALID_VOUCHER",
            Category = "Accounts",
            Meaning = "Accountinvalid Voucher",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154008",
            Name = "XONLINE_E_ACCOUNTS_DATA_CHANGED",
            Category = "Accounts",
            Meaning = "unexpected modifications made during request. commit is aborted to avoid overwriting modifcations.",
            CommonCause = "unexpected modifications made during request. commit is aborted to avoid overwriting modifcations.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154009",
            Name = "XONLINE_E_ACCOUNTS_VOUCHER_ALREADY_USED",
            Category = "Accounts",
            Meaning = "Accountvoucher Already Used",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015400A",
            Name = "XONLINE_E_ACCOUNTS_OPERATION_BLOCKED",
            Category = "Accounts",
            Meaning = "Accountoperation Blocked",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015400B",
            Name = "XONLINE_E_ACCOUNTS_POSTAL_CODE_REQUIRED",
            Category = "Accounts",
            Meaning = "Accountpostal Codrequired",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015400C",
            Name = "XONLINE_E_ACCOUNTS_TRY_AGAIN_LATER",
            Category = "Accounts",
            Meaning = "Accounttry Again Later",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015400D",
            Name = "XONLINE_E_ACCOUNTS_NOT_A_RENEWAL_OFFER",
            Category = "Marketplace",
            Meaning = "Accountnot A Renewal Offer",
            CommonCause = "Offer, license, entitlement, content availability, or region restriction issue.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015400E",
            Name = "XONLINE_E_ACCOUNTS_RENEWAL_IS_LOCKED",
            Category = "Accounts",
            Meaning = "Accountrenewal Ilocked",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015400F",
            Name = "XONLINE_E_ACCOUNTS_VOUCHER_REQUIRED",
            Category = "Accounts",
            Meaning = "Accountvoucher Required",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154010",
            Name = "XONLINE_E_ACCOUNTS_ALREADY_DEPROVISIONED",
            Category = "Accounts",
            Meaning = "Accountalready Deprovisioned",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154011",
            Name = "XONLINE_E_ACCOUNTS_INVALID_PRIVILEGE",
            Category = "Accounts",
            Meaning = "Accountinvalid Privilege",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154012",
            Name = "XONLINE_E_ACCOUNTS_INVALID_SIGNED_PASSPORT_PUID",
            Category = "Network",
            Meaning = "Accountinvalid Signed Passport Puid",
            CommonCause = "DNS, NAT, gateway, Xbox Live endpoint, or local network connectivity issue.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154013",
            Name = "XONLINE_E_ACCOUNTS_PASSPORT_ALREADY_LINKED",
            Category = "Network",
            Meaning = "Accountpassport Already Linked",
            CommonCause = "DNS, NAT, gateway, Xbox Live endpoint, or local network connectivity issue.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154014",
            Name = "XONLINE_E_ACCOUNTS_MIGRATE_NOT_XBOX1_USER",
            Category = "Accounts",
            Meaning = "Accountmigratnot Xbox1 User",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154015",
            Name = "XONLINE_E_ACCOUNTS_MIGRATE_BAD_SUBSCRIPTION",
            Category = "Accounts",
            Meaning = "Accountmigratbad Subscription",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154016",
            Name = "XONLINE_E_ACCOUNTS_PASSPORT_NOT_LINKED",
            Category = "Network",
            Meaning = "Accountpassport Not Linked",
            CommonCause = "DNS, NAT, gateway, Xbox Live endpoint, or local network connectivity issue.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154017",
            Name = "XONLINE_E_ACCOUNTS_NOT_XENON_USER",
            Category = "Accounts",
            Meaning = "Accountnot Xenon User",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154018",
            Name = "XONLINE_E_ACCOUNTS_CREDIT_CARD_REQUIRED",
            Category = "Billing",
            Meaning = "Accountcredit Card Required",
            CommonCause = "Payment method, purchase state, billing provider, points balance, or transaction issue.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80154019",
            Name = "XONLINE_E_ACCOUNTS_MIGRATE_NOT_XBOXCOM_USER",
            Category = "Accounts",
            Meaning = "Accountmigratnot Xboxcom User",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015401A",
            Name = "XONLINE_E_ACCOUNTS_NOT_A_VOUCHER_OFFER",
            Category = "Marketplace",
            Meaning = "Accountnot A Voucher Offer",
            CommonCause = "Offer, license, entitlement, content availability, or region restriction issue.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015401B",
            Name = "XONLINE_E_ACCOUNTS_REACHED_TRIAL_OFFER_LIMIT",
            Category = "Marketplace",
            Meaning = "Can’t use trial offer because the limit for this console has already been reached",
            CommonCause = "Can’t use trial offer because the limit for this console has already been reached",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015401C",
            Name = "XONLINE_E_ACCOUNTS_XBOX1_MANAGEMENT_BLOCKED",
            Category = "Accounts",
            Meaning = "A Xenon user is not allowed to access certain features (such as account management) from an xbox1 console",
            CommonCause = "A Xenon user is not allowed to access certain features (such as account management) from an xbox1 console",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015401D",
            Name = "XONLINE_E_ACCOUNTS_OFFLINE_XUID_ALREADY_USED",
            Category = "Accounts",
            Meaning = "The provided offline xuid has already been used to create an account",
            CommonCause = "The provided offline xuid has already been used to create an account",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015401E",
            Name = "XONLINE_E_ACCOUNTS_BILLING_PROVIDER_TIMEOUT",
            Category = "Billing",
            Meaning = "The billing provider operation timed out",
            CommonCause = "The billing provider operation timed out",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015401F",
            Name = "XONLINE_E_ACCOUNTS_MIGRATION_OFFER_NOT_FOUND",
            Category = "Marketplace",
            Meaning = "The billing offer id for the Xbox1 migration offer was not found",
            CommonCause = "The billing offer id for the Xbox1 migration offer was not found",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80154020",
            Name = "XONLINE_E_ACCOUNTS_UNDER_AGE",
            Category = "Accounts",
            Meaning = "Request cannot be processed because user is under-age.",
            CommonCause = "Request cannot be processed because user is under-age.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154021",
            Name = "XONLINE_E_ACCOUNTS_XBOX1_LOGON_BLOCKED",
            Category = "Authentication",
            Meaning = "The user account is restricted from signing on with Xbox1 titles",
            CommonCause = "The user account is restricted from signing on with Xbox1 titles",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154022",
            Name = "XONLINE_E_ACCOUNTS_VOUCHER_INVALID_FOR_TIER",
            Category = "Accounts",
            Meaning = "The voucher supplied is valid but not for the user’s tier",
            CommonCause = "The voucher supplied is valid but not for the user’s tier",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154023",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_PASSPORT_QUEUED",
            Category = "Network",
            Meaning = "The SwitchUserPassport operation was interrupted due to SCS or DB error and will be retried",
            CommonCause = "The SwitchUserPassport operation was interrupted due to SCS or DB error and will be retried",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154024",
            Name = "XONLINE_E_ACCOUNTS_SERVICE_NOT_PROVISIONED",
            Category = "Accounts",
            Meaning = "The user account is not provisioned for this service type",
            CommonCause = "The user account is not provisioned for this service type",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154025",
            Name = "XONLINE_E_ACCOUNTS_ACCOUNT_UNBAN_BLOCKED",
            Category = "Accounts",
            Meaning = "The user account has been permantely banned and cannot be revoked by a CUST tool",
            CommonCause = "The user account has been permantely banned and cannot be revoked by a CUST tool",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154026",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_PASSPORT_INELIGIBLE",
            Category = "Network",
            Meaning = "The user has switched passports less than 30 days ago",
            CommonCause = "The user has switched passports less than 30 days ago",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154027",
            Name = "XONLINE_E_ACCOUNTS_ADDITIONAL_DATA_REQUIRED",
            Category = "Accounts",
            Meaning = "The user has not provided address or phone information for XeSetAccountInfo",
            CommonCause = "The user has not provided address or phone information for XeSetAccountInfo",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154028",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_PASSPORT_SCS_PENDING",
            Category = "Network",
            Meaning = "The user has a pending SCS points purchase request",
            CommonCause = "The user has a pending SCS points purchase request",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154029",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_PASSPORT_NO_BIRTHDATE",
            Category = "Network",
            Meaning = "The user has no birthdate present in their Passport profile",
            CommonCause = "The user has no birthdate present in their Passport profile",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015102A",
            Name = "XONLINE_E_ACCOUNTS_GRADUATE_USER_NO_PRIVILEGE",
            Category = "Accounts",
            Meaning = "User does not have privilege to graduate",
            CommonCause = "User does not have privilege to graduate",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015102B",
            Name = "XONLINE_E_ACCOUNTS_GRADUATE_USER_NOT_CHILD",
            Category = "Accounts",
            Meaning = "User does not have a child account",
            CommonCause = "User does not have a child account",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015102C",
            Name = "XONLINE_E_ACCOUNTS_GRADUATE_USER_NOT_ADULT",
            Category = "Accounts",
            Meaning = "User is not an adult (in their country)",
            CommonCause = "User is not an adult (in their country)",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015102D",
            Name = "XONLINE_E_ACCOUNTS_GRADUATE_USER_NO_PI",
            Category = "Accounts",
            Meaning = "Client didn’t specify payment instrument when one was required",
            CommonCause = "Client didn’t specify payment instrument when one was required",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015102E",
            Name = "XONLINE_E_ACCOUNTS_GRADUATE_USER_PI_MISMATCH",
            Category = "Accounts",
            Meaning = "User supplied existing payment instrument but personal information does not match",
            CommonCause = "User supplied existing payment instrument but personal information does not match",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015102F",
            Name = "XONLINE_E_ACCOUNTS_GRADUATE_USER_ALREADY",
            Category = "Accounts",
            Meaning = "User supplied is already graduated",
            CommonCause = "User supplied is already graduated",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154030",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_PASSPORT_ADULT_TO_CHILD",
            Category = "Network",
            Meaning = "The user has an adult account but is trying to switch to a juvenile Passport",
            CommonCause = "The user has an adult account but is trying to switch to a juvenile Passport",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80151031",
            Name = "XONLINE_E_ACCOUNTS_GRADUATE_USER_QUEUED",
            Category = "Accounts",
            Meaning = "Graduation of the user was queued",
            CommonCause = "Graduation of the user was queued",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80151032",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_PASSPORT_NEW_PASSPORT_INELIGIBLE",
            Category = "Network",
            Meaning = "Cannot switch ownership of the specified account",
            CommonCause = "Cannot switch ownership of the specified account",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154033",
            Name = "XONLINE_E_ACCOUNTS_NO_AUTHENTICATION_DATA",
            Category = "Authentication",
            Meaning = "no authentication data was provided",
            CommonCause = "no authentication data was provided",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154034",
            Name = "XONLINE_E_ACCOUNTS_CLIENT_TYPE_CONFIG_ERROR",
            Category = "Accounts",
            Meaning = "npdb configuration of client types in t_multisettings is invalid",
            CommonCause = "npdb configuration of client types in t_multisettings is invalid",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154035",
            Name = "XONLINE_E_ACCOUNTS_CLIENT_TYPE_MISSING",
            Category = "Accounts",
            Meaning = "client type is missing, was not provided",
            CommonCause = "client type is missing, was not provided",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154036",
            Name = "XONLINE_E_ACCOUNTS_CLIENT_TYPE_INVALID",
            Category = "Accounts",
            Meaning = "client type provided is invalid",
            CommonCause = "client type provided is invalid",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154037",
            Name = "XONLINE_E_ACCOUNTS_COUNTRY_NOT_AUTHORIZED",
            Category = "Authentication",
            Meaning = "service type / client type combination is not authorized for specified country",
            CommonCause = "service type / client type combination is not authorized for specified country",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154038",
            Name = "XONLINE_E_ACCOUNTS_TAG_CHANGE_REQUIRED",
            Category = "Accounts",
            Meaning = "account is required to change their gamertag",
            CommonCause = "account is required to change their gamertag",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154039",
            Name = "XONLINE_E_ACCOUNTS_ACCOUNT_SUSPENDED",
            Category = "Accounts",
            Meaning = "account is otherwise disabled, banned, suspended, etc. and requires management",
            CommonCause = "account is otherwise disabled, banned, suspended, etc. and requires management",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015403A",
            Name = "XONLINE_E_ACCOUNTS_TERMS_OF_SERVICE_NOT_ACCEPTED",
            Category = "Accounts",
            Meaning = "account is otherwise disabled, banned, suspended, etc. and requires management",
            CommonCause = "account is otherwise disabled, banned, suspended, etc. and requires management",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015403B",
            Name = "XONLINE_E_ACCOUNTS_SET_NO_AGE_OUT_QUEUED",
            Category = "Accounts",
            Meaning = "set passport no age out flag operation has been queued",
            CommonCause = "set passport no age out flag operation has been queued",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015403C",
            Name = "XONLINE_E_BILLING_USERACCOUNT_USER_NOT_FOUND",
            Category = "Billing",
            Meaning = "User not found in GetANID call.",
            CommonCause = "User not found in GetANID call.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015403D",
            Name = "XONLINE_E_BILLING_USERACCOUNT_INVALID_CLIENT",
            Category = "Billing",
            Meaning = "Invalid client in GetANID call.",
            CommonCause = "Invalid client in GetANID call.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015403E",
            Name = "XONLINE_E_BILLING_USERACCOUNT_XUID_DOES_NOT_MATCH_USER",
            Category = "Billing",
            Meaning = "Xuid does not match user in GetANID call.",
            CommonCause = "Xuid does not match user in GetANID call.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015403F",
            Name = "XONLINE_E_ACCOUNTS_MIGRATION_ERROR",
            Category = "Accounts",
            Meaning = "unspecified error migrating an XBOX 1 offer",
            CommonCause = "unspecified error migrating an XBOX 1 offer",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154040",
            Name = "XONLINE_E_ACCOUNTS_PUID_TO_ANID_ERROR",
            Category = "Accounts",
            Meaning = "unspecified error converting puid to anid",
            CommonCause = "unspecified error converting puid to anid",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154041",
            Name = "XONLINE_E_ACCOUNTS_PASSPORT_LOAD_USER_ERROR",
            Category = "Network",
            Meaning = "Error loading user by pasport puid",
            CommonCause = "Error loading user by pasport puid",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154042",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_PASSPORT_ERROR",
            Category = "Network",
            Meaning = "unspecified error switching user passport",
            CommonCause = "unspecified error switching user passport",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154043",
            Name = "XONLINE_E_ACCOUNTS_OFFLINE_XUID",
            Category = "Accounts",
            Meaning = "offline xuid prevents operation",
            CommonCause = "offline xuid prevents operation",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154044",
            Name = "XONLINE_E_ACCOUNTS_RECOVER_ACCOUNT_ERROR",
            Category = "Accounts",
            Meaning = "unspecified error recovering account",
            CommonCause = "unspecified error recovering account",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154045",
            Name = "XONLINE_E_ACCOUNTS_RENEWAL_ERROR",
            Category = "Accounts",
            Meaning = "unspecified subscription renewal error",
            CommonCause = "unspecified subscription renewal error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154046",
            Name = "XONLINE_E_BILLING_USERACCOUNT_CONTACTLIST_GAMERTAGS_ERROR",
            Category = "Billing",
            Meaning = "Failed to talk to ABCH.",
            CommonCause = "Failed to talk to ABCH.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80154047",
            Name = "XONLINE_E_ACCOUNTS_PUID_TO_ANID_FAILED",
            Category = "Accounts",
            Meaning = "Couldn’t transform a puid to an anid.",
            CommonCause = "Couldn’t transform a puid to an anid.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154048",
            Name = "XONLINE_E_ACCOUNTS_UPDATE_XBOX_COM_ACTIVITY_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154059",
            Name = "XONLINE_E_ACCOUNTS_UPS_GET_PROFILE_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015404A",
            Name = "XONLINE_E_ACCOUNTS_UPS_UPDATE_PROFILE_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015404B",
            Name = "XONLINE_E_ACCOUNTS_SET_ACCOUNT_INFO_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015404C",
            Name = "XONLINE_E_ACCOUNTS_ADD_PAYMENT_INSTRUMENT_ERROR",
            Category = "Billing",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015404D",
            Name = "XONLINE_E_ACCOUNTS_PASSPORT_GET_ENCRYPTED_PROXY_PARAMETERS_ERROR",
            Category = "Network",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015404E",
            Name = "XONLINE_E_ACCOUNTS_PASSPORT_GET_FRIEND_MEMBER_NAME_ERROR",
            Category = "Network",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015404F",
            Name = "XONLINE_E_ACCOUNTS_PASSPORT_GET_PUID_FROM_MEMBER_NAME_ERROR",
            Category = "Network",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154050",
            Name = "XONLINE_E_ACCOUNTS_PASSPORT_GET_SECRET_QUESTIONS_ERROR",
            Category = "Network",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154051",
            Name = "XONLINE_E_ACCOUNTS_PASSPORT_LOGIN_ERROR",
            Category = "Network",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154052",
            Name = "XONLINE_E_ACCOUNTS_TERMS_OF_SERVICE_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154053",
            Name = "XONLINE_E_ACCOUNTS_GET_ACCOUNT_STATUS_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154054",
            Name = "XONLINE_E_ACCOUNTS_ARGO_SIGN_IN_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154055",
            Name = "XONLINE_E_ACCOUNTS_ARGO_AUTHENTICATE_ACCOUNT_ERROR",
            Category = "Authentication",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154056",
            Name = "XONLINE_E_ACCOUNTS_CHANGE_SUBSCRIPTION_PAYMENT_INSTRUMENT_ERROR",
            Category = "Billing",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80154057",
            Name = "XONLINE_E_ACCOUNTS_CLEAR_SUBSCRIPTIONS_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154058",
            Name = "XONLINE_E_ACCOUNTS_CREATE_ACCOUNT_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154059",
            Name = "XONLINE_E_ACCOUNTS_DISABLE_SUBSCRIPTION_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015405A",
            Name = "XONLINE_E_ACCOUNTS_ENUMERATE_ELIGIBLE_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015405B",
            Name = "XONLINE_E_ACCOUNTS_GET_ANID_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015405C",
            Name = "XONLINE_E_ACCOUNTS_GET_LINKED_GAMERTAG_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015405D",
            Name = "XONLINE_E_ACCOUNTS_GET_NO_AGE_OUT_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015405E",
            Name = "XONLINE_E_ACCOUNTS_GET_PAYMNET_INFO_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015405F",
            Name = "XONLINE_E_ACCOUNTS_GET_PAYMENT_INSTRUMENTS_ERROR",
            Category = "Billing",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80154060",
            Name = "XONLINE_E_ACCOUNTS_GET_POSTAL_CODE_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154061",
            Name = "XONLINE_E_ACCOUNTS_GET_SUBSCRIPTION_STATUS_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154062",
            Name = "XONLINE_E_ACCOUNTS_GET_USER_INFO_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154063",
            Name = "XONLINE_E_ACCOUNTS_GRADUATE_USER_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154064",
            Name = "XONLINE_E_ACCOUNTS_LINK_ACCOUNT_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154065",
            Name = "XONLINE_E_ACCOUNTS_MIGRATE_USER_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154066",
            Name = "XONLINE_E_ACCOUNTS_PASSPORT_GET_USER_DATA_ERROR",
            Category = "Network",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154067",
            Name = "XONLINE_E_ACCOUNTS_REMOVE_PAYMENT_INSTRUMENT_ERROR",
            Category = "Billing",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80154068",
            Name = "XONLINE_E_ACCOUNTS_RESERVE_NAME_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154069",
            Name = "XONLINE_E_ACCOUNTS_RESTORE_ACCOUNT_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154070",
            Name = "XONLINE_E_ACCOUNTS_SET_NO_AGE_OUT_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154071",
            Name = "XONLINE_E_ACCOUNTS_TERMS_OF_USE_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154072",
            Name = "XONLINE_E_ACCOUNTS_SET_PAYMENT_INFO_ERROR",
            Category = "Billing",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80154073",
            Name = "XONLINE_E_ACCOUNTS_SET_ACCOUNT_STATUS_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154074",
            Name = "XONLINE_E_ACCOUNTS_SET_USER_PIN_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154075",
            Name = "XONLINE_E_ACCOUNTS_SET_USER_SETTINGS_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154076",
            Name = "XONLINE_E_ACCOUNTS_TROUBLESHOOT_ACCOUNT_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154077",
            Name = "XONLINE_E_ACCOUNTS_UPDATE_PARENTAL_CONTROLS_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154078",
            Name = "XONLINE_E_ACCOUNTS_UPDATE_PAYMENT_INSTRUMENT_ERROR",
            Category = "Billing",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80154079",
            Name = "XONLINE_E_ACCOUNTS_VERIFY_BILLING_INFO_ERROR",
            Category = "Billing",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015407A",
            Name = "XONLINE_E_ACCOUNTS_VERIFY_BILLING_PIN_ERROR",
            Category = "Billing",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015407B",
            Name = "XONLINE_E_ACCOUNTS_VERIFY_PARENT_CREDIT_CARD_ERROR",
            Category = "Billing",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8015407C",
            Name = "XONLINE_E_ACCOUNTS_VERIFY_VOUCHER_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015407D",
            Name = "XONLINE_E_ACCOUNTS_VERIFY_VOUCHER_GET_OFFER_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015407E",
            Name = "XONLINE_E_ACCOUNTS_WEB_GET_USER_SETTINGS_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015407F",
            Name = "XONLINE_E_ACCOUNTS_ACKNOWLEDGE_DOWNGRADE_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154080",
            Name = "XONLINE_E_ACCOUNTS_GET_ACCOUNT_INFO_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154081",
            Name = "XONLINE_E_ACCOUNTS_GET_ACCOUNT_INFO_FROM_PASSPORT_ERROR",
            Category = "Network",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154082",
            Name = "XONLINE_E_ACCOUNTS_GET_POINTS_BALANCE_ERROR",
            Category = "Billing",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80154083",
            Name = "XONLINE_E_ACCOUNTS_GET_USER_SUBSCRIPTION_DETAILS_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154084",
            Name = "XONLINE_E_ACCOUNTS_GET_USER_TYPE_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154085",
            Name = "XONLINE_E_ACCOUNTS_GET_USER_WEB_INFO_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154086",
            Name = "XONLINE_E_ACCOUNTS_PASSPORT_CHANGE_PASSWORD_ERROR",
            Category = "Network",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154087",
            Name = "XONLINE_E_ACCOUNTS_PASSPORT_CREATE_ERROR",
            Category = "Network",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154088",
            Name = "XONLINE_E_ACCOUNTS_PASSPORT_GET_BUDDY_GAMERTAG_ERROR",
            Category = "Network",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154089",
            Name = "XONLINE_E_ACCOUNTS_RESERVE_GAMERTAG_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015408A",
            Name = "XONLINE_E_ACCOUNTS_PASSPORT_GET_MEMBER_NAME_ERROR",
            Category = "Network",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015408B",
            Name = "XONLINE_E_ACCOUNTS_PAYPAL_UNSUPPORTED_COUNTRY",
            Category = "Network",
            Meaning = "PayPal not supported in user’s country",
            CommonCause = "PayPal not supported in user’s country",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015408D",
            Name = "XONLINE_E_ACCOUNTS_CANNOT_REPLACE_VALID_PASSPORT",
            Category = "Network",
            Meaning = "Accountcannot Replacvalid Passport",
            CommonCause = "DNS, NAT, gateway, Xbox Live endpoint, or local network connectivity issue.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015408E",
            Name = "XONLINE_E_ACCOUNTS_REPLACE_OWNER_PASSPORT_ERROR",
            Category = "Network",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015408F",
            Name = "XONLINE_E_ACCOUNTS_REPLACE_USER_PASSPORT_ERROR",
            Category = "Network",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154090",
            Name = "XONLINE_E_ACCOUNTS_REPLACE_PASSPORT_QUEUED",
            Category = "Network",
            Meaning = "The XeReplaceUserPassport operation was interrupted due to SCS or DB error and will be retried",
            CommonCause = "The XeReplaceUserPassport operation was interrupted due to SCS or DB error and will be retried",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154091",
            Name = "XONLINE_E_ACCOUNTS_GET_USER_TENURE_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154092",
            Name = "XONLINE_E_ACCOUNTS_GET_USER_TENURE_DATABASE_ERROR",
            Category = "Database",
            Meaning = "Accountget User Tenurdatabaserror",
            CommonCause = "Backend database error, timeout, duplicate key, deadlock, or invalid query result.",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x80154093",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_USER_DATE_OF_BIRTH_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154094",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_OWNER_PASSPORT_ERROR",
            Category = "Network",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154095",
            Name = "XONLINE_E_ACCOUNTS_INVALID_OWNER_PASSPORT_ERROR",
            Category = "Network",
            Meaning = "Accountinvalid Owner Passport Error",
            CommonCause = "DNS, NAT, gateway, Xbox Live endpoint, or local network connectivity issue.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154096",
            Name = "XONLINE_E_ACCOUNTS_GET_SUBSCRIPTION_INFO_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154097",
            Name = "XONLINE_E_ACCOUNTS_GET_BILLING_NOTIFICATIONS_ERROR",
            Category = "Billing",
            Meaning = "no notifications found",
            CommonCause = "no notifications found",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80154098",
            Name = "XONLINE_E_ACCOUNTS_USER_GET_ACCOUNT_INFO_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x80154099",
            Name = "XONLINE_E_ACCOUNTS_USER_OPTED_OUT",
            Category = "Accounts",
            Meaning = "Accountuser Opted Out",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015409A",
            Name = "XONLINE_E_ACCOUNTS_GET_USER_TENURE_NO_POLICY",
            Category = "Accounts",
            Meaning = "a configuration error",
            CommonCause = "a configuration error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015409B",
            Name = "XONLINE_E_ACCOUNTS_GET_USER_TENURE_NO_TENURE",
            Category = "Accounts",
            Meaning = "Accountget User Tenurno Tenure",
            CommonCause = "Account state, Passport/WLID link, gamertag, privilege, region, or enforcement issue.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015409C",
            Name = "XONLINE_E_ACCOUNTS_GET_USER_TENURE_NO_LEVELS",
            Category = "Accounts",
            Meaning = "a configuration error",
            CommonCause = "a configuration error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x8015409D",
            Name = "XONLINE_E_ACCOUNTS_GET_USER_TENURE_NO_MILESTONES",
            Category = "Accounts",
            Meaning = "a configuration error",
            CommonCause = "a configuration error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x0015409E",
            Name = "XONLINE_I_ACCOUNTS_GET_USER_TENURE_NO_NEXT_MILESTONE",
            Category = "Success",
            Meaning = "a configuration error",
            CommonCause = "a configuration error",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x8015409F",
            Name = "XONLINE_E_ACCOUNTS_GET_SUBSCRIPTION_INFO_SILVER_USER",
            Category = "Accounts",
            Meaning = "User is a silver account…",
            CommonCause = "User is a silver account…",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540A0",
            Name = "XONLINE_E_ACCOUNTS_CANNOT_UNGRADUATE_USER",
            Category = "Accounts",
            Meaning = "cannot change a user’s birthday from adult to child",
            CommonCause = "cannot change a user’s birthday from adult to child",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540A1",
            Name = "XONLINE_E_ACCOUNTS_INVALID_AGE",
            Category = "Accounts",
            Meaning = "cannot change a user’s birthday to a date in the future, or long ago",
            CommonCause = "cannot change a user’s birthday to a date in the future, or long ago",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540A2",
            Name = "XONLINE_E_ACCOUNTS_MAX_ACCOUNTS_REACHED",
            Category = "Accounts",
            Meaning = "Max number of xbox accounts created from a machine has been reached",
            CommonCause = "Max number of xbox accounts created from a machine has been reached",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540A3",
            Name = "XONLINE_E_ACCOUNTS_GENERATE_GAMERTAG_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540A4",
            Name = "XONLINE_E_ACCOUNTS_USER_FREE_GAMERTAG_CHANGE_NOT_ELIGIBLE_ERROR",
            Category = "Accounts",
            Meaning = "User is not eligible for Free gamertag change",
            CommonCause = "User is not eligible for Free gamertag change",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540A5",
            Name = "XONLINE_E_ACCOUNTS_GET_CONSOLE_TRUST_LEVEL",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540A6",
            Name = "XONLINE_E_ACCOUNTS_SET_CONSOLE_TRUST_LEVEL",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540A7",
            Name = "XONLINE_E_ACCOUNTS_INVALID_CONSOLE_TRUST_LEVEL",
            Category = "Accounts",
            Meaning = "Console Trust Level outside valid range",
            CommonCause = "Console Trust Level outside valid range",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540A8",
            Name = "XONLINE_E_ACCOUNTS_CREATE_MOBILE_ACCOUNT_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540A9",
            Name = "XONLINE_E_ACCOUNTS_ENUM_FAMILY_MEMBERS_ERROR",
            Category = "Accounts",
            Meaning = "Error occurred during call to EnumFamilyMembers API",
            CommonCause = "Error occurred during call to EnumFamilyMembers API",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540AA",
            Name = "XONLINE_E_ACCOUNTS_ADD_DEPENDENT_ERROR",
            Category = "Accounts",
            Meaning = "Error occurred during call to AddDependent API",
            CommonCause = "Error occurred during call to AddDependent API",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540AB",
            Name = "XONLINE_E_ACCOUNTS_INVALID_FAMILY_SUBSCRIPTION",
            Category = "Accounts",
            Meaning = "No corresponding family subscription offer found in catalog.",
            CommonCause = "No corresponding family subscription offer found in catalog.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540AC",
            Name = "XONLINE_E_ACCOUNTS_INVALID_DEPENDENT_BASE",
            Category = "Accounts",
            Meaning = "Dependent does not have base subscription",
            CommonCause = "Dependent does not have base subscription",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540AD",
            Name = "XONLINE_E_ACCOUNTS_INVALID_DEPENDENT_TIER",
            Category = "Accounts",
            Meaning = "Dependent is not silver",
            CommonCause = "Dependent is not silver",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540AE",
            Name = "XONLINE_E_ACCOUNTS_UNKNOWN_ERROR_CHANGE_STATE",
            Category = "Accounts",
            Meaning = "Unknow error changing the state",
            CommonCause = "Unknow error changing the state",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540AF",
            Name = "XONLINE_E_ACCOUNTS_UNKNOWN_ERROR_MOVE_DEP_SUBSCRIPTION",
            Category = "Accounts",
            Meaning = "Unknow error moving dependent subscription",
            CommonCause = "Unknow error moving dependent subscription",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540B0",
            Name = "XONLINE_E_ACCOUNTS_INVALID_COUNTRYID",
            Category = "Accounts",
            Meaning = "CountryDictionary.CountryCode returns null",
            CommonCause = "CountryDictionary.CountryCode returns null",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540B1",
            Name = "XONLINE_E_ACCOUNTS_INVALID_GAMERTAG",
            Category = "Accounts",
            Meaning = "VerifyGamerTag rejected gamertag",
            CommonCause = "VerifyGamerTag rejected gamertag",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540B2",
            Name = "XONLINE_E_ACCOUNTS_BIRTHDATE_INVALID",
            Category = "Accounts",
            Meaning = "Birthdate Invalid (future)",
            CommonCause = "Birthdate Invalid (future)",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540B3",
            Name = "XONLINE_E_ACCOUNTS_FORBIDDEN_GAMERTAG",
            Category = "Accounts",
            Meaning = "VetName failed for gamertag",
            CommonCause = "VetName failed for gamertag",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540B4",
            Name = "XONLINE_E_ACCOUNTS_FAMILY_GOLD_ASSIGNMENT_INELIGIBLE_ERROR",
            Category = "Accounts",
            Meaning = "User is not elegible to be granted dependent gold right in family subscription",
            CommonCause = "User is not elegible to be granted dependent gold right in family subscription",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540B5",
            Name = "XONLINE_E_ACCOUNTS_GET_PARENTAL_CONTROLS_ERROR",
            Category = "Accounts",
            Meaning = "Error in parental controls",
            CommonCause = "Error in parental controls",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540B6",
            Name = "XONLINE_E_ACCOUNTS_GET_DEPENDENT_SIGNED_PUID_ERROR",
            Category = "Accounts",
            Meaning = "Error in GetDependentSignedPuid",
            CommonCause = "Error in GetDependentSignedPuid",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540B7",
            Name = "XONLINE_E_ACCOUNTS_GET_BASE_SUBSCRIPTION_ERROR",
            Category = "Accounts",
            Meaning = "No Base subscription for the user",
            CommonCause = "No Base subscription for the user",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540B8",
            Name = "XONLINE_E_ACCOUNTS_MULTIPLE_ACTIVE_BASE_SUBSCRIPTION",
            Category = "Accounts",
            Meaning = "Multiple Active base subscription",
            CommonCause = "Multiple Active base subscription",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540B9",
            Name = "XONLINE_E_ACCOUNTS_NO_ACTIVE_BASE_SUBSCRIPTION",
            Category = "Accounts",
            Meaning = "No Active base subscription",
            CommonCause = "No Active base subscription",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540BA",
            Name = "XONLINE_E_ACCOUNTS_INVALID_FAMILYGOLD_CONVERTION_VALUE",
            Category = "Accounts",
            Meaning = "Invalid value configured for Family gold convertion value",
            CommonCause = "Invalid value configured for Family gold convertion value",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540BB",
            Name = "XONLINE_E_ACCOUNTS_FAMILY_GOLD_NOT_DEPENDENT_ERROR",
            Category = "Accounts",
            Meaning = "User is not a dependent and can’t be granted a gold seat in a family subscription",
            CommonCause = "User is not a dependent and can’t be granted a gold seat in a family subscription",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540BC",
            Name = "XONLINE_E_ACCOUNTS_FAMILY_GOLD_NO_SEATS_REMAIN_ERROR",
            Category = "Accounts",
            Meaning = "A dependent can’t be assigned a gold seat in a family subscription because no seats remain.",
            CommonCause = "A dependent can’t be assigned a gold seat in a family subscription because no seats remain.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540BD",
            Name = "XONLINE_E_ACCOUNTS_FAMILY_GOLD_ALREADY_ASSIGNED_ERROR",
            Category = "Accounts",
            Meaning = "A dependent can’t be assigned a gold seat because they already occupy one",
            CommonCause = "A dependent can’t be assigned a gold seat because they already occupy one",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540BE",
            Name = "XONLINE_E_ACCOUNTS_FAMILY_GOLD_BILLING_REGION_ERROR",
            Category = "Billing",
            Meaning = "A dependent can’t be assigned a gold seat because their billing region does not match the owner’s",
            CommonCause = "A dependent can’t be assigned a gold seat because their billing region does not match the owner’s",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x801540BF",
            Name = "XONLINE_E_ACCOUNTS_FAMILY_GOLD_COOLDOWN_ERROR",
            Category = "Accounts",
            Meaning = "A dependent can’t be assigned a gold seat because the cooldown period has no expired",
            CommonCause = "A dependent can’t be assigned a gold seat because the cooldown period has no expired",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540C0",
            Name = "XONLINE_E_ACCOUNTS_FAMILY_GOLD_TOO_MANY_USERS_ERROR",
            Category = "Accounts",
            Meaning = "A dependent can’t be assigned a gold seat because too many dependents have been assigned to gold seats recently",
            CommonCause = "A dependent can’t be assigned a gold seat because too many dependents have been assigned to gold seats recently",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540C1",
            Name = "XONLINE_E_ACCOUNTS_FAMILY_GOLD_UNTRUSTED_CONSOLE_ERROR",
            Category = "Accounts",
            Meaning = "A dependent can’t be assigned a gold seat because the console is not trusted",
            CommonCause = "A dependent can’t be assigned a gold seat because the console is not trusted",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540C2",
            Name = "XONLINE_E_ACCOUNTS_INVALID_MACHINEPUID",
            Category = "Accounts",
            Meaning = "Can’t find the specified machine",
            CommonCause = "Can’t find the specified machine",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540C3",
            Name = "XONLINE_E_ACCOUNTS_INVALID_BETA_GROUPID",
            Category = "Accounts",
            Meaning = "Can’t find the specified beta group",
            CommonCause = "Can’t find the specified beta group",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540C4",
            Name = "XONLINE_E_ACCOUNTS_POINTS_TRANSFER_ERROR",
            Category = "Billing",
            Meaning = "Unkown error transfering points balance",
            CommonCause = "Unkown error transfering points balance",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x801540C5",
            Name = "XONLINE_E_ACCOUNTS_INVALID_DEPENDENT_PRIMARY",
            Category = "Accounts",
            Meaning = "There is no parent child relationship between users",
            CommonCause = "There is no parent child relationship between users",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540C7",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_USER_COUNTRY_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540C8",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_USER_COUNTRY_INELIGIBLE",
            Category = "Accounts",
            Meaning = "The user has switched countries less than one year ago",
            CommonCause = "The user has switched countries less than one year ago",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540C9",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_USER_COUNTRY_INVALID_COUNTRY",
            Category = "Accounts",
            Meaning = "The country requested is not a LIVE-enabled country",
            CommonCause = "The country requested is not a LIVE-enabled country",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540CA",
            Name = "XONLINE_E_ACCOUNTS_FAMILY_GOLD_DEPENDENT_HAS_FAMILY",
            Category = "Accounts",
            Meaning = "A dependent can’t be added because they have a family membership.",
            CommonCause = "A dependent can’t be added because they have a family membership.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540CB",
            Name = "XONLINE_E_ACCOUNTS_GET_PARENTAL_CONTROL_GROUP_TEMPLATES_ERROR",
            Category = "Accounts",
            Meaning = "Error in GetParentalControlGroupTemplates",
            CommonCause = "Error in GetParentalControlGroupTemplates",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540CC",
            Name = "XONLINE_E_ACCOUNTS_GET_USER_PARENTAL_CONTROL_GROUP_ERROR",
            Category = "Accounts",
            Meaning = "Error in GetUserParentalControlGroup",
            CommonCause = "Error in GetUserParentalControlGroup",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540CD",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_USER_COUNTRY_INVALID_LOCALE",
            Category = "Accounts",
            Meaning = "The locale requested is not a valid LIVE locale",
            CommonCause = "The locale requested is not a valid LIVE locale",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x001540CE",
            Name = "XONLINE_S_ACCOUNTS_SWITCH_USER_COUNTRY_SAME_COUNTRY",
            Category = "Success",
            Meaning = "The user requested a switch to the country already associated with their account",
            CommonCause = "The user requested a switch to the country already associated with their account",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x801540CF",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_USER_COUNTRY_TRY_AGAIN",
            Category = "Accounts",
            Meaning = "The operation was interrupted or timed out. Retry the call to determine the result.",
            CommonCause = "The operation was interrupted or timed out. Retry the call to determine the result.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540D0",
            Name = "XONLINE_E_ACCOUNTS_UPDATE_PARENTAL_CONTROL_GROUP_ERROR",
            Category = "Accounts",
            Meaning = "Error in UpdateParentalControlGroup",
            CommonCause = "Error in UpdateParentalControlGroup",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540D1",
            Name = "XONLINE_E_ACCOUNTS_NEGATIVE_POINTS_VALUE",
            Category = "Billing",
            Meaning = "Negative points value passed",
            CommonCause = "Negative points value passed",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x801540D2",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_USER_COUNTRY_ADULT_TO_CHILD",
            Category = "Accounts",
            Meaning = "The user switching is an adult but would be a child in the new country.",
            CommonCause = "The user switching is an adult but would be a child in the new country.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540D3",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_USER_COUNTRY_DATABASE_ERROR",
            Category = "Database",
            Meaning = "Database error as part of executing SwitchUserCountry",
            CommonCause = "Database error as part of executing SwitchUserCountry",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x801540D4",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_USER_COUNTRY_NO_MIGRATION_OFFER",
            Category = "Marketplace",
            Meaning = "No migration offer found for SwitchUserCountry",
            CommonCause = "No migration offer found for SwitchUserCountry",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x801540D5",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_USER_COUNTRY_SUSPENDED_SUBSCRIPTION",
            Category = "Accounts",
            Meaning = "SwitchUserCountry does not allow migration for users with Xbox subscription which is suspended or expired",
            CommonCause = "SwitchUserCountry does not allow migration for users with Xbox subscription which is suspended or expired",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540D6",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_USER_COUNTRY_NON_XBOX_SUBSCRIPTION",
            Category = "Accounts",
            Meaning = "SwitchUserCountry does not allow migration for users with non-Xbox subscriptions such as Zune Pass, XNA Creators’ Club, etc.",
            CommonCause = "SwitchUserCountry does not allow migration for users with non-Xbox subscriptions such as Zune Pass, XNA Creators’ Club, etc.",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540D7",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_USER_COUNTRY_CHILD_ACCOUNT",
            Category = "Accounts",
            Meaning = "SwitchUserCountry does not allow child accounts to migrate",
            CommonCause = "SwitchUserCountry does not allow child accounts to migrate",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540D8",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_USER_COUNTRY_FAMILY_ACCOUNT",
            Category = "Accounts",
            Meaning = "SwitchUserCountry does not allow family accounts to migrate",
            CommonCause = "SwitchUserCountry does not allow family accounts to migrate",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540D9",
            Name = "XONLINE_E_ACCOUNTS_GET_DEPENDENT_POINTS_BALANCE_ERROR",
            Category = "Billing",
            Meaning = "Unkown error getting dependent point balance",
            CommonCause = "Unkown error getting dependent point balance",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x801540DA",
            Name = "XONLINE_E_ACCOUNTS_DEPENDENT_MOVE_SUBSCRIPTION",
            Category = "Accounts",
            Meaning = "Error when moving dependents subscription",
            CommonCause = "Error when moving dependents subscription",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540DB",
            Name = "XONLINE_E_ACCOUNTS_CREDENTIAL_LIST_BY_NAME_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540DC",
            Name = "XONLINE_E_ACCOUNTS_USER_GET_AGE_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540DD",
            Name = "XONLINE_E_ACCOUNTS_USER_GET_AGE_GROUP_ERROR",
            Category = "Accounts",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540DE",
            Name = "XONLINE_E_ACCOUNTS_DEPENDENT_USER_IS_OWNER",
            Category = "Accounts",
            Meaning = "Error while creating owner a dependent user",
            CommonCause = "Error while creating owner a dependent user",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540DF",
            Name = "XONLINE_E_ACCOUNTS_OWNER_IS_DEPENDENT",
            Category = "Accounts",
            Meaning = "Error while creating dependent user as owner",
            CommonCause = "Error while creating dependent user as owner",
            FixSuggestion = "Verify account details, gamertag/passport link, region, privileges, and enforcement/account management status."
        },

        new()
        {
            Code = "0x801540E0",
            Name = "XONLINE_E_CREATE_BULK_USER_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch all) api error",
            CommonCause = "non-specific (catch all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801540E7",
            Name = "XONLINE_E_ACCOUNTS_SWITCH_OWNER_PASSPORT_USER_INELIGIBLE",
            Category = "Network",
            Meaning = "user attempting to switch owner passport puids does not meet certain requirements",
            CommonCause = "user attempting to switch owner passport puids does not meet certain requirements",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80154100",
            Name = "XONLINE_E_MSNRR_BEGIN_ERROR_RANGE",
            Category = "Xbox Live",
            Meaning = "marks beginning of msnrr-related error codes",
            CommonCause = "marks beginning of msnrr-related error codes",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80154100",
            Name = "XONLINE_E_MSNRR_UNKNOWN_ERROR",
            Category = "Xbox Live",
            Meaning = "Generic MSNRR error. See server event log for specific details about what went wrong.",
            CommonCause = "Generic MSNRR error. See server event log for specific details about what went wrong.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80154101",
            Name = "XONLINE_E_MSNRR_INVALID_CONTENT_TYPE_ID",
            Category = "Marketplace",
            Meaning = "Invalid content type id. This error also occurs if content type does not have a rating attribute or review field.",
            CommonCause = "Invalid content type id. This error also occurs if content type does not have a rating attribute or review field.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80154102",
            Name = "XONLINE_E_MSNRR_INVALID_ITEM_ID",
            Category = "Validation",
            Meaning = "Invalid item id",
            CommonCause = "Invalid item id",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80154103",
            Name = "XONLINE_E_MSNRR_PUID_CANNOT_BE_ZERO",
            Category = "Xbox Live",
            Meaning = "PUID cannot be zero",
            CommonCause = "PUID cannot be zero",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80154104",
            Name = "XONLINE_E_MSNRR_USE_MULTIPLE_RATING",
            Category = "Xbox Live",
            Meaning = "Content type has multiple rating attribute ids, use the Multiple Rating Interface.",
            CommonCause = "Content type has multiple rating attribute ids, use the Multiple Rating Interface.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80154105",
            Name = "XONLINE_E_MSNRR_INVALID_TRANSACTION_TOKEN",
            Category = "Authentication",
            Meaning = "Token does not match request parameters, start over with no token.",
            CommonCause = "Token does not match request parameters, start over with no token.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154200",
            Name = "XONLINE_E_TOKEN_UNKNOWN_ERROR",
            Category = "Authentication",
            Meaning = "Generic token error. See server event log for specific details about what went wrong.",
            CommonCause = "Generic token error. See server event log for specific details about what went wrong.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154201",
            Name = "XONLINE_E_TOKEN_FILE_NOT_FOUND",
            Category = "Authentication",
            Meaning = "Token File Not Found",
            CommonCause = "Token File Not Found",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154202",
            Name = "XONLINE_E_TOKEN_REQUESTED_TOKENS_EXCEEDS_MAXIMUM_ALLOWABLE",
            Category = "Authentication",
            Meaning = "Token Requested Tokenexceedmaximum Allowable",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154203",
            Name = "XONLINE_E_TOKEN_INVALID_START_PARAMETER",
            Category = "Authentication",
            Meaning = "Token Invalid Start Parameter",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154204",
            Name = "XONLINE_E_TOKEN_MALFORMED_TOKEN_ENTRY_FOUND",
            Category = "Authentication",
            Meaning = "Token Malformed Token Entry Found",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154205",
            Name = "XONLINE_E_TOKEN_MALFORMED_5X5_TOKEN_CODE_FOUND",
            Category = "Authentication",
            Meaning = "Token Malformed 5X5 Token Codfound",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154206",
            Name = "XONLINE_E_TOKEN_FILE_COULD_NOT_BE_DECRYPTED",
            Category = "Authentication",
            Meaning = "Token Filcould Not Bdecrypted",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154207",
            Name = "XONLINE_E_TOKEN_EOF_REACHED_BEFORE_ALL_TOKENS_RETRIEVED",
            Category = "Authentication",
            Meaning = "Token Eof Reached Beforall Tokenretrieved",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154208",
            Name = "XONLINE_E_TOKEN_JOB_NOT_IN_REQUIRED_STATE",
            Category = "Authentication",
            Meaning = "Token Job Not In Required State",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154209",
            Name = "XONLINE_E_TOKEN_NOT_YET_REDEEMABLE",
            Category = "Authentication",
            Meaning = "token cannot be redeemed yet because the token category is not flagged as redeemable",
            CommonCause = "token cannot be redeemed yet because the token category is not flagged as redeemable",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154210",
            Name = "XONLINE_E_TOKEN_LOAD_SCS_ERROR",
            Category = "Authentication",
            Meaning = "Token Load Scerror",
            CommonCause = "Xbox Live sign-in, token, profile, credential, or authorization failure.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154211",
            Name = "XONLINE_E_TOKEN_DUPLICATE_CATEGORY_NAME",
            Category = "Authentication",
            Meaning = "A duplicate token category name is found when trying to create a new token category.",
            CommonCause = "A duplicate token category name is found when trying to create a new token category.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154212",
            Name = "XONLINE_E_TOKEN_NO_CATEGORY_FOUND",
            Category = "Authentication",
            Meaning = "Cannot find the token category.",
            CommonCause = "Cannot find the token category.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154213",
            Name = "XONLINE_E_TOKEN_CATEGORY_READ_ONLY_PROPERTY_CHANGE",
            Category = "Authentication",
            Meaning = "The “read only” properties of token category cannot be changed.",
            CommonCause = "The “read only” properties of token category cannot be changed.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154220",
            Name = "XONLINE_E_TOKEN_JOB_DATABASE_ERROR",
            Category = "Database",
            Meaning = "Unexpected database error manipulating Token Job in NPDB",
            CommonCause = "Unexpected database error manipulating Token Job in NPDB",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x80154221",
            Name = "XONLINE_E_TOKEN_JOB_NOT_FOUND",
            Category = "Authentication",
            Meaning = "Token Job either not present or not currently in Pending or abandoned state",
            CommonCause = "Token Job either not present or not currently in Pending or abandoned state",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154231",
            Name = "XONLINE_E_TOKEN_SEQUENCE_NUMBER_INVALID_INDEX",
            Category = "Authentication",
            Meaning = "We can’t generate sequence number because the index is too large.",
            CommonCause = "We can’t generate sequence number because the index is too large.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80154232",
            Name = "XONLINE_E_TOKEN_EXPIRED_TOKEN_CATEGORY",
            Category = "Authentication",
            Meaning = "Tokens cannot be generated for an expired token category",
            CommonCause = "Tokens cannot be generated for an expired token category",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80155000",
            Name = "XONLINE_E_NOTIFICATION_BAD_CONTENT_TYPE",
            Category = "Marketplace",
            Meaning = "Notification Bad Content Type",
            CommonCause = "Offer, license, entitlement, content availability, or region restriction issue.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80155001",
            Name = "XONLINE_E_NOTIFICATION_REQUEST_TOO_SMALL",
            Category = "Notification",
            Meaning = "Notification Request Too Small",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155002",
            Name = "XONLINE_E_NOTIFICATION_INVALID_MESSAGE_TYPE",
            Category = "Notification",
            Meaning = "Notification Invalid Messagtype",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155003",
            Name = "XONLINE_E_NOTIFICATION_NO_ADDRESS",
            Category = "Notification",
            Meaning = "Notification No Address",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155004",
            Name = "XONLINE_E_NOTIFICATION_INVALID_PUID",
            Category = "Notification",
            Meaning = "Notification Invalid Puid",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155005",
            Name = "XONLINE_E_NOTIFICATION_NO_CONNECTION",
            Category = "Network",
            Meaning = "Notification No Connection",
            CommonCause = "DNS, NAT, gateway, Xbox Live endpoint, or local network connectivity issue.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80155006",
            Name = "XONLINE_E_NOTIFICATION_SEND_FAILED",
            Category = "Notification",
            Meaning = "Notification Send Failed",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155007",
            Name = "XONLINE_E_NOTIFICATION_RECV_FAILED",
            Category = "Notification",
            Meaning = "Notification Recv Failed",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155008",
            Name = "XONLINE_E_NOTIFICATION_MESSAGE_TRUNCATED",
            Category = "Notification",
            Meaning = "Notification Messagtruncated",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155100",
            Name = "XONLINE_E_MATCH_INVALID_SESSION_ID",
            Category = "Validation",
            Meaning = "specified session id does not exist",
            CommonCause = "specified session id does not exist",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155101",
            Name = "XONLINE_E_MATCH_INVALID_TITLE_ID",
            Category = "Validation",
            Meaning = "specified title id is zero, or does not exist",
            CommonCause = "specified title id is zero, or does not exist",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155102",
            Name = "XONLINE_E_MATCH_INVALID_DATA_TYPE",
            Category = "Validation",
            Meaning = "attribute ID or parameter type specifies an invalid data type",
            CommonCause = "attribute ID or parameter type specifies an invalid data type",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155103",
            Name = "XONLINE_E_MATCH_REQUEST_TOO_SMALL",
            Category = "Xbox Live",
            Meaning = "the request did not meet the minimum length for a valid request",
            CommonCause = "the request did not meet the minimum length for a valid request",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155104",
            Name = "XONLINE_E_MATCH_REQUEST_TRUNCATED",
            Category = "Xbox Live",
            Meaning = "the self described length is greater than the actual buffer size",
            CommonCause = "the self described length is greater than the actual buffer size",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155105",
            Name = "XONLINE_E_MATCH_INVALID_SEARCH_REQ",
            Category = "Validation",
            Meaning = "the search request was invalid",
            CommonCause = "the search request was invalid",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155106",
            Name = "XONLINE_E_MATCH_INVALID_OFFSET",
            Category = "Validation",
            Meaning = "one of the attribute/parameter offsets in the request was invalid. Will be followed by the zero based offset number.",
            CommonCause = "one of the attribute/parameter offsets in the request was invalid. Will be followed by the zero based offset number.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155107",
            Name = "XONLINE_E_MATCH_INVALID_ATTR_TYPE",
            Category = "Validation",
            Meaning = "the attribute type was something other than user or session",
            CommonCause = "the attribute type was something other than user or session",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155108",
            Name = "XONLINE_E_MATCH_INVALID_VERSION",
            Category = "Validation",
            Meaning = "bad protocol version in request",
            CommonCause = "bad protocol version in request",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155109",
            Name = "XONLINE_E_MATCH_OVERFLOW",
            Category = "Xbox Live",
            Meaning = "an attribute or parameter flowed past the end of the request",
            CommonCause = "an attribute or parameter flowed past the end of the request",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015510A",
            Name = "XONLINE_E_MATCH_INVALID_RESULT_COL",
            Category = "Validation",
            Meaning = "referenced stored procedure returned a column with an unsupported data type",
            CommonCause = "referenced stored procedure returned a column with an unsupported data type",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015510B",
            Name = "XONLINE_E_MATCH_INVALID_STRING",
            Category = "Validation",
            Meaning = "string with length-prefix of zero, or string with no terminating null",
            CommonCause = "string with length-prefix of zero, or string with no terminating null",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015510C",
            Name = "XONLINE_E_MATCH_STRING_TOO_LONG",
            Category = "Xbox Live",
            Meaning = "string exceeded 400 characters",
            CommonCause = "string exceeded 400 characters",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015510D",
            Name = "XONLINE_E_MATCH_BLOB_TOO_LONG",
            Category = "Xbox Live",
            Meaning = "blob exceeded 800 bytes",
            CommonCause = "blob exceeded 800 bytes",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155110",
            Name = "XONLINE_E_MATCH_INVALID_ATTRIBUTE_ID",
            Category = "Validation",
            Meaning = "attribute id is invalid",
            CommonCause = "attribute id is invalid",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155112",
            Name = "XONLINE_E_MATCH_SESSION_ALREADY_EXISTS",
            Category = "Xbox Live",
            Meaning = "session id already exists in the db",
            CommonCause = "session id already exists in the db",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155115",
            Name = "XONLINE_E_MATCH_CRITICAL_DB_ERR",
            Category = "Xbox Live",
            Meaning = "critical error in db",
            CommonCause = "critical error in db",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155116",
            Name = "XONLINE_E_MATCH_NOT_ENOUGH_COLUMNS",
            Category = "Xbox Live",
            Meaning = "search result set had too few columns",
            CommonCause = "search result set had too few columns",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155117",
            Name = "XONLINE_E_MATCH_PERMISSION_DENIED",
            Category = "Security",
            Meaning = "incorrect permissions set on search sp",
            CommonCause = "incorrect permissions set on search sp",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x80155118",
            Name = "XONLINE_E_MATCH_INVALID_PART_SCHEME",
            Category = "Validation",
            Meaning = "title specified an invalid partitioning scheme",
            CommonCause = "title specified an invalid partitioning scheme",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155119",
            Name = "XONLINE_E_MATCH_INVALID_PARAM",
            Category = "Validation",
            Meaning = "bad parameter passed to sp",
            CommonCause = "bad parameter passed to sp",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015511D",
            Name = "XONLINE_E_MATCH_DATA_TYPE_MISMATCH",
            Category = "Xbox Live",
            Meaning = "data type specified in attr id did not match type of attr being set",
            CommonCause = "data type specified in attr id did not match type of attr being set",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015511E",
            Name = "XONLINE_E_MATCH_SERVER_ERROR",
            Category = "Xbox Live",
            Meaning = "error on server not correctable by client",
            CommonCause = "error on server not correctable by client",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015511F",
            Name = "XONLINE_E_MATCH_NO_USERS",
            Category = "Xbox Live",
            Meaning = "no authenticated users in search request.",
            CommonCause = "no authenticated users in search request.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155120",
            Name = "XONLINE_E_MATCH_INVALID_BLOB",
            Category = "Validation",
            Meaning = "invalid blob attribute",
            CommonCause = "invalid blob attribute",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155121",
            Name = "XONLINE_E_MATCH_TOO_MANY_USERS",
            Category = "Xbox Live",
            Meaning = "too many users in search request",
            CommonCause = "too many users in search request",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155122",
            Name = "XONLINE_E_MATCH_INVALID_FLAGS",
            Category = "Validation",
            Meaning = "invalid flags were specified in a search request",
            CommonCause = "invalid flags were specified in a search request",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155123",
            Name = "XONLINE_E_MATCH_PARAM_MISSING",
            Category = "Xbox Live",
            Meaning = "required parameter not passed to sp",
            CommonCause = "required parameter not passed to sp",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155124",
            Name = "XONLINE_E_MATCH_TOO_MANY_PARAM",
            Category = "Xbox Live",
            Meaning = "too many paramters passed to sp or in request structure",
            CommonCause = "too many paramters passed to sp or in request structure",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155125",
            Name = "XONLINE_E_MATCH_DUPLICATE_PARAM",
            Category = "Xbox Live",
            Meaning = "a paramter was passed to twice to a search procedure",
            CommonCause = "a paramter was passed to twice to a search procedure",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155126",
            Name = "XONLINE_E_MATCH_TOO_MANY_ATTR",
            Category = "Xbox Live",
            Meaning = "too many attributes in the request structure",
            CommonCause = "too many attributes in the request structure",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155127",
            Name = "XONLINE_E_MATCH_CONCURRENT_REQ_CONFLICT",
            Category = "Xbox Live",
            Meaning = "this request conflicted with another that was in progress",
            CommonCause = "this request conflicted with another that was in progress",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155128",
            Name = "XONLINE_E_MATCH_SESSION_TYPE_MISMATCH",
            Category = "Xbox Live",
            Meaning = "this request was operation of a row type (breadcrumb vs. sandwich) which mismatched.",
            CommonCause = "this request was operation of a row type (breadcrumb vs. sandwich) which mismatched.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155129",
            Name = "XONLINE_E_MATCH_LOCK_ALREADY_RELEASED",
            Category = "Xbox Live",
            Meaning = "the session was expected to be locked, but it was actually unlocked..",
            CommonCause = "the session was expected to be locked, but it was actually unlocked..",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155200",
            Name = "XONLINE_E_SESSION_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "the specified session was not found",
            CommonCause = "the specified session was not found",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155201",
            Name = "XONLINE_E_SESSION_INSUFFICIENT_PRIVILEGES",
            Category = "Xbox Live",
            Meaning = "the requester does not have permissions to perform this operation",
            CommonCause = "the requester does not have permissions to perform this operation",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155202",
            Name = "XONLINE_E_SESSION_FULL",
            Category = "Xbox Live",
            Meaning = "Session Full",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155203",
            Name = "XONLINE_E_SESSION_INVITES_DISABLED",
            Category = "Xbox Live",
            Meaning = "Session Invitedisabled",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155204",
            Name = "XONLINE_E_SESSION_INVALID_FLAGS",
            Category = "Validation",
            Meaning = "Session Invalid Flags",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155205",
            Name = "XONLINE_E_SESSION_REQUIRES_ARBITRATION",
            Category = "Xbox Live",
            Meaning = "Session Requirearbitration",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155206",
            Name = "XONLINE_E_SESSION_WRONG_STATE",
            Category = "Xbox Live",
            Meaning = "Session Wrong State",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155207",
            Name = "XONLINE_E_SESSION_INSUFFICIENT_BUFFER",
            Category = "Xbox Live",
            Meaning = "Session Insufficient Buffer",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155208",
            Name = "XONLINE_E_SESSION_REGISTRATION_ERROR",
            Category = "Xbox Live",
            Meaning = "Session Registration Error",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155209",
            Name = "XONLINE_E_SESSION_NOT_LOGGED_ON",
            Category = "Xbox Live",
            Meaning = "Session Not Logged On",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015520A",
            Name = "XONLINE_E_SESSION_JOIN_ILLEGAL",
            Category = "Xbox Live",
            Meaning = "Session Join Illegal",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015520B",
            Name = "XONLINE_E_SESSION_CREATE_KEY_FAILED",
            Category = "Xbox Live",
            Meaning = "Session Creatkey Failed",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015520C",
            Name = "XONLINE_E_SESSION_NOT_REGISTERED",
            Category = "Xbox Live",
            Meaning = "Session Not Registered",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015520D",
            Name = "XONLINE_E_SESSION_REGISTER_KEY_FAILED",
            Category = "Xbox Live",
            Meaning = "Session Register Key Failed",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015520E",
            Name = "XONLINE_E_SESSION_UNREGISTER_KEY_FAILED",
            Category = "Xbox Live",
            Meaning = "Session Unregister Key Failed",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155300",
            Name = "XONLINE_E_AUDIT_INVALID_SUBSYSTEM",
            Category = "Validation",
            Meaning = "use specified unknown subsystem",
            CommonCause = "use specified unknown subsystem",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155301",
            Name = "XONLINE_E_AUDIT_NO_SETTING",
            Category = "Xbox Live",
            Meaning = "npdb setting for subsystem does not defined",
            CommonCause = "npdb setting for subsystem does not defined",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155302",
            Name = "XONLINE_E_AUDIT_LOG_FAILURE",
            Category = "Xbox Live",
            Meaning = "error writing to audit log (database)",
            CommonCause = "error writing to audit log (database)",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155380",
            Name = "XONLINE_E_FSE_ERROR",
            Category = "Xbox Live",
            Meaning = "generic unhandled fse exception",
            CommonCause = "generic unhandled fse exception",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155381",
            Name = "XONLINE_E_FSE_BOOKMARK_INVALID_SETTING",
            Category = "Validation",
            Meaning = "invalid bookmark setting (range)",
            CommonCause = "invalid bookmark setting (range)",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155382",
            Name = "XONLINE_E_FSE_BOOKMARK_NO_SETTING",
            Category = "Xbox Live",
            Meaning = "missing bookmark setting",
            CommonCause = "missing bookmark setting",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155388",
            Name = "XONLINE_E_FSE_HANDLER_MISSING",
            Category = "Xbox Live",
            Meaning = "application page or result handler is missing",
            CommonCause = "application page or result handler is missing",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155389",
            Name = "XONLINE_E_FSE_HANDLER_CREATE_ERROR",
            Category = "Xbox Live",
            Meaning = "unhandled application exception derializing result",
            CommonCause = "unhandled application exception derializing result",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015538A",
            Name = "XONLINE_E_FSE_HANDLER_MERGE_ERROR",
            Category = "Xbox Live",
            Meaning = "unhandled application exception merge pages",
            CommonCause = "unhandled application exception merge pages",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015538B",
            Name = "XONLINE_E_FSE_HANDLER_EXTRACT_ERROR",
            Category = "Xbox Live",
            Meaning = "unhandled application exception extracting from page",
            CommonCause = "unhandled application exception extracting from page",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015538C",
            Name = "XONLINE_E_FSE_HANDLER_SPLIT_ERROR",
            Category = "Xbox Live",
            Meaning = "unhandled application exception splitting page",
            CommonCause = "unhandled application exception splitting page",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155390",
            Name = "XONLINE_E_FSE_CACHE_INVALID_EXPIRATION",
            Category = "Validation",
            Meaning = "invalid cache expriation time",
            CommonCause = "invalid cache expriation time",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155391",
            Name = "XONLINE_E_FSE_CACHE_KEY_TOO_LONG",
            Category = "Xbox Live",
            Meaning = "cache key is longer than xbanc configuration setting",
            CommonCause = "cache key is longer than xbanc configuration setting",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x00155392",
            Name = "XONLINE_S_FSE_CACHE_ENTRY_FOUND",
            Category = "Success",
            Meaning = "cache entry found successfully",
            CommonCause = "cache entry found successfully",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80155393",
            Name = "XONLINE_E_FSE_CACHE_TIMEOUT",
            Category = "Xbox Live",
            Meaning = "timeout waiting for cache entry",
            CommonCause = "timeout waiting for cache entry",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155394",
            Name = "XONLINE_E_FSE_CACHE_ERROR",
            Category = "Xbox Live",
            Meaning = "exception thrown out of xbanc proxy",
            CommonCause = "exception thrown out of xbanc proxy",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155395",
            Name = "XONLINE_E_FSE_CACHE_QUERY_ERROR",
            Category = "Xbox Live",
            Meaning = "exception thrown out of xbanc proxy",
            CommonCause = "exception thrown out of xbanc proxy",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155396",
            Name = "XONLINE_E_FSE_CACHE_INSERT_ERROR",
            Category = "Xbox Live",
            Meaning = "exception thrown out of xbanc proxy",
            CommonCause = "exception thrown out of xbanc proxy",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553A0",
            Name = "XONLINE_E_FSE_CONFIG_METHOD_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "configured method was not found",
            CommonCause = "configured method was not found",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553A1",
            Name = "XONLINE_E_FSE_CONFIG_NO_INTERFACE",
            Category = "Xbox Live",
            Meaning = "no interface for configured method",
            CommonCause = "no interface for configured method",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553A2",
            Name = "XONLINE_E_FSE_CONFIG_PARAM_ERROR",
            Category = "Xbox Live",
            Meaning = "parameter configuration is invalid (couldn’t be parsed)",
            CommonCause = "parameter configuration is invalid (couldn’t be parsed)",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553A3",
            Name = "XONLINE_E_FSE_CONFIG_UNKNOWN_TYPE",
            Category = "Xbox Live",
            Meaning = "unknown parameter type (won’t be able to convert)",
            CommonCause = "unknown parameter type (won’t be able to convert)",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553A4",
            Name = "XONLINE_E_FSE_CONFIG_MIN_OCCURS_INVALID",
            Category = "Validation",
            Meaning = "minOccurs < 0",
            CommonCause = "minOccurs < 0",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x801553A5",
            Name = "XONLINE_E_FSE_CONFIG_MAX_OCCURS_INVALID",
            Category = "Validation",
            Meaning = "maxOccurs < 1",
            CommonCause = "maxOccurs < 1",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x801553A6",
            Name = "XONLINE_E_FSE_CONFIG_OCCURENCE_INVERSION",
            Category = "Xbox Live",
            Meaning = "minOccurs > maxOccurs",
            CommonCause = "minOccurs > maxOccurs",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553A7",
            Name = "XONLINE_E_FSE_CONFIG_DUPLICATE_DELEGATE",
            Category = "Xbox Live",
            Meaning = "parameter delegate already exists",
            CommonCause = "parameter delegate already exists",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553A8",
            Name = "XONLINE_E_FSE_CONFIG_PAGE_SIZE_INVALID",
            Category = "Validation",
            Meaning = "cache page size or db page size are invalid",
            CommonCause = "cache page size or db page size are invalid",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x801553B0",
            Name = "XONLINE_E_FSE_DATABASE_ERROR",
            Category = "Database",
            Meaning = "generic database error",
            CommonCause = "generic database error",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x801553B1",
            Name = "XONLINE_E_FSE_DATABASE_CONNECT_ERROR",
            Category = "Database",
            Meaning = "generic database connection error",
            CommonCause = "generic database connection error",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x801553B2",
            Name = "XONLINE_E_FSE_DATABASE_EXECUTE_ERROR",
            Category = "Database",
            Meaning = "generic database execute error",
            CommonCause = "generic database execute error",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x801553C0",
            Name = "XONLINE_E_FSE_METHOD_ERROR",
            Category = "Xbox Live",
            Meaning = "generic method error",
            CommonCause = "generic method error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553C1",
            Name = "XONLINE_E_FSE_METHOD_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "method called by user does not exist",
            CommonCause = "method called by user does not exist",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553C2",
            Name = "XONLINE_E_FSE_METHOD_RETRY_COUNT_EXCEEDED",
            Category = "Xbox Live",
            Meaning = "retry count exceeded when calling database",
            CommonCause = "retry count exceeded when calling database",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553C3",
            Name = "XONLINE_E_FSE_METHOD_RESULT_NOT_PAGED",
            Category = "Xbox Live",
            Meaning = "error constructing paged result",
            CommonCause = "error constructing paged result",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553D0",
            Name = "XONLINE_E_FSE_PARAM_LIST_INVALID",
            Category = "Validation",
            Meaning = "number of names and values do not match",
            CommonCause = "number of names and values do not match",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x801553D1",
            Name = "XONLINE_E_FSE_PARAM_MISSING",
            Category = "Xbox Live",
            Meaning = "missing required (non-optional) parameter",
            CommonCause = "missing required (non-optional) parameter",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553D2",
            Name = "XONLINE_E_FSE_PARAM_UNEXPECTED",
            Category = "Xbox Live",
            Meaning = "extra unrecognized / unexpected parameter",
            CommonCause = "extra unrecognized / unexpected parameter",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553D3",
            Name = "XONLINE_E_FSE_PARAM_INTERNAL_ERROR",
            Category = "Xbox Live",
            Meaning = "internal programming error",
            CommonCause = "internal programming error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553D4",
            Name = "XONLINE_E_FSE_PARAM_TOO_MANY",
            Category = "Xbox Live",
            Meaning = "too many parameters",
            CommonCause = "too many parameters",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553D5",
            Name = "XONLINE_E_FSE_PARAM_INVALID_VALUE",
            Category = "Validation",
            Meaning = "invalid param value (fails regex checks)",
            CommonCause = "invalid param value (fails regex checks)",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x801553D6",
            Name = "XONLINE_E_FSE_PARAM_CONVERSION_ERROR",
            Category = "Xbox Live",
            Meaning = "error converting value to configured type",
            CommonCause = "error converting value to configured type",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553D7",
            Name = "XONLINE_E_FSE_PARAM_DELEGATE_FAILURE",
            Category = "Xbox Live",
            Meaning = "a parameter modifying delegate defined by the application threw an exception",
            CommonCause = "a parameter modifying delegate defined by the application threw an exception",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553D8",
            Name = "XONLINE_E_FSE_PARAM_IS_NULL",
            Category = "Xbox Live",
            Meaning = "required parameter is null",
            CommonCause = "required parameter is null",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553D9",
            Name = "XONLINE_E_FSE_PARAM_TOO_FEW",
            Category = "Xbox Live",
            Meaning = "number of values are is too few",
            CommonCause = "number of values are is too few",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553DA",
            Name = "XONLINE_E_FSE_PARAM_INTERNAL",
            Category = "Xbox Live",
            Meaning = "supplied parameter is internal only",
            CommonCause = "supplied parameter is internal only",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553DB",
            Name = "XONLINE_E_FSE_PARAM_RESERVED",
            Category = "Xbox Live",
            Meaning = "supplied parameter name is reserved",
            CommonCause = "supplied parameter name is reserved",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553E0",
            Name = "XONLINE_E_FSE_BIN_ERROR_EXECUTING",
            Category = "Xbox Live",
            Meaning = "Error executing the FsePlugin",
            CommonCause = "Error executing the FsePlugin",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801553E1",
            Name = "XONLINE_E_FSE_BIN_ERROR_CONSTRUCTING",
            Category = "Xbox Live",
            Meaning = "Error contructing the FsePlugin",
            CommonCause = "Error contructing the FsePlugin",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155400",
            Name = "XONLINE_E_CATALOG_INVALID_DETAIL_VIEW",
            Category = "Validation",
            Meaning = "detail view value is invalid",
            CommonCause = "detail view value is invalid",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155401",
            Name = "XONLINE_E_CATALOG_INVALID_OFFER_FILTER_LEVEL",
            Category = "Marketplace",
            Meaning = "offer filter level value is invalid",
            CommonCause = "offer filter level value is invalid",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80155402",
            Name = "XONLINE_E_CATALOG_INVALID_ORDER_BY",
            Category = "Validation",
            Meaning = "order by value is invalid",
            CommonCause = "order by value is invalid",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155403",
            Name = "XONLINE_E_CATALOG_INVALID_ORDER_DIRECTION",
            Category = "Validation",
            Meaning = "order direction value is invalid",
            CommonCause = "order direction value is invalid",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155404",
            Name = "XONLINE_E_CATALOG_INVALID_RANKING_TYPE",
            Category = "Validation",
            Meaning = "invalid ranking type in full text search",
            CommonCause = "invalid ranking type in full text search",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155405",
            Name = "XONLINE_E_CATALOG_INVALID_LOCALE",
            Category = "Validation",
            Meaning = "invalid locale in full text search",
            CommonCause = "invalid locale in full text search",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155406",
            Name = "XONLINE_E_CATALOG_INVALID_VIDEO_FILTER",
            Category = "Media",
            Meaning = "invalid video filter in FindVideos",
            CommonCause = "invalid video filter in FindVideos",
            FixSuggestion = "Verify media license, device authorization, purchase status, and retry playback or license refresh."
        },

        new()
        {
            Code = "0x80155407",
            Name = "XONLINE_E_CATALOG_INVALID_EDITORIAL_PRIV",
            Category = "Validation",
            Meaning = "invalid editorial privilege",
            CommonCause = "invalid editorial privilege",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155410",
            Name = "XONLINE_E_CATALOG_INVALID_SETTING",
            Category = "Validation",
            Meaning = "catalog service could not process a setting update",
            CommonCause = "catalog service could not process a setting update",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155411",
            Name = "XONLINE_E_CATALOG_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified catalog error",
            CommonCause = "unspecified catalog error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155412",
            Name = "XONLINE_E_CATALOG_DESERIALIZATION_ERROR",
            Category = "Xbox Live",
            Meaning = "could not deserialize catalog result",
            CommonCause = "could not deserialize catalog result",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155413",
            Name = "XONLINE_E_CATALOG_SUBSCRIPTIONS_INVALID_ARGS",
            Category = "Validation",
            Meaning = "Invalid Argument passed to FindSubscriptionProducts API",
            CommonCause = "Invalid Argument passed to FindSubscriptionProducts API",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80155480",
            Name = "XONLINE_E_SVCTUNNEL_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified svctunnel error",
            CommonCause = "unspecified svctunnel error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155481",
            Name = "XONLINE_E_SVCTUNNEL_TIMEOUT",
            Category = "Xbox Live",
            Meaning = "svctunnel timeout error",
            CommonCause = "svctunnel timeout error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155482",
            Name = "XONLINE_E_SVCTUNNEL_GET_USER_INFO_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified svctunnel get user info error",
            CommonCause = "unspecified svctunnel get user info error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155483",
            Name = "XONLINE_E_SVCTUNNEL_SEND_USER_FEEDBACK_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified svctunnel send feedback error",
            CommonCause = "unspecified svctunnel send feedback error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155484",
            Name = "XONLINE_E_SVCTUNNEL_GET_GAMERTAG_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified svctunnel get gamertag error",
            CommonCause = "unspecified svctunnel get gamertag error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155485",
            Name = "XONLINE_E_SVCTUNNEL_GET_USER_ID_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified svctunnel get user id error",
            CommonCause = "unspecified svctunnel get user id error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155486",
            Name = "XONLINE_E_SVCTUNNEL_GET_FRIENDS_ERROR",
            Category = "Notification",
            Meaning = "unspecified svctunnel get friends error",
            CommonCause = "unspecified svctunnel get friends error",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155487",
            Name = "XONLINE_E_SVCTUNNEL_GET_MESSAGES_ERROR",
            Category = "Notification",
            Meaning = "unspecified svctunnel get messages error",
            CommonCause = "unspecified svctunnel get messages error",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155488",
            Name = "XONLINE_E_SVCTUNNEL_FRIEND_REQUEST_ERROR",
            Category = "Notification",
            Meaning = "unspecified svctunnel friend request error",
            CommonCause = "unspecified svctunnel friend request error",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155489",
            Name = "XONLINE_E_SVCTUNNEL_GAME_INVITE_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified svctunnel game invite error",
            CommonCause = "unspecified svctunnel game invite error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015548A",
            Name = "XONLINE_E_SVCTUNNEL_MUTE_USER_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified svctunnel mute user error",
            CommonCause = "unspecified svctunnel mute user error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015548B",
            Name = "XONLINE_E_SVCTUNNEL_SET_NOTIFICATION_ERROR",
            Category = "Notification",
            Meaning = "unspecified svctunnel set notification error",
            CommonCause = "unspecified svctunnel set notification error",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155500",
            Name = "XONLINE_E_XCRYPTO_ERROR",
            Category = "Xbox Live",
            Meaning = "Generic error",
            CommonCause = "Generic error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155501",
            Name = "XONLINE_E_XCRYPTO_CONFIG_ERROR",
            Category = "Xbox Live",
            Meaning = "NPDB configuration error.",
            CommonCause = "NPDB configuration error.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155502",
            Name = "XONLINE_E_XCRYPTO_KEY_ERROR",
            Category = "Xbox Live",
            Meaning = "Error creating crypto key.",
            CommonCause = "Error creating crypto key.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155503",
            Name = "XONLINE_E_XCRYPTO_CACHE_ERROR",
            Category = "Xbox Live",
            Meaning = "Error creating / refreshing a cache. Most likely due to a key error.",
            CommonCause = "Error creating / refreshing a cache. Most likely due to a key error.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155504",
            Name = "XONLINE_E_XCRYPTO_REQUEST_FAILED",
            Category = "Xbox Live",
            Meaning = "An xcrypto request has failed.",
            CommonCause = "An xcrypto request has failed.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155505",
            Name = "XONLINE_E_XCRYPTO_NCIPHER_ERROR",
            Category = "Xbox Live",
            Meaning = "Error calling down into nCipher wrap, which goes down to the nCipher hardware device.",
            CommonCause = "Error calling down into nCipher wrap, which goes down to the nCipher hardware device.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80155A01",
            Name = "XONLINE_E_MESSAGE_INVALID_MESSAGE_ID",
            Category = "Notification",
            Meaning = "the specified message was not found",
            CommonCause = "the specified message was not found",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155A02",
            Name = "XONLINE_E_MESSAGE_PROPERTY_DOWNLOAD_REQUIRED",
            Category = "Notification",
            Meaning = "the property was too large to fit into the details block, it must be retrieved separately using XOnlineMessageDownloadAttachmentxxx",
            CommonCause = "the property was too large to fit into the details block, it must be retrieved separately using XOnlineMessageDownloadAttachmentxxx",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155A03",
            Name = "XONLINE_E_MESSAGE_PROPERTY_NOT_FOUND",
            Category = "Notification",
            Meaning = "the specified property tag was not found",
            CommonCause = "the specified property tag was not found",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155A04",
            Name = "XONLINE_E_MESSAGE_NO_VALID_SENDS_TO_REVOKE",
            Category = "Notification",
            Meaning = "no valid sends to revoke were found",
            CommonCause = "no valid sends to revoke were found",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155A05",
            Name = "XONLINE_E_MESSAGE_NO_MESSAGE_DETAILS",
            Category = "Notification",
            Meaning = "the specified message does not have any details",
            CommonCause = "the specified message does not have any details",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155A06",
            Name = "XONLINE_E_MESSAGE_INVALID_TITLE_ID",
            Category = "Notification",
            Meaning = "an invalid title ID was specified",
            CommonCause = "an invalid title ID was specified",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155A07",
            Name = "XONLINE_E_MESSAGE_SENDER_BLOCKED",
            Category = "Notification",
            Meaning = "a send failed because the recipient has blocked the sender",
            CommonCause = "a send failed because the recipient has blocked the sender",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155A08",
            Name = "XONLINE_E_MESSAGE_MAX_DETAILS_SIZE_EXCEEDED",
            Category = "Notification",
            Meaning = "the property couldn’t be added because the maximum details size would be exceeded",
            CommonCause = "the property couldn’t be added because the maximum details size would be exceeded",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155A09",
            Name = "XONLINE_E_MESSAGE_INVALID_MESSAGE_TYPE",
            Category = "Notification",
            Meaning = "Messaginvalid Messagtype",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155A0A",
            Name = "XONLINE_E_MESSAGE_USER_OPTED_OUT",
            Category = "Notification",
            Meaning = "Messaguser Opted Out",
            CommonCause = "Friend, presence, invite, message, or notification service issue.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155A0B",
            Name = "XONLINE_E_MESSAGE_INSUFFICIENT_PRIVILEGES",
            Category = "Notification",
            Meaning = "the sender does not have permissions to send this message",
            CommonCause = "the sender does not have permissions to send this message",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155A0C",
            Name = "XONLINE_E_MESSAGE_UNDELIVERABLE",
            Category = "Notification",
            Meaning = "the recipient does not have permissions to receive this message",
            CommonCause = "the recipient does not have permissions to receive this message",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x80155A0D",
            Name = "XONLINE_E_MESSAGE_THROTTLED",
            Category = "Notification",
            Meaning = "the sender has sent too many messages today.",
            CommonCause = "the sender has sent too many messages today.",
            FixSuggestion = "Retry later, check friend/presence service status, and verify the target user/list state."
        },

        new()
        {
            Code = "0x00155A01",
            Name = "XONLINE_S_MESSAGE_PENDING_SYNC",
            Category = "Success",
            Meaning = "updated message list is currently being retrieved (after logon or disabling summary refresh), returned results may be out of date",
            CommonCause = "updated message list is currently being retrieved (after logon or disabling summary refresh), returned results may be out of date",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80156000",
            Name = "XONLINE_E_UODB_KEY_ALREADY_EXISTS",
            Category = "Xbox Live",
            Meaning = "service key already exists when attempting to insert key",
            CommonCause = "service key already exists when attempting to insert key",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156001",
            Name = "XONLINE_E_UODB_INEXISTENT_TITLE_ID",
            Category = "Xbox Live",
            Meaning = "Uodb Inexistent Titlid",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156002",
            Name = "XONLINE_E_UODB_KEY_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "Uodb Key Not Found",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156002",
            Name = "XONLINE_E_SERVICE_KEY_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "Servickey Not Found",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156003",
            Name = "XONLINE_E_UODB_INEXISTENT_OFFER_ID",
            Category = "Marketplace",
            Meaning = "Uodb Inexistent Offer Id",
            CommonCause = "Offer, license, entitlement, content availability, or region restriction issue.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80156004",
            Name = "XONLINE_E_SERVICE_KEY_IMPORT_ERROR",
            Category = "Network",
            Meaning = "Servickey Import Error",
            CommonCause = "DNS, NAT, gateway, Xbox Live endpoint, or local network connectivity issue.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80156005",
            Name = "XONLINE_E_UODB_INVALID_SUBSCRIPTION_PAYMENT_TYPE",
            Category = "Billing",
            Meaning = "Uodb Invalid Subscription Payment Type",
            CommonCause = "Payment method, purchase state, billing provider, points balance, or transaction issue.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80156006",
            Name = "XONLINE_E_UODB_DUPLICATE_SUBSCRIPTION_INFO",
            Category = "Xbox Live",
            Meaning = "Uodb Duplicatsubscription Info",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156100",
            Name = "XONLINE_E_QUERY_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified query error",
            CommonCause = "unspecified query error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156101",
            Name = "XONLINE_E_QUERY_QUOTA_FULL",
            Category = "Xbox Live",
            Meaning = "this user or team’s quota for the dataset is full. you must remove an entity first.",
            CommonCause = "this user or team’s quota for the dataset is full. you must remove an entity first.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156102",
            Name = "XONLINE_E_QUERY_ENTITY_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "the requested entity didn’t exist in the provided dataset.",
            CommonCause = "the requested entity didn’t exist in the provided dataset.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156103",
            Name = "XONLINE_E_QUERY_PERMISSION_DENIED",
            Category = "Security",
            Meaning = "the user tried to update or delete an entity that he didn’t own.",
            CommonCause = "the user tried to update or delete an entity that he didn’t own.",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x80156104",
            Name = "XONLINE_E_QUERY_ATTRIBUTE_TOO_LONG",
            Category = "Xbox Live",
            Meaning = "attribute passed exceeds schema definition",
            CommonCause = "attribute passed exceeds schema definition",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156105",
            Name = "XONLINE_E_QUERY_UNEXPECTED_ATTRIBUTE",
            Category = "Xbox Live",
            Meaning = "attribute passed was a bad param for the database operation",
            CommonCause = "attribute passed was a bad param for the database operation",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156106",
            Name = "XONLINE_E_QUERY_RETHROW_ERROR",
            Category = "Xbox Live",
            Meaning = "rethrow the original exception (used internally only)",
            CommonCause = "rethrow the original exception (used internally only)",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156107",
            Name = "XONLINE_E_QUERY_INVALID_ACTION",
            Category = "Validation",
            Meaning = "the specified action (or dataset) doesn’t have a select action associated with it.",
            CommonCause = "the specified action (or dataset) doesn’t have a select action associated with it.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80156108",
            Name = "XONLINE_E_QUERY_SPEC_COUNT_MISMATCH",
            Category = "Xbox Live",
            Meaning = "the provided number of QUERY_ATTRIBUTE_SPECs doesn’t match the number returned by the procedure",
            CommonCause = "the provided number of QUERY_ATTRIBUTE_SPECs doesn’t match the number returned by the procedure",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156109",
            Name = "XONLINE_E_QUERY_DATASET_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "The specified dataset id was not found.",
            CommonCause = "The specified dataset id was not found.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015610A",
            Name = "XONLINE_E_QUERY_PROCEDURE_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "The specified proc index was not found.",
            CommonCause = "The specified proc index was not found.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015610B",
            Name = "XONLINE_E_QUERY_DUPLICATE_ENTRY",
            Category = "Xbox Live",
            Meaning = "An entry already exists that conflicts with the unique data index specified for this dataset",
            CommonCause = "An entry already exists that conflicts with the unique data index specified for this dataset",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015610C",
            Name = "XONLINE_E_QUERY_RETRY",
            Category = "Xbox Live",
            Meaning = "Retry if possible",
            CommonCause = "Retry if possible",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156200",
            Name = "XONLINE_E_COMP_ERROR",
            Category = "Xbox Live",
            Meaning = "Unspecified comp error",
            CommonCause = "Unspecified comp error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156202",
            Name = "XONLINE_E_COMP_ACCESS_DENIED",
            Category = "Security",
            Meaning = "The specified source (client) is not permitted to execute this method",
            CommonCause = "The specified source (client) is not permitted to execute this method",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x80156203",
            Name = "XONLINE_E_COMP_REGISTRATION_CLOSED",
            Category = "Xbox Live",
            Meaning = "The competition is closed to registration",
            CommonCause = "The competition is closed to registration",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156204",
            Name = "XONLINE_E_COMP_FULL",
            Category = "Xbox Live",
            Meaning = "The competition has reached it’s max enrollment",
            CommonCause = "The competition has reached it’s max enrollment",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156205",
            Name = "XONLINE_E_COMP_NOT_REGISTERED",
            Category = "Xbox Live",
            Meaning = "The user or team isn’t registered for the competition",
            CommonCause = "The user or team isn’t registered for the competition",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156206",
            Name = "XONLINE_E_COMP_CANCELLED",
            Category = "Xbox Live",
            Meaning = "The competition has been cancelled, and the operation is invalid.",
            CommonCause = "The competition has been cancelled, and the operation is invalid.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156207",
            Name = "XONLINE_E_COMP_CHECKIN_TIME_INVALID",
            Category = "Validation",
            Meaning = "The user is attempting to checkin to an event outside the allowed time.",
            CommonCause = "The user is attempting to checkin to an event outside the allowed time.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80156208",
            Name = "XONLINE_E_COMP_CHECKIN_BAD_EVENT",
            Category = "Validation",
            Meaning = "The user is attempting to checkin to an event in which they are not a valid participant.",
            CommonCause = "The user is attempting to checkin to an event in which they are not a valid participant.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80156209",
            Name = "XONLINE_E_COMP_CHECKIN_EVENT_SCORED",
            Category = "Xbox Live",
            Meaning = "The user is attempting to checkin to an event which has already been scored by the service (user has forfeited or been ejected)",
            CommonCause = "The user is attempting to checkin to an event which has already been scored by the service (user has forfeited or been ejected)",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x00156209",
            Name = "XONLINE_S_COMP_CHECKIN_EVENT_SCORED",
            Category = "Success",
            Meaning = "The user is attempting to checkin to an event but the users event has been updated. Re-query for a new event",
            CommonCause = "The user is attempting to checkin to an event but the users event has been updated. Re-query for a new event",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80156210",
            Name = "XONLINE_E_COMP_UNEXPECTED",
            Category = "Xbox Live",
            Meaning = "Results from the Database are unexpected or inconsistent with the current operation.",
            CommonCause = "Results from the Database are unexpected or inconsistent with the current operation.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156216",
            Name = "XONLINE_E_COMP_TOPOLOGY_ERROR",
            Category = "Xbox Live",
            Meaning = "The topology request cannot be fulfilled by the server",
            CommonCause = "The topology request cannot be fulfilled by the server",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156217",
            Name = "XONLINE_E_COMP_TOPOLOGY_PENDING",
            Category = "Xbox Live",
            Meaning = "The topology request has not completed yet",
            CommonCause = "The topology request has not completed yet",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156218",
            Name = "XONLINE_E_COMP_CHECKIN_TOO_EARLY",
            Category = "Xbox Live",
            Meaning = "The user is attempting to checkin to an event outside the allowed time.",
            CommonCause = "The user is attempting to checkin to an event outside the allowed time.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80156219",
            Name = "XONLINE_E_COMP_ALREADY_REGISTERED",
            Category = "Xbox Live",
            Meaning = "The user has already registered for this competition",
            CommonCause = "The user has already registered for this competition",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015621A",
            Name = "XONLINE_E_COMP_INVALID_ENTRANT_TYPE",
            Category = "Validation",
            Meaning = "A team was specified for a non-team competition, or a user was specified for a team competition",
            CommonCause = "A team was specified for a non-team competition, or a user was specified for a team competition",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015621B",
            Name = "XONLINE_E_COMP_TOO_LATE",
            Category = "Xbox Live",
            Meaning = "The time alloted for performing the requested action has already passed.",
            CommonCause = "The time alloted for performing the requested action has already passed.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015621C",
            Name = "XONLINE_E_COMP_TOO_EARLY",
            Category = "Xbox Live",
            Meaning = "The specified action cannot yet be peformed.",
            CommonCause = "The specified action cannot yet be peformed.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015621D",
            Name = "XONLINE_E_COMP_NO_BYES_AVAILABLE",
            Category = "Xbox Live",
            Meaning = "No byes remain to be granted",
            CommonCause = "No byes remain to be granted",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015621E",
            Name = "XONLINE_E_COMP_SERVICE_OUTAGE",
            Category = "Xbox Live",
            Meaning = "A service outage has occured, try again in a bit",
            CommonCause = "A service outage has occured, try again in a bit",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x0000000F",
            Name = "XONLINE_S_COMP_SERVICE_OUTAGE",
            Category = "Success",
            Meaning = "A service outage was detected, evevnts were successfully rescheduled. (used by cron/logging only)",
            CommonCause = "A service outage was detected, evevnts were successfully rescheduled. (used by cron/logging only)",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80157001",
            Name = "XONLINE_E_MSGSVR_INVALID_REQUEST",
            Category = "Validation",
            Meaning = "request type was not one of the expected values",
            CommonCause = "request type was not one of the expected values",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80157100",
            Name = "XONLINE_E_STRING_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified error",
            CommonCause = "unspecified error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80157101",
            Name = "XONLINE_E_STRING_TOO_LONG",
            Category = "Xbox Live",
            Meaning = "the string was longer than the allowed maximum",
            CommonCause = "the string was longer than the allowed maximum",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80157102",
            Name = "XONLINE_E_STRING_OFFENSIVE_TEXT",
            Category = "Xbox Live",
            Meaning = "the string contains offensive text",
            CommonCause = "the string contains offensive text",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80157103",
            Name = "XONLINE_E_STRING_NO_DEFAULT_STRING",
            Category = "Xbox Live",
            Meaning = "returned by AddString when no string of the language specified as the default is found",
            CommonCause = "returned by AddString when no string of the language specified as the default is found",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80157104",
            Name = "XONLINE_E_STRING_INVALID_LANGUAGE",
            Category = "Validation",
            Meaning = "returned by AddString when an invalid language is specified for a string",
            CommonCause = "returned by AddString when an invalid language is specified for a string",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80157105",
            Name = "XONLINE_E_STRING_LANGUAGE_DUPLICATE",
            Category = "Xbox Live",
            Meaning = "returned by AddString when a language is specified more than once in a single request",
            CommonCause = "returned by AddString when a language is specified more than once in a single request",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80157106",
            Name = "XONLINE_E_STRING_ADD_STRING_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified error in stringsvr add string",
            CommonCause = "unspecified error in stringsvr add string",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80157107",
            Name = "XONLINE_E_STRING_GET_STRING_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified error in stringsvr get string",
            CommonCause = "unspecified error in stringsvr get string",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80157108",
            Name = "XONLINE_E_STRING_LOAD_STRING_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified error in stringsvr load string",
            CommonCause = "unspecified error in stringsvr load string",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80157109",
            Name = "XONLINE_E_STRING_LOG_STRING_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified error in stringsvr log string",
            CommonCause = "unspecified error in stringsvr log string",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015710A",
            Name = "XONLINE_E_STRING_TITLE_ID_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified error in stringsvr v1titleid",
            CommonCause = "unspecified error in stringsvr v1titleid",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015710B",
            Name = "XONLINE_E_STRING_VET_STRING_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified error in stringsvr vet string",
            CommonCause = "unspecified error in stringsvr vet string",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80157201",
            Name = "XONLINE_E_ALERTS_SUBSCRIPTION_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "user attempted to operate on a subscriptionid not present in the DB",
            CommonCause = "user attempted to operate on a subscriptionid not present in the DB",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80157202",
            Name = "XONLINE_E_ALERTS_SUBSCRIBER_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "user attempted to operate on a subscriber not present in the DB",
            CommonCause = "user attempted to operate on a subscriber not present in the DB",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80157300",
            Name = "XONLINE_E_SUPPORT_ERROR",
            Category = "Network",
            Meaning = "unspecified support error",
            CommonCause = "unspecified support error",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80158000",
            Name = "XONLINE_E_FEEDBACK_ERROR",
            Category = "Xbox Live",
            Meaning = "Feedback Error",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158001",
            Name = "XONLINE_E_FEEDBACK_NULL_TARGET",
            Category = "Xbox Live",
            Meaning = "Feedback Null Target",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158002",
            Name = "XONLINE_E_FEEDBACK_BAD_TYPE",
            Category = "Validation",
            Meaning = "Feedback Bad Type",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80158003",
            Name = "XONLINE_E_FEEDBACK_USER_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "Feedback User Not Found",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158006",
            Name = "XONLINE_E_FEEDBACK_CANNOT_LOG",
            Category = "Xbox Live",
            Meaning = "Feedback Cannot Log",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158007",
            Name = "XONLINE_E_FEEDBACK_REVIEW_INVALID",
            Category = "Validation",
            Meaning = "Feedback Review Invalid",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80158008",
            Name = "XONLINE_E_FEEDBACK_DATABASE_ERROR",
            Category = "Database",
            Meaning = "Feedback Databaserror",
            CommonCause = "Backend database error, timeout, duplicate key, deadlock, or invalid query result.",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x80158009",
            Name = "XONLINE_E_FEEDBACK_REVIEW_LIMIT_EXCEEDED",
            Category = "Xbox Live",
            Meaning = "Feedback Review Limit Exceeded",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015800A",
            Name = "XONLINE_E_FEEDBACK_GET_AGGREGATE_REVIEW_ERROR",
            Category = "Xbox Live",
            Meaning = "Feedback Get Aggregatreview Error",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015800B",
            Name = "XONLINE_E_FEEDBACK_USER_NOT_PRESENT",
            Category = "Xbox Live",
            Meaning = "Feedback User Not Present",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015800C",
            Name = "XONLINE_E_FEEDBACK_SUBMIT_COMPLAINT_ERROR",
            Category = "Xbox Live",
            Meaning = "Feedback Submit Complaint Error",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015800D",
            Name = "XONLINE_E_FEEDBACK_SUBMIT_REVIEW_ERROR",
            Category = "Xbox Live",
            Meaning = "Feedback Submit Review Error",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158100",
            Name = "XONLINE_E_LISTS_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) component error",
            CommonCause = "non-specific (catch-all) component error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158101",
            Name = "XONLINE_E_LISTS_PROPERTIES_TOO_LONG",
            Category = "Xbox Live",
            Meaning = "list properties XML exceeds maximum length (1000)",
            CommonCause = "list properties XML exceeds maximum length (1000)",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158102",
            Name = "XONLINE_E_LISTS_INVALID_XML",
            Category = "Validation",
            Meaning = "XML failed schema validation",
            CommonCause = "XML failed schema validation",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80158103",
            Name = "XONLINE_E_LISTS_MALFORMED_XML",
            Category = "Xbox Live",
            Meaning = "XML not well-formed",
            CommonCause = "XML not well-formed",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158104",
            Name = "XONLINE_E_LISTS_ID_MUST_BE_WILDCARD",
            Category = "Xbox Live",
            Meaning = "list ID must be wildcard when list type is wildcard",
            CommonCause = "list ID must be wildcard when list type is wildcard",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158105",
            Name = "XONLINE_E_LISTS_ITEM_TOO_LONG",
            Category = "Xbox Live",
            Meaning = "list item XML exceeds maximum length",
            CommonCause = "list item XML exceeds maximum length",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158106",
            Name = "XONLINE_E_LISTS_DUPLICATE_IDS",
            Category = "Xbox Live",
            Meaning = "arguments contain duplicate item IDs",
            CommonCause = "arguments contain duplicate item IDs",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158107",
            Name = "XONLINE_E_LISTS_EMPTY_PARAMETER",
            Category = "Xbox Live",
            Meaning = "null or empty parameter array",
            CommonCause = "null or empty parameter array",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158108",
            Name = "XONLINE_E_LISTS_NONEXISTENT_LIST_TYPE",
            Category = "Xbox Live",
            Meaning = "nonexistent list type",
            CommonCause = "nonexistent list type",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158109",
            Name = "XONLINE_E_LISTS_TOO_MANY_INSTANCES",
            Category = "Xbox Live",
            Meaning = "new list would exceed maximum instances",
            CommonCause = "new list would exceed maximum instances",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015810A",
            Name = "XONLINE_E_LISTS_NAME_TOO_LONG",
            Category = "Xbox Live",
            Meaning = "list name exceeds maximum length (100)",
            CommonCause = "list name exceeds maximum length (100)",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158110",
            Name = "XONLINE_E_LISTS_NONEXISTENT_LIST",
            Category = "Xbox Live",
            Meaning = "nonexistent list instance",
            CommonCause = "nonexistent list instance",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158111",
            Name = "XONLINE_E_LISTS_TOO_MANY_ITEMS",
            Category = "Xbox Live",
            Meaning = "new items would exceed maximum items",
            CommonCause = "new items would exceed maximum items",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158112",
            Name = "XONLINE_E_LISTS_DUPLICATE_ITEM",
            Category = "Xbox Live",
            Meaning = "item with given ID already exists in the list",
            CommonCause = "item with given ID already exists in the list",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158113",
            Name = "XONLINE_E_LISTS_NONEXISTENT_ITEM",
            Category = "Xbox Live",
            Meaning = "nonexistent list item",
            CommonCause = "nonexistent list item",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158114",
            Name = "XONLINE_E_LISTS_DUPLICATE_INDEX",
            Category = "Xbox Live",
            Meaning = "duplicate index given",
            CommonCause = "duplicate index given",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158115",
            Name = "XONLINE_E_LISTS_INVALID_INDEX",
            Category = "Validation",
            Meaning = "invalid index given",
            CommonCause = "invalid index given",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80158116",
            Name = "XONLINE_E_LISTS_DUPLICATE_LIST",
            Category = "Xbox Live",
            Meaning = "user already has list with given name",
            CommonCause = "user already has list with given name",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158117",
            Name = "XONLINE_E_LISTS_INVALID_PAGE_SIZE",
            Category = "Validation",
            Meaning = "invalid page size (1 <= pageSize <= max results)",
            CommonCause = "invalid page size (1 <= pageSize <= max results)",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80158118",
            Name = "XONLINE_E_LISTS_INCONSISTENT_VIEW",
            Category = "Xbox Live",
            Meaning = "timestamp argument does not match list timestamp",
            CommonCause = "timestamp argument does not match list timestamp",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158119",
            Name = "XONLINE_E_LISTS_INVALID_ORDER",
            Category = "Validation",
            Meaning = "invalid order direction (1 ascending, 2 descending)",
            CommonCause = "invalid order direction (1 ascending, 2 descending)",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80158120",
            Name = "XONLINE_E_LISTS_API_MISMATCH",
            Category = "Xbox Live",
            Meaning = "lists of this type should use a different API set",
            CommonCause = "lists of this type should use a different API set",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158180",
            Name = "XONLINE_E_LISTS_CREATE_LIST_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158181",
            Name = "XONLINE_E_LISTS_DELETE_ITEMS_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158182",
            Name = "XONLINE_E_LISTS_DELETE_LIST_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158183",
            Name = "XONLINE_E_LISTS_GRANT_ACCESS_ERROR",
            Category = "Security",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x80158184",
            Name = "XONLINE_E_LISTS_INSERT_ITEMS_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158185",
            Name = "XONLINE_E_LISTS_MODIFY_ITEMS_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158186",
            Name = "XONLINE_E_LISTS_MODIFY_LIST_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158187",
            Name = "XONLINE_E_LISTS_QUERY_ITEMS_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158188",
            Name = "XONLINE_E_LISTS_QUERY_LISTS_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80158189",
            Name = "XONLINE_E_LISTS_REVOKE_ACCESS_ERROR",
            Category = "Security",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x8015818A",
            Name = "XONLINE_E_LISTS_MOVE_ITEMS_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801581C0",
            Name = "XONLINE_E_DOWNLOAD_QUEUE_CREATE_QUEUE_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801581C1",
            Name = "XONLINE_E_DOWNLOAD_QUEUE_DELETE_ITEMS_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801581C2",
            Name = "XONLINE_E_DOWNLOAD_QUEUE_DELETE_QUEUE_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error (not used)",
            CommonCause = "non-specific (catch-all) api error (not used)",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801581C3",
            Name = "XONLINE_E_DOWNLOAD_QUEUE_GRANT_ACCESS_ERROR",
            Category = "Security",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x801581C4",
            Name = "XONLINE_E_DOWNLOAD_QUEUE_INSERT_ITEMS_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801581C5",
            Name = "XONLINE_E_DOWNLOAD_QUEUE_MODIFY_ITEMS_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801581C6",
            Name = "XONLINE_E_DOWNLOAD_QUEUE_MODIFY_QUEUE_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801581C7",
            Name = "XONLINE_E_DOWNLOAD_QUEUE_QUERY_ITEMS_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801581C8",
            Name = "XONLINE_E_DOWNLOAD_QUEUE_QUERY_QUEUES_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x801581C9",
            Name = "XONLINE_E_DOWNLOAD_QUEUE_REVOKE_ACCESS_ERROR",
            Category = "Security",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x801581CA",
            Name = "XONLINE_E_DOWNLOAD_QUEUE_MOVE_ITEMS_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) api error",
            CommonCause = "non-specific (catch-all) api error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80159000",
            Name = "XONLINE_E_STAT_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified stat error",
            CommonCause = "unspecified stat error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80159001",
            Name = "XONLINE_E_STAT_BAD_REQUEST",
            Category = "Validation",
            Meaning = "server received incorrectly formatted request.",
            CommonCause = "server received incorrectly formatted request.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80159002",
            Name = "XONLINE_E_STAT_INVALID_TITLE_OR_LEADERBOARD",
            Category = "Validation",
            Meaning = "title or leaderboard id were not recognized by the server.",
            CommonCause = "title or leaderboard id were not recognized by the server.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80159003",
            Name = "XONLINE_E_STAT_USER_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "user not found.",
            CommonCause = "user not found.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80159004",
            Name = "XONLINE_E_STAT_TOO_MANY_SPECS",
            Category = "Xbox Live",
            Meaning = "too many stat specs in a request.",
            CommonCause = "too many stat specs in a request.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80159005",
            Name = "XONLINE_E_STAT_TOO_MANY_STATS",
            Category = "Xbox Live",
            Meaning = "too manu stats in a spec.",
            CommonCause = "too manu stats in a spec.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80159100",
            Name = "XONLINE_E_STAT_SET_FAILED_0",
            Category = "Xbox Live",
            Meaning = "set operation failed on spec index 0",
            CommonCause = "set operation failed on spec index 0",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80159200",
            Name = "XONLINE_E_STAT_PERMISSION_DENIED",
            Category = "Security",
            Meaning = "operation failed because of credentials. UserId is not logged in or this operation is not supported in production (e.g. userId=0 in XOnlineStatReset)",
            CommonCause = "operation failed because of credentials. UserId is not logged in or this operation is not supported in production (e.g. userId=0 in XOnlineStatReset)",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x80159201",
            Name = "XONLINE_E_STAT_LEADERBOARD_WAS_RESET",
            Category = "Xbox Live",
            Meaning = "operation failed because user was logged on before the leaderboard was reset.",
            CommonCause = "operation failed because user was logged on before the leaderboard was reset.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80159202",
            Name = "XONLINE_E_STAT_INVALID_ATTACHMENT",
            Category = "Validation",
            Meaning = "attachment is invalid.",
            CommonCause = "attachment is invalid.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x00159203",
            Name = "XONLINE_S_STAT_CAN_UPLOAD_ATTACHMENT",
            Category = "Success",
            Meaning = "Use XOnlineStatWriteGetResults to get a handle to upload a attachment.",
            CommonCause = "Use XOnlineStatWriteGetResults to get a handle to upload a attachment.",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x80159204",
            Name = "XONLINE_E_STAT_TOO_MANY_PARAMETERS",
            Category = "Xbox Live",
            Meaning = "Stat Too Many Parameters",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80159205",
            Name = "XONLINE_E_STAT_TOO_MANY_PROCEDURES",
            Category = "Xbox Live",
            Meaning = "Stat Too Many Procedures",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80159206",
            Name = "XONLINE_E_STAT_STAT_POST_PROC_ERROR",
            Category = "Xbox Live",
            Meaning = "Stat Stat Post Proc Error",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80159208",
            Name = "XONLINE_E_STAT_NOT_ENOUGH_PARAMETERS",
            Category = "Xbox Live",
            Meaning = "Stat Not Enough Parameters",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80159209",
            Name = "XONLINE_E_STAT_INVALID_PROCEDURE",
            Category = "Validation",
            Meaning = "Stat Invalid Procedure",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015920A",
            Name = "XONLINE_E_STAT_EXCEEDED_WRITE_READ_LIMIT",
            Category = "Xbox Live",
            Meaning = "Stat Exceeded Writread Limit",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015920B",
            Name = "XONLINE_E_STAT_LEADERBOARD_READONLY",
            Category = "Xbox Live",
            Meaning = "Stat Leaderboard Readonly",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015920C",
            Name = "XONLINE_E_STAT_MUSIGMA_ARITHMETIC_OVERFLOW",
            Category = "Xbox Live",
            Meaning = "Stat Musigma Arithmetic Overflow",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015920D",
            Name = "XONLINE_E_STAT_READ_NO_SPEC",
            Category = "Xbox Live",
            Meaning = "Stat Read No Spec",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015920E",
            Name = "XONLINE_E_STAT_MUSIGMA_NO_GAME_MODE",
            Category = "Xbox Live",
            Meaning = "no game mode found for this leaderboard",
            CommonCause = "no game mode found for this leaderboard",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015920F",
            Name = "XONLINE_E_STAT_MISSING_RESULTS",
            Category = "Xbox Live",
            Meaning = "not enough results returned from lb server",
            CommonCause = "not enough results returned from lb server",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80159210",
            Name = "XONLINE_E_STAT_EXTRA_RESULTS",
            Category = "Xbox Live",
            Meaning = "too many results returned from lb server",
            CommonCause = "too many results returned from lb server",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80159211",
            Name = "XONLINE_E_STAT_SERVER_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "server not found",
            CommonCause = "server not found",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80159212",
            Name = "XONLINE_E_STAT_ACHIEVEMENTS_NOT_SUPPORTED",
            Category = "Network",
            Meaning = "achievements not supported for title/platform",
            CommonCause = "achievements not supported for title/platform",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80159213",
            Name = "XONLINE_E_STAT_AVATAR_ASSETS_NOT_SUPPORTED",
            Category = "Network",
            Meaning = "avatar assets not supported for title/platform",
            CommonCause = "avatar assets not supported for title/platform",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80159214",
            Name = "XONLINE_E_STAT_CONSOLE_LIST_NOT_SUPPORTED",
            Category = "Network",
            Meaning = "console audit list not supported for platform",
            CommonCause = "console audit list not supported for platform",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80159215",
            Name = "XONLINE_E_STAT_CONSOLE_LIST_EMPTY",
            Category = "Xbox Live",
            Meaning = "console audit list is empty",
            CommonCause = "console audit list is empty",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015A000",
            Name = "XSUPP_E_EMAIL_ALREADY_SENT",
            Category = "Xbox Live",
            Meaning = "Xsupp Email Already Sent",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015A001",
            Name = "XSUPP_E_INVALID_PUID_OR_SUB",
            Category = "Validation",
            Meaning = "Xsupp Invalid Puid Or Sub",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015A002",
            Name = "XSUPP_E_INVALID_EMAIL_SENT_DATE",
            Category = "Validation",
            Meaning = "Xsupp Invalid Email Sent Date",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015A003",
            Name = "XSUPP_E_INVALID_SUBSCRIPTION_CODE",
            Category = "Validation",
            Meaning = "Xsupp Invalid Subscription Code",
            CommonCause = "Invalid request data, identifier, parameter, product, user, or service component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015A004",
            Name = "XSUPP_E_USER_DOES_NOT_OWN_OFFER",
            Category = "Marketplace",
            Meaning = "Xsupp User Doenot Own Offer",
            CommonCause = "Offer, license, entitlement, content availability, or region restriction issue.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015A005",
            Name = "XSUPP_E_TOO_MANY_EXTRA_LICENSES",
            Category = "Marketplace",
            Meaning = "Xsupp Too Many Extra Licenses",
            CommonCause = "Offer, license, entitlement, content availability, or region restriction issue.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015A006",
            Name = "XSUPP_E_LICENSE_INCREMENT_GENERIC_ERROR",
            Category = "Marketplace",
            Meaning = "Xsupp Licensincrement Generic Error",
            CommonCause = "Offer, license, entitlement, content availability, or region restriction issue.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015A007",
            Name = "XSUPP_E_OWNER_PASSPORT_SWAP_CHILD_ONLY",
            Category = "Network",
            Meaning = "Xsupp Owner Passport Swap Child Only",
            CommonCause = "DNS, NAT, gateway, Xbox Live endpoint, or local network connectivity issue.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015A008",
            Name = "XSUPP_E_NO_NEW_PASSPORT_SPECIFIED",
            Category = "Network",
            Meaning = "Xsupp No New Passport Specified",
            CommonCause = "DNS, NAT, gateway, Xbox Live endpoint, or local network connectivity issue.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015A009",
            Name = "XSUPP_E_USER_PASSPORT_SWAP",
            Category = "Network",
            Meaning = "Xsupp User Passport Swap",
            CommonCause = "DNS, NAT, gateway, Xbox Live endpoint, or local network connectivity issue.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015A00A",
            Name = "XSUPP_E_OWNER_PASSPORT_SWAP",
            Category = "Network",
            Meaning = "Xsupp Owner Passport Swap",
            CommonCause = "DNS, NAT, gateway, Xbox Live endpoint, or local network connectivity issue.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015A00B",
            Name = "XSUPP_E_USER_DATE_OF_BIRTH_SWAP",
            Category = "Xbox Live",
            Meaning = "Xsupp User Datof Birth Swap",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015A00C",
            Name = "XSUPP_E_NEW_PASSPORT_MEMBER_NAME_MISMATCH",
            Category = "Network",
            Meaning = "Xsupp New Passport Member Nammismatch",
            CommonCause = "DNS, NAT, gateway, Xbox Live endpoint, or local network connectivity issue.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015A00D",
            Name = "XSUPP_E_INVALID_NEW_PASSPORT_MEMBER_NAME",
            Category = "Network",
            Meaning = "Xsupp Invalid New Passport Member Name",
            CommonCause = "DNS, NAT, gateway, Xbox Live endpoint, or local network connectivity issue.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015A00E",
            Name = "XSUPP_E_MULTIPLE_CREDENTIALS_FOR_THIS_MEMBER_NAME",
            Category = "Xbox Live",
            Meaning = "Xsupp Multiplcredentialfor Thimember Name",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015A00F",
            Name = "XSUPP_E_USER_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "Xsupp User Not Found",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015A010",
            Name = "XSUPP_E_USER_MACHINE_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "Xsupp User Machinnot Found",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015A011",
            Name = "XSUPP_E_USER_UNIQUE_MACHINE_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "Xsupp User Uniqumachinnot Found",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B000",
            Name = "XONLINE_E_SIGNATURE_ERROR",
            Category = "Xbox Live",
            Meaning = "unspecified signature error",
            CommonCause = "unspecified signature error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B001",
            Name = "XONLINE_E_SIGNATURE_VER_INVALID_SIGNATURE",
            Category = "Validation",
            Meaning = "presented signature does not match",
            CommonCause = "presented signature does not match",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015B002",
            Name = "XONLINE_E_SIGNATURE_VER_UNKNOWN_KEY_VER",
            Category = "Xbox Live",
            Meaning = "signature key version specified is not found among the valid signature keys",
            CommonCause = "signature key version specified is not found among the valid signature keys",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B003",
            Name = "XONLINE_E_SIGNATURE_VER_UNKNOWN_SIGNATURE_VER",
            Category = "Xbox Live",
            Meaning = "signature version is unknown, currently only version 1 is supported",
            CommonCause = "signature version is unknown, currently only version 1 is supported",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B004",
            Name = "XONLINE_E_SIGNATURE_BANNED_XBOX",
            Category = "Security",
            Meaning = "signature is not calculated or revoked because Xbox is banned",
            CommonCause = "signature is not calculated or revoked because Xbox is banned",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x8015B005",
            Name = "XONLINE_E_SIGNATURE_BANNED_USER",
            Category = "Security",
            Meaning = "signature is not calculated or revoked because at least one user is banned",
            CommonCause = "signature is not calculated or revoked because at least one user is banned",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x8015B006",
            Name = "XONLINE_E_SIGNATURE_BANNED_TITLE",
            Category = "Security",
            Meaning = "signature is not calculated or revoked because the given title and version is banned",
            CommonCause = "signature is not calculated or revoked because the given title and version is banned",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x8015B007",
            Name = "XONLINE_E_SIGNATURE_BANNED_DIGEST",
            Category = "Security",
            Meaning = "signature is not calculated or revoked because the digest is banned",
            CommonCause = "signature is not calculated or revoked because the digest is banned",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x8015B008",
            Name = "XONLINE_E_SIGNATURE_GET_BAD_AUTH_DATA",
            Category = "Authentication",
            Meaning = "fail to retrieve AuthData from SG, returned by GetSigningKey api",
            CommonCause = "fail to retrieve AuthData from SG, returned by GetSigningKey api",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015B009",
            Name = "XONLINE_E_SIGNATURE_SERVICE_UNAVAILABLE",
            Category = "Xbox Live",
            Meaning = "fail to retrieve a signature server master key, returned by GetSigningKey or SignOnBehalf api",
            CommonCause = "fail to retrieve a signature server master key, returned by GetSigningKey or SignOnBehalf api",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B00A",
            Name = "XONLINE_E_SIGNATURE_LICENSE_NOT_ACQUIRABLE",
            Category = "Marketplace",
            Meaning = "AcquireMediaLicenses will not be able to acquire the license without a state change (like repurchase)",
            CommonCause = "AcquireMediaLicenses will not be able to acquire the license without a state change (like repurchase)",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015B00B",
            Name = "XONLINE_E_SIGNATURE_LICENSE_COUNT_EXCEEDED",
            Category = "Marketplace",
            Meaning = "The user already has the limit of licenses allowed.",
            CommonCause = "The user already has the limit of licenses allowed.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015B00C",
            Name = "XONLINE_E_SIGNATURE_LICENSE_TRANSFER_BAD_COMMAND",
            Category = "Marketplace",
            Meaning = "Tried to send a command that is inconsistent with the curent state of the transfer.",
            CommonCause = "Tried to send a command that is inconsistent with the curent state of the transfer.",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015B00D",
            Name = "XONLINE_E_SIGNATURE_LICENSE_TRANSFER_UNAUTHORIZED",
            Category = "Authentication",
            Meaning = "The user requesting a transfer is not authorized due to a missing profile setting.",
            CommonCause = "The user requesting a transfer is not authorized due to a missing profile setting.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015B00E",
            Name = "XONLINE_E_SIGNATURE_CERTIFICATE_INVALID",
            Category = "Validation",
            Meaning = "this is what we return to the user for invalid certificates",
            CommonCause = "this is what we return to the user for invalid certificates",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015B00F",
            Name = "XONLINE_E_SIGNATURE_CERTIFICATE_USAGE_INVALID",
            Category = "Validation",
            Meaning = "this an internal only code",
            CommonCause = "this an internal only code",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015B010",
            Name = "XONLINE_E_SIGNATURE_CERTIFICATE_CHAIN_INVALID",
            Category = "Validation",
            Meaning = "this an internal only code",
            CommonCause = "this an internal only code",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015B011",
            Name = "XONLINE_E_SIGNATURE_CERTIFICATE_EXPIRED",
            Category = "Xbox Live",
            Meaning = "this an internal only code",
            CommonCause = "this an internal only code",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B012",
            Name = "XONLINE_E_SIGNATURE_PLAYREADY_DEVICE_CERT_EMPTY",
            Category = "Xbox Live",
            Meaning = "The playready device certificate we generated is empty (internal only)",
            CommonCause = "The playready device certificate we generated is empty (internal only)",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B013",
            Name = "XONLINE_E_SIGNATURE_PLAYREADY_DEVICE_CERT_TOO_BIG",
            Category = "Xbox Live",
            Meaning = "The playready device certificate we generated is too big (internal only)",
            CommonCause = "The playready device certificate we generated is too big (internal only)",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B014",
            Name = "XONLINE_E_SIGNATURE_PLAYREADY_DEVICE_CERT_FAILED",
            Category = "Xbox Live",
            Meaning = "Failed to generate a playready device certificate. This is what we return to the caller.",
            CommonCause = "Failed to generate a playready device certificate. This is what we return to the caller.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B080",
            Name = "XONLINE_E_SIGNATURE_ACKNOWLEDGE_LICENSE_DELIVERY_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific acknowldege license delivery error",
            CommonCause = "non-specific acknowldege license delivery error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015B081",
            Name = "XONLINE_E_SIGNATURE_ACQUIRE_MEDIA_LICENSES_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific acquire media licenses error",
            CommonCause = "non-specific acquire media licenses error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015B082",
            Name = "XONLINE_E_SIGNATURE_CREATE_CERTIFICATE_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific create certificate error",
            CommonCause = "non-specific create certificate error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B083",
            Name = "XONLINE_E_SIGNATURE_GET_AA_INFO_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific get aa info error",
            CommonCause = "non-specific get aa info error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B084",
            Name = "XONLINE_E_SIGNATURE_GET_SIGNED_HEADER_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific get signed header error",
            CommonCause = "non-specific get signed header error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B085",
            Name = "XONLINE_E_SIGNATURE_GENERATE_LICENSE_RESPONSE_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific generate license response error",
            CommonCause = "non-specific generate license response error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015B086",
            Name = "XONLINE_E_SIGNATURE_REFRESH_GAME_LICENSE_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific refresh game license error",
            CommonCause = "non-specific refresh game license error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015B087",
            Name = "XONLINE_E_SIGNATURE_TRANSFER_USER_LICENSES_ERROR",
            Category = "Marketplace",
            Meaning = "non-specific transfer user licenses error",
            CommonCause = "non-specific transfer user licenses error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015B089",
            Name = "XONLINE_E_SIGNATURE_VALIDATE_CERTIFICATE_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific validate certificate error",
            CommonCause = "non-specific validate certificate error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B101",
            Name = "XONLINE_E_ARBITRATION_SERVICE_UNAVAILABLE",
            Category = "Xbox Live",
            Meaning = "Service temporarily unavailable",
            CommonCause = "Service temporarily unavailable",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B102",
            Name = "XONLINE_E_ARBITRATION_INVALID_REQUEST",
            Category = "Validation",
            Meaning = "The request is invalidly formatted",
            CommonCause = "The request is invalidly formatted",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015B103",
            Name = "XONLINE_E_ARBITRATION_SESSION_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "The session is not found or has expired",
            CommonCause = "The session is not found or has expired",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B104",
            Name = "XONLINE_E_ARBITRATION_REGISTRATION_FLAGS_MISMATCH",
            Category = "Xbox Live",
            Meaning = "The session was registered with different flags by another Xbox",
            CommonCause = "The session was registered with different flags by another Xbox",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B105",
            Name = "XONLINE_E_ARBITRATION_REGISTRATION_SESSION_TIME_MISMATCH",
            Category = "Xbox Live",
            Meaning = "The session was registered with a different session time by another Xbox",
            CommonCause = "The session was registered with a different session time by another Xbox",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B106",
            Name = "XONLINE_E_ARBITRATION_REGISTRATION_TOO_LATE",
            Category = "Xbox Live",
            Meaning = "Registration came too late, the session has already been arbitrated",
            CommonCause = "Registration came too late, the session has already been arbitrated",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B107",
            Name = "XONLINE_E_ARBITRATION_NEED_TO_REGISTER_FIRST",
            Category = "Xbox Live",
            Meaning = "Must register in seesion first, before any other activity",
            CommonCause = "Must register in seesion first, before any other activity",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B108",
            Name = "XONLINE_E_ARBITRATION_TIME_EXTENSION_NOT_ALLOWED",
            Category = "Xbox Live",
            Meaning = "Time extension of this session not allowed, or session is already arbitrated",
            CommonCause = "Time extension of this session not allowed, or session is already arbitrated",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B109",
            Name = "XONLINE_E_ARBITRATION_INCONSISTENT_FLAGS",
            Category = "Xbox Live",
            Meaning = "Inconsistent flags are used in the request",
            CommonCause = "Inconsistent flags are used in the request",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B10A",
            Name = "XONLINE_E_ARBITRATION_INCONSISTENT_COMPETITION_STATUS",
            Category = "Xbox Live",
            Meaning = "Whether the session is a competition is inconsistent between registration and report",
            CommonCause = "Whether the session is a competition is inconsistent between registration and report",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B10B",
            Name = "XONLINE_E_ARBITRATION_REPORT_ALREADY_CALLED",
            Category = "Network",
            Meaning = "Report call for this session already made by this client",
            CommonCause = "Report call for this session already made by this client",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015B10C",
            Name = "XONLINE_E_ARBITRATION_TOO_MANY_XBOXES_IN_SESSION",
            Category = "Xbox Live",
            Meaning = "Only up to 255 Xboxes can register in a session",
            CommonCause = "Only up to 255 Xboxes can register in a session",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B10D",
            Name = "XONLINE_E_ARBITRATION_1_XBOX_1_USER_SESSION_NOT_ALLOWED",
            Category = "Xbox Live",
            Meaning = "Single Xbox single user sessions should not be arbitrated",
            CommonCause = "Single Xbox single user sessions should not be arbitrated",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015B10E",
            Name = "XONLINE_E_ARBITRATION_REPORT_TOO_LARGE",
            Category = "Network",
            Meaning = "The stats or query submission is too large",
            CommonCause = "The stats or query submission is too large",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015B10F",
            Name = "XONLINE_E_ARBITRATION_INVALID_TEAMTICKET",
            Category = "Teams",
            Meaning = "An invalid team ticket was submitted",
            CommonCause = "An invalid team ticket was submitted",
            FixSuggestion = "Check team capacity, membership, permissions, and moderation restrictions."
        },

        new()
        {
            Code = "0x0015B1F0",
            Name = "XONLINE_S_ARBITRATION_INVALID_XBOX_SPECIFIED",
            Category = "Success",
            Meaning = "Invalid/duplicate Xbox specified in lost connectivity or suspicious info. Never the less, this report is accepted",
            CommonCause = "Invalid/duplicate Xbox specified in lost connectivity or suspicious info. Never the less, this report is accepted",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x0015B1F1",
            Name = "XONLINE_S_ARBITRATION_INVALID_USER_SPECIFIED",
            Category = "Success",
            Meaning = "Invalid/duplicate user specified in lost connectivity or suspicious info. Never the less, this report is accepted",
            CommonCause = "Invalid/duplicate user specified in lost connectivity or suspicious info. Never the less, this report is accepted",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x0015B1F2",
            Name = "XONLINE_S_ARBITRATION_DIFFERENT_RESULTS_DETECTED",
            Category = "Success",
            Meaning = "Differing result submissions have been detected in this session. Never the less, this report submission is accepted",
            CommonCause = "Differing result submissions have been detected in this session. Never the less, this report submission is accepted",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x8015C000",
            Name = "XONLINE_E_STORAGE_ERROR",
            Category = "Storage",
            Meaning = "non-specific storage error",
            CommonCause = "non-specific storage error",
            FixSuggestion = "Reconnect storage, clear cache, redownload profile/content, and test another storage device."
        },

        new()
        {
            Code = "0x8015C001",
            Name = "XONLINE_E_STORAGE_INVALID_REQUEST",
            Category = "Storage",
            Meaning = "Request is invalid",
            CommonCause = "Request is invalid",
            FixSuggestion = "Reconnect storage, clear cache, redownload profile/content, and test another storage device."
        },

        new()
        {
            Code = "0x8015C002",
            Name = "XONLINE_E_STORAGE_ACCESS_DENIED",
            Category = "Security",
            Meaning = "Client doesn’t have the rights to upload the file",
            CommonCause = "Client doesn’t have the rights to upload the file",
            FixSuggestion = "Verify permissions, account/console status, trust/certificate state, and use official support for enforcement issues."
        },

        new()
        {
            Code = "0x8015C003",
            Name = "XONLINE_E_STORAGE_FILE_IS_TOO_BIG",
            Category = "Storage",
            Meaning = "File is too big",
            CommonCause = "File is too big",
            FixSuggestion = "Reconnect storage, clear cache, redownload profile/content, and test another storage device."
        },

        new()
        {
            Code = "0x8015C004",
            Name = "XONLINE_E_STORAGE_FILE_NOT_FOUND",
            Category = "Storage",
            Meaning = "File not found",
            CommonCause = "File not found",
            FixSuggestion = "Reconnect storage, clear cache, redownload profile/content, and test another storage device."
        },

        new()
        {
            Code = "0x8015C005",
            Name = "XONLINE_E_STORAGE_INVALID_ACCESS_TOKEN",
            Category = "Authentication",
            Meaning = "Access token signature is invalid",
            CommonCause = "Access token signature is invalid",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015C006",
            Name = "XONLINE_E_STORAGE_CANNOT_FIND_PATH",
            Category = "Storage",
            Meaning = "name resolution failed",
            CommonCause = "name resolution failed",
            FixSuggestion = "Reconnect storage, clear cache, redownload profile/content, and test another storage device."
        },

        new()
        {
            Code = "0x8015C007",
            Name = "XONLINE_E_STORAGE_FILE_IS_ELSEWHERE",
            Category = "Storage",
            Meaning = "redirection request",
            CommonCause = "redirection request",
            FixSuggestion = "Reconnect storage, clear cache, redownload profile/content, and test another storage device."
        },

        new()
        {
            Code = "0x8015C008",
            Name = "XONLINE_E_STORAGE_INVALID_STORAGE_PATH",
            Category = "Storage",
            Meaning = "Invalid storage path",
            CommonCause = "Invalid storage path",
            FixSuggestion = "Reconnect storage, clear cache, redownload profile/content, and test another storage device."
        },

        new()
        {
            Code = "0x8015C009",
            Name = "XONLINE_E_STORAGE_INVALID_FACILITY",
            Category = "Storage",
            Meaning = "Invalid facility code",
            CommonCause = "Invalid facility code",
            FixSuggestion = "Reconnect storage, clear cache, redownload profile/content, and test another storage device."
        },

        new()
        {
            Code = "0x8015C00A",
            Name = "XONLINE_E_STORAGE_UNKNOWN_DOMAIN",
            Category = "Storage",
            Meaning = "Name resolver has no idea where to send you.",
            CommonCause = "Name resolver has no idea where to send you.",
            FixSuggestion = "Reconnect storage, clear cache, redownload profile/content, and test another storage device."
        },

        new()
        {
            Code = "0x8015C00B",
            Name = "XONLINE_E_STORAGE_SYNC_TIME_SKEW",
            Category = "Storage",
            Meaning = "SyncDomain timestamp skew",
            CommonCause = "SyncDomain timestamp skew",
            FixSuggestion = "Reconnect storage, clear cache, redownload profile/content, and test another storage device."
        },

        new()
        {
            Code = "0x8015C00C",
            Name = "XONLINE_E_STORAGE_SYNC_TIME_SKEW_LOCALTIME",
            Category = "Storage",
            Meaning = "SyncDomain timestamp appears to be localtime",
            CommonCause = "SyncDomain timestamp appears to be localtime",
            FixSuggestion = "Reconnect storage, clear cache, redownload profile/content, and test another storage device."
        },

        new()
        {
            Code = "0x8015C00D",
            Name = "XONLINE_E_STORAGE_QUOTA_EXCEEDED",
            Category = "Storage",
            Meaning = "Quota exceeded for storage domain",
            CommonCause = "Quota exceeded for storage domain",
            FixSuggestion = "Reconnect storage, clear cache, redownload profile/content, and test another storage device."
        },

        new()
        {
            Code = "0x8015C011",
            Name = "XONLINE_E_STORAGE_FILE_ALREADY_EXISTS",
            Category = "Storage",
            Meaning = "File already exists and storage domain does not allow overwrites",
            CommonCause = "File already exists and storage domain does not allow overwrites",
            FixSuggestion = "Reconnect storage, clear cache, redownload profile/content, and test another storage device."
        },

        new()
        {
            Code = "0x8015C012",
            Name = "XONLINE_E_STORAGE_DATABASE_ERROR",
            Category = "Database",
            Meaning = "Unknown database error",
            CommonCause = "Unknown database error",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x0015C013",
            Name = "XONLINE_S_STORAGE_FILE_NOT_MODIFIED",
            Category = "Success",
            Meaning = "File hasn’t been modified since given date",
            CommonCause = "File hasn’t been modified since given date",
            FixSuggestion = "No action required unless the caller expected a different result."
        },

        new()
        {
            Code = "0x8015C014",
            Name = "XONLINE_E_STORAGE_INVALID_PATH",
            Category = "Storage",
            Meaning = "Invalid file path",
            CommonCause = "Invalid file path",
            FixSuggestion = "Reconnect storage, clear cache, redownload profile/content, and test another storage device."
        },

        new()
        {
            Code = "0x8015C015",
            Name = "XONLINE_E_STORAGE_TITLE_FILES_NOT_FOUND",
            Category = "Storage",
            Meaning = "No storage files were found for the specified title",
            CommonCause = "No storage files were found for the specified title",
            FixSuggestion = "Reconnect storage, clear cache, redownload profile/content, and test another storage device."
        },

        new()
        {
            Code = "0x8015CA0E",
            Name = "XONLINE_E_STORAGE_UNSUPPORTED_CONTENT_TYPE",
            Category = "Network",
            Meaning = "Storagunsupported Content Type",
            CommonCause = "DNS, NAT, gateway, Xbox Live endpoint, or local network connectivity issue.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015C100",
            Name = "XONLINE_E_LIVEINFO_ERROR",
            Category = "Xbox Live",
            Meaning = "Liveinfo Error",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015C101",
            Name = "XONLINE_E_LIVEINFO_HIVE_INVALID_CONFIG",
            Category = "Validation",
            Meaning = "Config name is invalid",
            CommonCause = "Config name is invalid",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015C102",
            Name = "XONLINE_E_LIVEINFO_HIVE_ERROR_LOADING_CONFIG",
            Category = "Xbox Live",
            Meaning = "Error occured loading config",
            CommonCause = "Error occured loading config",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015C103",
            Name = "XONLINE_E_LIVEINFO_CLIENT_ERROR",
            Category = "Xbox Live",
            Meaning = "General error for client failures",
            CommonCause = "General error for client failures",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015C200",
            Name = "XONLINE_E_LSP_ERROR",
            Category = "Xbox Live",
            Meaning = "Lsp Error",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015C201",
            Name = "XONLINE_E_LSP_BUCKET_INTERFACE_CONFIG_ERROR",
            Category = "Xbox Live",
            Meaning = "Bucket Interface not configured",
            CommonCause = "Bucket Interface not configured",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D000",
            Name = "XONLINE_E_PASSPORT_ERROR",
            Category = "Network",
            Meaning = "generic passport error for when we can’t find a mapping",
            CommonCause = "generic passport error for when we can’t find a mapping",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D001",
            Name = "XONLINE_E_PASSPORT_NAME_ALREADY_TAKEN",
            Category = "Network",
            Meaning = "Failed to create passport: name already taken",
            CommonCause = "Failed to create passport: name already taken",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D002",
            Name = "XONLINE_E_PASSPORT_WRONG_NAME_OR_PASSWORD",
            Category = "Network",
            Meaning = "Wrong password and/or membername dose not exist.",
            CommonCause = "Wrong password and/or membername dose not exist.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D003",
            Name = "XONLINE_E_PASSPORT_LOCKED_OUT",
            Category = "Network",
            Meaning = "The credential is locked out.",
            CommonCause = "The credential is locked out.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D004",
            Name = "XONLINE_E_PASSPORT_FORCE_RENAME",
            Category = "Network",
            Meaning = "The credential is in a forced renamed state",
            CommonCause = "The credential is in a forced renamed state",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D005",
            Name = "XONLINE_E_PASSPORT_FORCE_CHANGE_PASSWORD",
            Category = "Network",
            Meaning = "The password has to be changed.",
            CommonCause = "The password has to be changed.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D006",
            Name = "XONLINE_E_PASSPORT_FORCE_CHANGE_SQ_SA",
            Category = "Network",
            Meaning = "The secret question and answer has to be changed.",
            CommonCause = "The secret question and answer has to be changed.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D007",
            Name = "XONLINE_E_PASSPORT_PASSWORD_EXPIRED",
            Category = "Network",
            Meaning = "The password for the account has expired.",
            CommonCause = "The password for the account has expired.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D008",
            Name = "XONLINE_E_PASSPORT_REQUIRE_EMAIL_VALIDATION",
            Category = "Network",
            Meaning = "The account is blocked pending email address validation.",
            CommonCause = "The account is blocked pending email address validation.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D009",
            Name = "XONLINE_E_FORBIDDEN_WORD",
            Category = "Xbox Live",
            Meaning = "String contained words that are forbidden by namespace administrator, examine input.",
            CommonCause = "String contained words that are forbidden by namespace administrator, examine input.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D00A",
            Name = "XONLINE_E_PASSWORD_BLANK",
            Category = "Xbox Live",
            Meaning = "The password is blank.",
            CommonCause = "The password is blank.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D00B",
            Name = "XONLINE_E_PASSWORD_TOO_SHORT",
            Category = "Xbox Live",
            Meaning = "The password is too short",
            CommonCause = "The password is too short",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D00C",
            Name = "XONLINE_E_PASSWORD_TOO_LONG",
            Category = "Xbox Live",
            Meaning = "The password is too long",
            CommonCause = "The password is too long",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D00D",
            Name = "XONLINE_E_PASSWORD_CONTAINS_MEMBER_NAME",
            Category = "Xbox Live",
            Meaning = "The password contains the member name.",
            CommonCause = "The password contains the member name.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D00E",
            Name = "XONLINE_E_PASSWORD_CONTAINS_INVALID_CHARACTERS",
            Category = "Validation",
            Meaning = "The password contains invalid characters.",
            CommonCause = "The password contains invalid characters.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015D00F",
            Name = "XONLINE_E_SQ_CONTAINS_PASSWORD",
            Category = "Xbox Live",
            Meaning = "The secret question contains the password.",
            CommonCause = "The secret question contains the password.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D010",
            Name = "XONLINE_E_SA_CONTAINS_PASSWORD",
            Category = "Xbox Live",
            Meaning = "The answer for the secret question contains the password.",
            CommonCause = "The answer for the secret question contains the password.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D011",
            Name = "XONLINE_E_PASSWORD_CONTAINS_SA",
            Category = "Xbox Live",
            Meaning = "The password validation code detected the answer to the secret question in the password.",
            CommonCause = "The password validation code detected the answer to the secret question in the password.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D015",
            Name = "XONLINE_E_SQ_CONTAINS_SA",
            Category = "Xbox Live",
            Meaning = "The secret question contains the answer.",
            CommonCause = "The secret question contains the answer.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D017",
            Name = "XONLINE_E_SA_TOO_SHORT",
            Category = "Xbox Live",
            Meaning = "Sa Too Short",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D018",
            Name = "XONLINE_E_SA_CONTAINS_SQ",
            Category = "Xbox Live",
            Meaning = "The answer to the secret question contains that question.",
            CommonCause = "The answer to the secret question contains that question.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D019",
            Name = "XONLINE_E_SA_CONTAINS_MEMBER_NAME",
            Category = "Xbox Live",
            Meaning = "The answer for the secret question contains the member name.",
            CommonCause = "The answer for the secret question contains the member name.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D01A",
            Name = "XONLINE_E_MEMBER_NAME_TOO_SHORT",
            Category = "Xbox Live",
            Meaning = "The signin name is too short.",
            CommonCause = "The signin name is too short.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D01B",
            Name = "XONLINE_E_MEMBER_NAME_INVALID",
            Category = "Validation",
            Meaning = "The signin name is incomplete or has invalid characters.",
            CommonCause = "The signin name is incomplete or has invalid characters.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015D01C",
            Name = "XONLINE_E_PASSPORT_INVALID_DOMAIN",
            Category = "Network",
            Meaning = "Cannot create EASI passport on reserved domain (e.g. hotmail.com)",
            CommonCause = "Cannot create EASI passport on reserved domain (e.g. hotmail.com)",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D01D",
            Name = "XONLINE_E_PASSPORT_INVALID_POSTAL_CODE",
            Category = "Network",
            Meaning = "The postal code specified is invalid",
            CommonCause = "The postal code specified is invalid",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D01F",
            Name = "XONLINE_E_PASSPORT_SQ_TOO_SHORT",
            Category = "Network",
            Meaning = "Secret question is too short",
            CommonCause = "Secret question is too short",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D020",
            Name = "XONLINE_E_PASSPORT_SQ_TOO_LONG",
            Category = "Network",
            Meaning = "Secret question is too long",
            CommonCause = "Secret question is too long",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D021",
            Name = "XONLINE_E_PASSPORT_KIDS_ACCOUNT_NO_CONSENT",
            Category = "Network",
            Meaning = "The PP Kids account does not have consent",
            CommonCause = "The PP Kids account does not have consent",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D022",
            Name = "XONLINE_E_PASSPORT_SITE_NOT_AUTHORIZED",
            Category = "Authentication",
            Meaning = "xbox live service is not authorized to call the passport api",
            CommonCause = "xbox live service is not authorized to call the passport api",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015D080",
            Name = "XONLINE_E_PASSPORT_INVALID_ID",
            Category = "Network",
            Meaning = "passport could not find id (passport puid)",
            CommonCause = "passport could not find id (passport puid)",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D081",
            Name = "XONLINE_E_PASSPORT_INVALID_RESPONSE",
            Category = "Network",
            Meaning = "passport could not find id (passport puid)",
            CommonCause = "passport could not find id (passport puid)",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D082",
            Name = "XONLINE_E_PASSPORT_TOO_MANY_SECRET_QUESTIONS",
            Category = "Network",
            Meaning = "too many secret questions",
            CommonCause = "too many secret questions",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D083",
            Name = "XONLINE_E_PASSPORT_TIMEOUT",
            Category = "Network",
            Meaning = "timeout talking to passport",
            CommonCause = "timeout talking to passport",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D084",
            Name = "XONLINE_E_PASSPORT_NO_SECRET_QUESTIONS",
            Category = "Network",
            Meaning = "no secret questions for country and language",
            CommonCause = "no secret questions for country and language",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D085",
            Name = "XONLINE_E_PASSPORT_NO_DATA",
            Category = "Network",
            Meaning = "passport could not find any data to return",
            CommonCause = "passport could not find any data to return",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D086",
            Name = "XONLINE_E_PASSPORT_ASM_KEY_NOT_FOUND",
            Category = "Network",
            Meaning = "key sent to passport is not valid (old version, expired, etc.)",
            CommonCause = "key sent to passport is not valid (old version, expired, etc.)",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D100",
            Name = "XONLINE_E_UPS_ERROR",
            Category = "Xbox Live",
            Meaning = "error talking to UPS",
            CommonCause = "error talking to UPS",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D101",
            Name = "XONLINE_E_UPS_TIMEOUT_ERROR",
            Category = "Xbox Live",
            Meaning = "timeout error talking to UPS",
            CommonCause = "timeout error talking to UPS",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D102",
            Name = "XONLINE_E_UPS_GET_PROFILE_ERROR",
            Category = "Xbox Live",
            Meaning = "non-specific (catch-all) error calling get profile",
            CommonCause = "non-specific (catch-all) error calling get profile",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D103",
            Name = "XONLINE_E_UPS_UPDATE_PROFILE_ERROR",
            Category = "System Update",
            Meaning = "non-specific (catch-all) error calling update profile",
            CommonCause = "non-specific (catch-all) error calling update profile",
            FixSuggestion = "Update the dashboard/system files, verify title activation/update requirements, then retry."
        },

        new()
        {
            Code = "0x8015D200",
            Name = "XONLINE_E_RATINGS_UNKNOWNERROR",
            Category = "Xbox Live",
            Meaning = "Unhandled/unknownError",
            CommonCause = "Unhandled/unknownError",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D201",
            Name = "XONLINE_E_RATINGS_INVALID_COUNTRY",
            Category = "Validation",
            Meaning = "Invalid country passed",
            CommonCause = "Invalid country passed",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015D203",
            Name = "XONLINE_E_RATINGS_INVALID_USER",
            Category = "Validation",
            Meaning = "Invalid user not presetn in UODB",
            CommonCause = "Invalid user not presetn in UODB",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015D204",
            Name = "XONLINE_E_RATINGS_INVALID_RATING",
            Category = "Validation",
            Meaning = "Invalid Rating value (<0 or >5)",
            CommonCause = "Invalid Rating value (<0 or >5)",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015D205",
            Name = "XONLINE_E_RATINGS_INVALID_MEDIATYPE",
            Category = "Validation",
            Meaning = "Invalid Mediatype which cannot be rated",
            CommonCause = "Invalid Mediatype which cannot be rated",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015D206",
            Name = "XONLINE_E_RATINGS_MSN_ERROR",
            Category = "Xbox Live",
            Meaning = "General Error saving the rating",
            CommonCause = "General Error saving the rating",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D207",
            Name = "XONLINE_E_RATINGS_UNKNOWNCATALOG_ERROR",
            Category = "Xbox Live",
            Meaning = "Unknown Catalog error cannling getbasicmediainfo",
            CommonCause = "Unknown Catalog error cannling getbasicmediainfo",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D208",
            Name = "XONLINE_E_RATINGS_MULTISETTING_ERROR",
            Category = "Xbox Live",
            Meaning = "Error getting the Multisettign for ratings mediatypes",
            CommonCause = "Error getting the Multisettign for ratings mediatypes",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D209",
            Name = "XONLINE_E_RATINGS_MEDIATYPE_VALIDATION",
            Category = "Xbox Live",
            Meaning = "Error validatign the mediatype for ratigns",
            CommonCause = "Error validatign the mediatype for ratigns",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D20A",
            Name = "XONLINE_E_RATINGS_ERROR_GETUSER",
            Category = "Xbox Live",
            Meaning = "Error getting the user details",
            CommonCause = "Error getting the user details",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D20B",
            Name = "XONLINE_E_RATINGS_ERROR_MEDIAGET",
            Category = "Xbox Live",
            Meaning = "Error getting thr Media Information from Catalog",
            CommonCause = "Error getting thr Media Information from Catalog",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D20C",
            Name = "XONLINE_E_RATINGS_CONTENTTYPE_SETTING_ERROR",
            Category = "Marketplace",
            Meaning = "Error retrieving contentTypesetting from npdb",
            CommonCause = "Error retrieving contentTypesetting from npdb",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015D20D",
            Name = "XONLINE_E_RATINGS_CONTENTTYPE_MAPPING_ERROR",
            Category = "Marketplace",
            Meaning = "Error mapping contentType to Mediatype",
            CommonCause = "Error mapping contentType to Mediatype",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015D20E",
            Name = "XONLINE_E_RATINGS_MSN_CONNECTION_ERROR",
            Category = "Network",
            Meaning = "Error connecting to MSN Ratings service",
            CommonCause = "Error connecting to MSN Ratings service",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015D20F",
            Name = "XONLINE_E_RATINGS_CRON_UNKNOWN_ERROR",
            Category = "Xbox Live",
            Meaning = "Unknown Error in MSNRRAverages CRON plug-in",
            CommonCause = "Unknown Error in MSNRRAverages CRON plug-in",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D211",
            Name = "XONLINE_E_RATINGS_CRON_SAVE_AVERAGE_ERROR",
            Category = "Xbox Live",
            Meaning = "Error saving Rating averages",
            CommonCause = "Error saving Rating averages",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D212",
            Name = "XONLINE_E_RATINGS_CRON_TRANSACTION_DATE_ERROR",
            Category = "Xbox Live",
            Meaning = "Error retrieving TransactionDate",
            CommonCause = "Error retrieving TransactionDate",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D213",
            Name = "XONLINE_E_RATINGS_CRON_NPDB_SETTING_ERROR",
            Category = "Xbox Live",
            Meaning = "Error retrieving npdb setting",
            CommonCause = "Error retrieving npdb setting",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D214",
            Name = "XONLINE_E_RATINGS_CRON_TRANSACTION_DATE_SAVE_ERROR",
            Category = "Xbox Live",
            Meaning = "Error saving transaction date",
            CommonCause = "Error saving transaction date",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D215",
            Name = "XONLINE_E_RATINGS_MISSING_COUNTRY_CODE_ON_GET_MEDIA_INFO",
            Category = "Xbox Live",
            Meaning = "GetMediaInfo invoked without specifying countryCode when retrieving user aggregate",
            CommonCause = "GetMediaInfo invoked without specifying countryCode when retrieving user aggregate",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D216",
            Name = "XONLINE_E_RATINGS_MISSING_MEDIA_ID_ON_GET_MEDIA_INFO",
            Category = "Xbox Live",
            Meaning = "GetMediaInfo invoked with empty/null guid for media id",
            CommonCause = "GetMediaInfo invoked with empty/null guid for media id",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D217",
            Name = "XONLINE_E_RATINGS_INVALID_MEDIA",
            Category = "Validation",
            Meaning = "Media Queried for does not exist in the catalog",
            CommonCause = "Media Queried for does not exist in the catalog",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015D300",
            Name = "XONLINE_E_CONTENTINGESTION_UNKNOWNERROR",
            Category = "Marketplace",
            Meaning = "Unhandled/Unknown Error",
            CommonCause = "Unhandled/Unknown Error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015D301",
            Name = "XONLINE_E_CONTENTINGESTION_INVALIDARGS",
            Category = "Marketplace",
            Meaning = "Invalid Argument Specified",
            CommonCause = "Invalid Argument Specified",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8015D400",
            Name = "XONLINE_E_REFLECTOR_GENERIC_ERROR",
            Category = "Xbox Live",
            Meaning = "Generic error",
            CommonCause = "Generic error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D401",
            Name = "XONLINE_E_REFLECTOR_NO_TOKEN",
            Category = "Authentication",
            Meaning = "The user does not have a token for the specified network Id",
            CommonCause = "The user does not have a token for the specified network Id",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015D500",
            Name = "XONLINE_E_MIGRATEUSER_GENERIC_ERROR",
            Category = "Xbox Live",
            Meaning = "generic error",
            CommonCause = "generic error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D501",
            Name = "XONLINE_E_MIGRATEUSER_USER_DOES_NOT_EXIST_ERROR",
            Category = "Xbox Live",
            Meaning = "specified user does not exist",
            CommonCause = "specified user does not exist",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D502",
            Name = "XONLINE_E_MIGRATEUSER_FAILED_TO_LOAD_USER_ERROR",
            Category = "Xbox Live",
            Meaning = "failed to load the user for some reason.",
            CommonCause = "failed to load the user for some reason.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D503",
            Name = "XONLINE_E_MIGRATEUSER_FAILED_TO_WRITE_XML_ERROR",
            Category = "Xbox Live",
            Meaning = "failed to write the user’s data to xml for some reason.",
            CommonCause = "failed to write the user’s data to xml for some reason.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D504",
            Name = "XONLINE_E_MIGRATEUSER_FAILED_TO_READ_XML_ERROR",
            Category = "Xbox Live",
            Meaning = "failed to read the user’s data from xml for some reason.",
            CommonCause = "failed to read the user’s data from xml for some reason.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D505",
            Name = "XONLINE_E_MIGRATEUSER_FAILED_TO_CREATE_USER_ERROR",
            Category = "Xbox Live",
            Meaning = "failed to create the user for some reason.",
            CommonCause = "failed to create the user for some reason.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D506",
            Name = "XONLINE_E_MIGRATEUSER_USER_ALREADY_EXISTS_ERROR",
            Category = "Xbox Live",
            Meaning = "specified user already exists",
            CommonCause = "specified user already exists",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D507",
            Name = "XONLINE_E_MIGRATEUSER_WRITE_TO_PROD_ERROR",
            Category = "Xbox Live",
            Meaning = "can’t write a user to prod",
            CommonCause = "can’t write a user to prod",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D600",
            Name = "XONLINE_E_TESTFD_GENERIC_ERROR",
            Category = "Xbox Live",
            Meaning = "generic error",
            CommonCause = "generic error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D601",
            Name = "XONLINE_E_TESTFD_API_NOT_AVAILABLE_ERROR",
            Category = "Xbox Live",
            Meaning = "insufficient permissions to access this API",
            CommonCause = "insufficient permissions to access this API",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D602",
            Name = "XONLINE_E_TESTFD_INVALID_DATABASE_ERROR",
            Category = "Database",
            Meaning = "database specified could not be found",
            CommonCause = "database specified could not be found",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x8015D603",
            Name = "XONLINE_E_TESTFD_INVALID_TABLE_ERROR",
            Category = "Validation",
            Meaning = "table specified could not be found",
            CommonCause = "table specified could not be found",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015D604",
            Name = "XONLINE_E_TESTFD_INVALID_COLUMN_ERROR",
            Category = "Validation",
            Meaning = "column specified could not be found",
            CommonCause = "column specified could not be found",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015D605",
            Name = "XONLINE_E_TESTFD_INVALID_VALUE_ERROR",
            Category = "Validation",
            Meaning = "value for specified column invalid",
            CommonCause = "value for specified column invalid",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015D606",
            Name = "XONLINE_E_TESTFD_INVALID_SQL_ERROR",
            Category = "Database",
            Meaning = "invalid sql statement",
            CommonCause = "invalid sql statement",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x8015D607",
            Name = "XONLINE_E_TESTFD_BAD_COMMAND_ERROR",
            Category = "Validation",
            Meaning = "command failed",
            CommonCause = "command failed",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015D608",
            Name = "XONLINE_E_TESTFD_COMMAND_TIMEOUT_ERROR",
            Category = "Xbox Live",
            Meaning = "command took longer than expected to run",
            CommonCause = "command took longer than expected to run",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D609",
            Name = "XONLINE_E_TESTFD_BAD_PARTITION_ERROR",
            Category = "Validation",
            Meaning = "invalid partition parameter",
            CommonCause = "invalid partition parameter",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015D60A",
            Name = "XONLINE_E_TESTFD_BAD_PARTITION_HASH_TYPE_ERROR",
            Category = "Validation",
            Meaning = "invalid partition hashtype parameter",
            CommonCause = "invalid partition hashtype parameter",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015D60B",
            Name = "XONLINE_E_TESTFD_SQL_NO_VALUE_ERROR",
            Category = "Database",
            Meaning = "no value returned for a sql query",
            CommonCause = "no value returned for a sql query",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x8015D60C",
            Name = "XONLINE_E_TESTFD_NOT_ENOUGH_TITLES_PROPPED",
            Category = "Xbox Live",
            Meaning = "not enough titles are propped to the environment to fufil the request",
            CommonCause = "not enough titles are propped to the environment to fufil the request",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D700",
            Name = "XONLINE_E_SOCIAL_QUERY_GENERIC_ERROR",
            Category = "Xbox Live",
            Meaning = "generic error",
            CommonCause = "generic error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D800",
            Name = "XONLINE_E_CERT_GRABBER_FAILED_TO_GET_NEW_CERT",
            Category = "Xbox Live",
            Meaning = "failed to query/retrieve the latest cert",
            CommonCause = "failed to query/retrieve the latest cert",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D801",
            Name = "XONLINE_E_CERT_GRABBER_FAILED_TO_PARSE_NEW_CERT",
            Category = "Xbox Live",
            Meaning = "failed to parse the latest cert",
            CommonCause = "failed to parse the latest cert",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D802",
            Name = "XONLINE_E_CERT_GRABBER_FAILED_TO_UPDATE_CERT",
            Category = "System Update",
            Meaning = "failed to update npdb with the latest cert",
            CommonCause = "failed to update npdb with the latest cert",
            FixSuggestion = "Update the dashboard/system files, verify title activation/update requirements, then retry."
        },

        new()
        {
            Code = "0x8015D900",
            Name = "XONLINE_E_XTOU_GETTERMSOFUSE_GENERIC_ERROR",
            Category = "Xbox Live",
            Meaning = "generic error for GetTermsOfUse",
            CommonCause = "generic error for GetTermsOfUse",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D901",
            Name = "XONLINE_E_XTOU_GETTERMSOFUSE_URL_PARSE_ERROR",
            Category = "Xbox Live",
            Meaning = "GetTermsOfUse failed to parse the URL",
            CommonCause = "GetTermsOfUse failed to parse the URL",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D902",
            Name = "XONLINE_E_XTOU_GETTERMSOFUSE_URL_GET_FILE_ERROR",
            Category = "Xbox Live",
            Meaning = "GetTermsOfUse failed to retrieve a TOU file from the database",
            CommonCause = "GetTermsOfUse failed to retrieve a TOU file from the database",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015D903",
            Name = "XONLINE_E_XTOU_GETLANGUAGESFORCOUNTRY_GENERIC_ERROR",
            Category = "Xbox Live",
            Meaning = "generic error for GetLanguagesForCountry",
            CommonCause = "generic error for GetLanguagesForCountry",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E200",
            Name = "XONLINE_E_RPS_NOT_INITIALIZED",
            Category = "Xbox Live",
            Meaning = "RPS is not initialized. Other RPS methods can be called only after the RPS.Initialize method has succeeded.",
            CommonCause = "RPS is not initialized. Other RPS methods can be called only after the RPS.Initialize method has succeeded.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E201",
            Name = "XONLINE_E_RPS_FAILED_TO_CREATE_DOM",
            Category = "Xbox Live",
            Meaning = "Failed to create DOM object.",
            CommonCause = "Failed to create DOM object.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E202",
            Name = "XONLINE_E_RPS_INTERNAL_ERROR",
            Category = "Xbox Live",
            Meaning = "Internal program or unexpected error. Could also be caused by programming or configuration error.",
            CommonCause = "Internal program or unexpected error. Could also be caused by programming or configuration error.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E203",
            Name = "XONLINE_E_RPS_INVALID_OBJECT_ID",
            Category = "Validation",
            Meaning = "The object ID is invalid. This condition can be caused by an internal RPS error or an error from a custom component.",
            CommonCause = "The object ID is invalid. This condition can be caused by an internal RPS error or an error from a custom component.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015E204",
            Name = "XONLINE_E_RPS_OBJECT_ID_CANNOT_OVERWRITE",
            Category = "Xbox Live",
            Meaning = "The object ID cannot be overridden. Custom component only: this error is caused by an attempt to override an object that cannot be overridden.",
            CommonCause = "The object ID cannot be overridden. Custom component only: this error is caused by an attempt to override an object that cannot be overridden.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E205",
            Name = "XONLINE_E_RPS_FAILED_TO_TLS",
            Category = "Xbox Live",
            Meaning = "TLS (thread local storage) call failed. The system is in a bad state. TlsAlloc failed.",
            CommonCause = "TLS (thread local storage) call failed. The system is in a bad state. TlsAlloc failed.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E206",
            Name = "XONLINE_E_RPS_XML_FILE_ERROR",
            Category = "Xbox Live",
            Meaning = "XML file error. RPS has encountered an invalid XML configuration file.",
            CommonCause = "XML file error. RPS has encountered an invalid XML configuration file.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E207",
            Name = "XONLINE_E_RPS_READ_ONLY",
            Category = "Xbox Live",
            Meaning = "Property is read only. This error is caused by an attempt to write to a read-only property bag.",
            CommonCause = "Property is read only. This error is caused by an attempt to write to a read-only property bag.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E208",
            Name = "XONLINE_E_RPS_SERVER_CONFIG_ALREADY_INITTED",
            Category = "Xbox Live",
            Meaning = "The server configuration has already been initialized. This condition is caused by an internal RPS error.",
            CommonCause = "The server configuration has already been initialized. This condition is caused by an internal RPS error.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E209",
            Name = "XONLINE_E_RPS_INVALIDCONFIG",
            Category = "Validation",
            Meaning = "Invalid configuration.",
            CommonCause = "Invalid configuration.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015E20A",
            Name = "XONLINE_E_RPS_CERT_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "Certificate cannot be found. A certificate required for the operation was not found.",
            CommonCause = "Certificate cannot be found. A certificate required for the operation was not found.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E20B",
            Name = "XONLINE_E_RPS_SKIBUFFER_TOO_SMALL",
            Category = "Xbox Live",
            Meaning = "Buffer for subject key identifier (SKI) is too small. This condition is caused by an internal RPS error or an invalid certificate with a large SKI.",
            CommonCause = "Buffer for subject key identifier (SKI) is too small. This condition is caused by an internal RPS error or an invalid certificate with a large SKI.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E20C",
            Name = "XONLINE_E_RPS_FILE_TOO_LARGE",
            Category = "Xbox Live",
            Meaning = "File is too large. This condition is caused by large certificate file. The maximum size is 512 kilobytes.",
            CommonCause = "File is too large. This condition is caused by large certificate file. The maximum size is 512 kilobytes.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E20D",
            Name = "XONLINE_E_RPS_INVALID_DATATYPE",
            Category = "Validation",
            Meaning = "Data type is invalid. The data type is different from the expected data type.",
            CommonCause = "Data type is invalid. The data type is different from the expected data type.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015E20E",
            Name = "XONLINE_E_RPS_MORE_DATA",
            Category = "Xbox Live",
            Meaning = "Insufficient data buffer.",
            CommonCause = "Insufficient data buffer.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E20F",
            Name = "XONLINE_E_RPS_INVALID_SIGNATURE",
            Category = "Validation",
            Meaning = "Signatures do not match.",
            CommonCause = "Signatures do not match.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015E211",
            Name = "XONLINE_E_RPS_ENCRYPTEDKEY_TOO_LARGE",
            Category = "Xbox Live",
            Meaning = "The encrypted key data is too large. The maximum size is 1024 bytes.",
            CommonCause = "The encrypted key data is too large. The maximum size is 1024 bytes.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E212",
            Name = "XONLINE_E_RPS_DATA_INTEGRITY_CHECK_FAILED",
            Category = "Xbox Live",
            Meaning = "The data integrity check failed. There was a hash mismatch.",
            CommonCause = "The data integrity check failed. There was a hash mismatch.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E214",
            Name = "XONLINE_E_RPS_CERT_WITHOUT_PRIVATE_KEY",
            Category = "Xbox Live",
            Meaning = "The certificate used for decryption did not have private key.",
            CommonCause = "The certificate used for decryption did not have private key.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E215",
            Name = "XONLINE_E_RPS_NET_CONFIG_CACHE_ALREADY_INITTED",
            Category = "Xbox Live",
            Meaning = "Network configuration cache has already been initialized. Caused by an internal RPS error.",
            CommonCause = "Network configuration cache has already been initialized. Caused by an internal RPS error.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E216",
            Name = "XONLINE_E_RPS_DOMAIN_ATTRIBUTE_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "The requested domain attribute was not found in RPSNetwork.xml.",
            CommonCause = "The requested domain attribute was not found in RPSNetwork.xml.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E217",
            Name = "XONLINE_E_RPS_INVALIDDATA",
            Category = "Validation",
            Meaning = "The data to pack or unpack is not valid RPS data.",
            CommonCause = "The data to pack or unpack is not valid RPS data.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015E218",
            Name = "XONLINE_E_RPS_TICKET_NOT_INITIALIZED",
            Category = "Xbox Live",
            Meaning = "Ticket was not initialized.",
            CommonCause = "Ticket was not initialized.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E219",
            Name = "XONLINE_E_RPS_TICKET_CANNOT_BE_INITIALIZED_MORE_THAN_ONCE",
            Category = "Xbox Live",
            Meaning = "Ticket has already been initialized. A ticket object cannot be reused.",
            CommonCause = "Ticket has already been initialized. A ticket object cannot be reused.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E21A",
            Name = "XONLINE_E_RPS_SAML_ASSERTION_MISSINGDATA",
            Category = "Xbox Live",
            Meaning = "A SAML assertion or WebSSO ticket is missing a data member. Invalid assertion.",
            CommonCause = "A SAML assertion or WebSSO ticket is missing a data member. Invalid assertion.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E21B",
            Name = "XONLINE_E_RPS_INVALID_TIMEWINDOW",
            Category = "Validation",
            Meaning = "Invalid time window. The time window parameter in the site configuration or supplied as an input parameter is either too large or too small.",
            CommonCause = "Invalid time window. The time window parameter in the site configuration or supplied as an input parameter is either too large or too small.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015E21D",
            Name = "XONLINE_E_RPS_HTTP_BODY_REQUIRED",
            Category = "Xbox Live",
            Meaning = "The HTTP body is required to authenticate. The application should call the method again with the HTTP body.",
            CommonCause = "The HTTP body is required to authenticate. The application should call the method again with the HTTP body.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E21E",
            Name = "XONLINE_E_RPS_INVALID_TICKET_TYPE",
            Category = "Validation",
            Meaning = "The ticket type is invalid. This condition could be caused by an incorrect ticket type or a switching of the RPSAuth and RPSSecAuth cookies.",
            CommonCause = "The ticket type is invalid. This condition could be caused by an incorrect ticket type or a switching of the RPSAuth and RPSSecAuth cookies.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015E21F",
            Name = "XONLINE_E_RPS_INVALID_SLIDINGWINDOW",
            Category = "Validation",
            Meaning = "Sliding time window is invalid. The input parameter for the sliding time window must be smaller than the parameter for the time window. These values can come from site configuration or method arguments.",
            CommonCause = "Sliding time window is invalid. The input parameter for the sliding time window must be smaller than the parameter for the time window. These values can come from site configuration or method arguments.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015E220",
            Name = "XONLINE_E_RPS_REASON_INVALID_AUTHMETHOD",
            Category = "Authentication",
            Meaning = "The Validate call failed because the AuthMethod check failed.",
            CommonCause = "The Validate call failed because the AuthMethod check failed.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015E222",
            Name = "XONLINE_E_RPS_NO_SUCH_PROFILE_ATTRIBUTE",
            Category = "Xbox Live",
            Meaning = "The attribute index requested is greater than the attribute count in the profile schema.",
            CommonCause = "The attribute index requested is greater than the attribute count in the profile schema.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E223",
            Name = "XONLINE_E_RPS_INVALID_PROFILESCHEMA_TYPE",
            Category = "Validation",
            Meaning = "The data type requested is not defined in the profile schema. The data type is not supported.",
            CommonCause = "The data type requested is not defined in the profile schema. The data type is not supported.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015E224",
            Name = "XONLINE_E_RPS_FAILED_DOWNLOAD",
            Category = "Xbox Live",
            Meaning = "The RPS service failed to download RPSNetwork.xml.",
            CommonCause = "The RPS service failed to download RPSNetwork.xml.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E226",
            Name = "XONLINE_E_RPS_INVALID_SITEID",
            Category = "Validation",
            Meaning = "The SiteId does not match the Ticket TargetId or audience.",
            CommonCause = "The SiteId does not match the Ticket TargetId or audience.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015E227",
            Name = "XONLINE_E_RPS_BASE64DECODE_FAILED",
            Category = "Xbox Live",
            Meaning = "Failed to do base64 decoding.",
            CommonCause = "Failed to do base64 decoding.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E228",
            Name = "XONLINE_E_RPS_REASON_TIMEWINDOW_EXPIRED",
            Category = "Xbox Live",
            Meaning = "The Validate call failed because the time window expired.",
            CommonCause = "The Validate call failed because the time window expired.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E229",
            Name = "XONLINE_E_RPS_REASON_SLIDINGWINDOW_EXPIRED",
            Category = "Xbox Live",
            Meaning = "The Validate call failed because the sliding time window expired.",
            CommonCause = "The Validate call failed because the sliding time window expired.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E22A",
            Name = "XONLINE_E_RPS_CERT_INVALID_KEY_SPEC",
            Category = "Validation",
            Meaning = "The certificate private key has an invalid key spec. The key spec should be AT_KEYEXCHANGE.",
            CommonCause = "The certificate private key has an invalid key spec. The key spec should be AT_KEYEXCHANGE.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015E22B",
            Name = "XONLINE_E_RPS_INTERNAL_ERROR_CODE_UNSET_IN_EXCEPTION",
            Category = "Xbox Live",
            Meaning = "Internal program error.",
            CommonCause = "Internal program error.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E22C",
            Name = "XONLINE_E_RPS_REASON_INVALID_AUTHINSTANT_DATATYPE",
            Category = "Authentication",
            Meaning = "An invalid AuthInstant data type was encountered during time-window validation.",
            CommonCause = "An invalid AuthInstant data type was encountered during time-window validation.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015E22D",
            Name = "XONLINE_E_RPS_REASON_HTTPS_OR_ENCRYPTED_TICKET_NEEDED",
            Category = "Xbox Live",
            Meaning = "HTTPS or an encrypted ticket is needed.",
            CommonCause = "HTTPS or an encrypted ticket is needed.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E22E",
            Name = "XONLINE_E_RPS_REASON_INCORRECT_IV_BYTES",
            Category = "Xbox Live",
            Meaning = "HTTPS or an encrypted ticket is needed.",
            CommonCause = "HTTPS or an encrypted ticket is needed.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E22F",
            Name = "XONLINE_E_RPS_REASON_PASSPORT_F_ERROR_ENCOUNTERED",
            Category = "Network",
            Meaning = "Passport f-code error was encountered in the query string.",
            CommonCause = "Passport f-code error was encountered in the query string.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8015E230",
            Name = "XONLINE_E_RPS_NO_SESSION_KEY",
            Category = "Xbox Live",
            Meaning = "There is no session key in the ticket.",
            CommonCause = "There is no session key in the ticket.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E231",
            Name = "XONLINE_E_RPS_INVALID_COOKIE_NAME",
            Category = "Validation",
            Meaning = "The reserved cookie name is specified.",
            CommonCause = "The reserved cookie name is specified.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015E232",
            Name = "XONLINE_E_RPS_INVALID_AUTHPOLICY",
            Category = "Authentication",
            Meaning = "The AuthPolicy parameter in site configuration or input parameter is invalid or missing. Check the RPSNetwork.xml file for valid AuthPolicy names.",
            CommonCause = "The AuthPolicy parameter in site configuration or input parameter is invalid or missing. Check the RPSNetwork.xml file for valid AuthPolicy names.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8015E233",
            Name = "XONLINE_E_RPS_INVALID_ENCRYPT_ALGID",
            Category = "Validation",
            Meaning = "The encryption method algId is invalid.",
            CommonCause = "The encryption method algId is invalid.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015E234",
            Name = "XONLINE_E_RPS_REASON_POST_TICKET_TIMEWINDOW_EXPIRED",
            Category = "Xbox Live",
            Meaning = "Post ticket time window expired. Ticket could be reposted.",
            CommonCause = "Post ticket time window expired. Ticket could be reposted.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E235",
            Name = "XONLINE_E_RPS_TICKET_HAS_NO_SESSIONKEY",
            Category = "Xbox Live",
            Meaning = "The ticket does not have a session key.",
            CommonCause = "The ticket does not have a session key.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E400",
            Name = "XONLINE_E_RPSDATA_DATA_TOO_LARGE",
            Category = "Xbox Live",
            Meaning = "The data is larger than the RPSData limit.",
            CommonCause = "The data is larger than the RPSData limit.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E401",
            Name = "XONLINE_E_RPSDATA_INVALID_DATATYPE",
            Category = "Validation",
            Meaning = "The data type in the data schema is not supported.",
            CommonCause = "The data type in the data schema is not supported.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015E402",
            Name = "XONLINE_E_RPSDATA_MORE_DATA",
            Category = "Xbox Live",
            Meaning = "The data buffer is insufficient.",
            CommonCause = "The data buffer is insufficient.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8015E403",
            Name = "XONLINE_E_RPSDATA_INVALID_DATAOFFSET",
            Category = "Validation",
            Meaning = "The data offset is too large or invalid.",
            CommonCause = "The data offset is too large or invalid.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8015E404",
            Name = "XONLINE_E_RPSDATA_INVALIDDATA",
            Category = "Validation",
            Meaning = "The data is invalid.",
            CommonCause = "The data is invalid.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8016148B",
            Name = "XONLINE_E_BILLING_PAYMENT_INSTRUMENT_CHANGES_RESTRICTED",
            Category = "Billing",
            Meaning = "Payment instrument changes are restricted through user (or tier or offer) privilege",
            CommonCause = "Payment instrument changes are restricted through user (or tier or offer) privilege",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8016148C",
            Name = "XONLINE_E_BILLING_PASSPORT_SWITCHING_RESTRICTED",
            Category = "Network",
            Meaning = "Payment instrument changes are restricted through user (or tier or offer) privilege",
            CommonCause = "Payment instrument changes are restricted through user (or tier or offer) privilege",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8016A0FA",
            Name = "XONLINE_E_BILLING_EXCEEDS_MAXIMUM_DURATION",
            Category = "Billing",
            Meaning = "Error subscription duration exceeds max duration.",
            CommonCause = "Error subscription duration exceeds max duration.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80167611",
            Name = "XONLINE_E_BILLING_AUTHORIZATION_FAILED",
            Category = "Authentication",
            Meaning = "Credit card authorization failed.",
            CommonCause = "Credit card authorization failed.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8016A04B",
            Name = "XONLINE_E_BILLING_TOKEN_NOT_VALID_FOR_OFFERING",
            Category = "Authentication",
            Meaning = "The token Id specified is not valid for the given offering.",
            CommonCause = "The token Id specified is not valid for the given offering.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8016EA71",
            Name = "XONLINE_E_BILLING_STATE_ZIP_CITY_INVALID2",
            Category = "Billing",
            Meaning = "VERAZIP: Invalid state code/ZIP code/city name combination.",
            CommonCause = "VERAZIP: Invalid state code/ZIP code/city name combination.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80164E75",
            Name = "XONLINE_E_BILLING_DUPLICATE_TRACKING_GUID",
            Category = "Billing",
            Meaning = "Duplicate tracking GUID.",
            CommonCause = "Duplicate tracking GUID.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x801675C8",
            Name = "XONLINE_E_BILLING_INVALID_PAYMENT_INSTRUMENT_TYPE",
            Category = "Billing",
            Meaning = "Invalid payment instrument type.",
            CommonCause = "Invalid payment instrument type.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80167531",
            Name = "XONLINE_E_BILLING_CREDIT_CARD_EXPIRED",
            Category = "Billing",
            Meaning = "Credit card has already expired.",
            CommonCause = "Credit card has already expired.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80169CF9",
            Name = "XONLINE_E_BILLING_INVALID_PAYMENT_METHOD_ID",
            Category = "Billing",
            Meaning = "Invalid payment instrument ID.",
            CommonCause = "Invalid payment instrument ID.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80169E89",
            Name = "XONLINE_E_BILLING_OFFERING_REQUIRES_PI",
            Category = "Billing",
            Meaning = "Offering requires a payment instrument.",
            CommonCause = "Offering requires a payment instrument.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80169E4D",
            Name = "XONLINE_E_BILLING_INVALID_CONVERSION",
            Category = "Billing",
            Meaning = "Either there is no path between the current offering and the target offering or the path does not match the calling mode (Convert or Renew).",
            CommonCause = "Either there is no path between the current offering and the target offering or the path does not match the calling mode (Convert or Renew).",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x801675C1",
            Name = "XONLINE_E_BILLING_COUNTRY_CURRENCY_PI_MISMATCH",
            Category = "Billing",
            Meaning = "Country/currency/payment instrument type mismatch.",
            CommonCause = "Country/currency/payment instrument type mismatch.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8016EA70",
            Name = "XONLINE_E_BILLING_STATE_ZIP_CITY_INVALID",
            Category = "Billing",
            Meaning = "VERAZIP: Invalid state code/ZIP code/city name combinations. Both state code/ZIP code and state code/city name were incorrect.",
            CommonCause = "VERAZIP: Invalid state code/ZIP code/city name combinations. Both state code/ZIP code and state code/city name were incorrect.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80169EA2",
            Name = "XONLINE_E_BILLING_INVALID_EMAIL_ADDRESS",
            Category = "Billing",
            Meaning = "Invalid e-mail address.",
            CommonCause = "Invalid e-mail address.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80167530",
            Name = "XONLINE_E_BILLING_INVALID_CREDIT_CARD_NUMBER",
            Category = "Billing",
            Meaning = "Invalid credit card number.",
            CommonCause = "Invalid credit card number.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8016EA6E",
            Name = "XONLINE_E_BILLING_STATE_ZIP_INVALID",
            Category = "Billing",
            Meaning = "VERAZIP: Invalid state code/ZIP code combination.",
            CommonCause = "VERAZIP: Invalid state code/ZIP code combination.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8016767F",
            Name = "XONLINE_E_BILLING_PAYMENT_PROVIDER_CONNECTION_TIMEOUT",
            Category = "Network",
            Meaning = "Payment provider connection timed out.",
            CommonCause = "Payment provider connection timed out.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x8016271F",
            Name = "XONLINE_E_BILLING_UNKNOWN_SERVER_FAILURE",
            Category = "Billing",
            Meaning = "Unknown server failure. API name: %2, Error code: 0x%3, Error description: %1.",
            CommonCause = "Unknown server failure. API name: %2, Error code: 0x%3, Error description: %1.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80162711",
            Name = "XONLINE_E_BILLING_NOPERMISSION",
            Category = "Billing",
            Meaning = "Access denied.",
            CommonCause = "Access denied.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80169C45",
            Name = "XONLINE_E_BILLING_REQUIRED_FIELD_MISSING",
            Category = "Billing",
            Meaning = "Required field missing.",
            CommonCause = "Required field missing.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8016EA8A",
            Name = "XONLINE_E_BILLING_MULTIPLE_CITIES_FOUND",
            Category = "Billing",
            Meaning = "VERAZIP: City has multiple ZIP codes.",
            CommonCause = "VERAZIP: City has multiple ZIP codes.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8016EA6C",
            Name = "XONLINE_E_BILLING_STATE_INVALID",
            Category = "Billing",
            Meaning = "VERAZIP: Invalid state code.",
            CommonCause = "VERAZIP: Invalid state code.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8016EA6B",
            Name = "XONLINE_E_BILLING_ZIP_INVALID",
            Category = "Billing",
            Meaning = "VERAZIP: Invalid ZIP code.",
            CommonCause = "VERAZIP: Invalid ZIP code.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80162725",
            Name = "XONLINE_E_BILLING_BADZIP",
            Category = "Billing",
            Meaning = "Invalid ZIP code.",
            CommonCause = "Invalid ZIP code.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x801613C9",
            Name = "XONLINE_E_BILLING_USAGE_COUNT_FOR_TOKEN_EXCEEDED",
            Category = "Authentication",
            Meaning = "The usage for the specified token has been exceeded.",
            CommonCause = "The usage for the specified token has been exceeded.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8016763E",
            Name = "XONLINE_E_BILLING_DD_INVALID_BRANCHCODE_FORMAT",
            Category = "Billing",
            Meaning = "Invalid branch code format.",
            CommonCause = "Invalid branch code format.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8016ABA5",
            Name = "XONLINE_E_BILLING_ADDITIONAL_ACCOUNT_DATA_REQUIRED",
            Category = "Billing",
            Meaning = "The account requires additional attributes before performing the operation.",
            CommonCause = "The account requires additional attributes before performing the operation.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80167684",
            Name = "XONLINE_E_BILLING_INVALID_WHOLESALE_PARTNER",
            Category = "Billing",
            Meaning = "The wholesale partner specified is invalid.",
            CommonCause = "The wholesale partner specified is invalid.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80169DD1",
            Name = "XONLINE_E_BILLING_ACCOUNT_CLOSED",
            Category = "Billing",
            Meaning = "Account is already closed.",
            CommonCause = "Account is already closed.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x8016138D",
            Name = "XONLINE_E_BILLING_INVALID_TOKEN_SPECIFIED",
            Category = "Authentication",
            Meaning = "Token passed is Invalid.",
            CommonCause = "Token passed is Invalid.",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x8016CE52",
            Name = "XONLINE_E_BILLING_UNKNOWN_ERROR",
            Category = "Billing",
            Meaning = "Billing Unknown Error",
            CommonCause = "Payment method, purchase state, billing provider, points balance, or transaction issue.",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80170000",
            Name = "XONLINE_E_PUID_IS_MACHINE",
            Category = "Xbox Live",
            Meaning = "Puid Imachine",
            CommonCause = "Xbox Live service, title, request, task, or server-side issue.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80170100",
            Name = "TOOLS_SERVER_E_FILE_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "tools service failed to find a file on file share to be propped to the data center",
            CommonCause = "tools service failed to find a file on file share to be propped to the data center",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80170101",
            Name = "TOOLS_SERVER_E_SD_ADD_FAILED",
            Category = "Xbox Live",
            Meaning = "tools service failed to add a file to title manager repository",
            CommonCause = "tools service failed to add a file to title manager repository",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80170102",
            Name = "TOOLS_SERVER_E_SD_SUBMIT_FAILED",
            Category = "Xbox Live",
            Meaning = "tools service failed to submit a file into title manager repository",
            CommonCause = "tools service failed to submit a file into title manager repository",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80170103",
            Name = "TOOLS_SERVER_E_TMR_STATS",
            Category = "Xbox Live",
            Meaning = "title manager failed to prop the package",
            CommonCause = "title manager failed to prop the package",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80170104",
            Name = "TOOLS_SERVER_E_INTERRUPTED_TASK",
            Category = "Xbox Live",
            Meaning = "tools service was stopped while processing a job. The job should be rolled back manually",
            CommonCause = "tools service was stopped while processing a job. The job should be rolled back manually",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80170105",
            Name = "TOOLS_SERVER_E_INVALID_TITLE_ID",
            Category = "Validation",
            Meaning = "tools service cannot process the job because title id is invalid",
            CommonCause = "tools service cannot process the job because title id is invalid",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80171001",
            Name = "XONLINE_E_LIVECACHE_EMPTY_RESULT",
            Category = "Xbox Live",
            Meaning = "live service replied with empty result, this should not happen",
            CommonCause = "live service replied with empty result, this should not happen",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80171002",
            Name = "XONLINE_E_LIVECACHE_FORWARD_FAILED",
            Category = "Xbox Live",
            Meaning = "failed to forward the request to live service: failed to connect or not getting response",
            CommonCause = "failed to forward the request to live service: failed to connect or not getting response",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80171003",
            Name = "XONLINE_E_LIVECACHE_OFFLINE",
            Category = "Xbox Live",
            Meaning = "the requested service has been turned off at livecache",
            CommonCause = "the requested service has been turned off at livecache",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80171004",
            Name = "XONLINE_E_LIVECACHE_USER_OVERHEAT",
            Category = "Xbox Live",
            Meaning = "too many requests from this same user within the current hour",
            CommonCause = "too many requests from this same user within the current hour",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80171005",
            Name = "XONLINE_E_LIVECACHE_INEXIST_KEY",
            Category = "Xbox Live",
            Meaning = "the key specified in LoadData request was not found, timed out?",
            CommonCause = "the key specified in LoadData request was not found, timed out?",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80171006",
            Name = "XONLINE_E_LIVECACHE_MAX_HEADER_COLLECTION_LIMIT",
            Category = "Xbox Live",
            Meaning = "max header limit exceeded.",
            CommonCause = "max header limit exceeded.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80172000",
            Name = "XONLINE_E_WCMUSIC_TEST_FAULT",
            Category = "MusicNet",
            Meaning = "returned by TestConnection in order to test throwing faults",
            CommonCause = "returned by TestConnection in order to test throwing faults",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80172001",
            Name = "XONLINE_E_WCMUSIC_ACCOUNT_SUSPENDED",
            Category = "MusicNet",
            Meaning = "returned by orderItems if account is suspended because of unconfirmed free trial",
            CommonCause = "returned by orderItems if account is suspended because of unconfirmed free trial",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80172002",
            Name = "XONLINE_E_WCMUSIC_INSUFFICIENT_BALANCE",
            Category = "MusicNet",
            Meaning = "returned by orderItems if no points available",
            CommonCause = "returned by orderItems if no points available",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80172003",
            Name = "XONLINE_E_WCMUSIC_ITEM_ALREADY_PURCHASED",
            Category = "Billing",
            Meaning = "returned by orderItems if repurchaseOverride is false, which it never is currently",
            CommonCause = "returned by orderItems if repurchaseOverride is false, which it never is currently",
            FixSuggestion = "Verify billing/payment details, account region, purchase state, and retry through official account tools."
        },

        new()
        {
            Code = "0x80172004",
            Name = "XONLINE_E_WCMUSIC_ITEM_UNAVAILABLE",
            Category = "MusicNet",
            Meaning = "returned by orderItems if the component is unavailable from MusicNet",
            CommonCause = "returned by orderItems if the component is unavailable from MusicNet",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80172005",
            Name = "XONLINE_E_WCMUSIC_TOO_MANY_ITEMS",
            Category = "MusicNet",
            Meaning = "returnded by orderItems if item count > 100",
            CommonCause = "returnded by orderItems if item count > 100",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80172006",
            Name = "XONLINE_E_WCMUSIC_NOT_SUSPENDED_BY_PARTNER",
            Category = "MusicNet",
            Meaning = "returned by orderItems if account is suspended manually by Customer Service Rep, or returned by resumeAccount if account is suspended manually by a Customer Service Rep",
            CommonCause = "returned by orderItems if account is suspended manually by Customer Service Rep, or returned by resumeAccount if account is suspended manually by a Customer Service Rep",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80172007",
            Name = "XONLINE_E_WCMUSIC_ACCOUNT_NOT_ELIGIBLE",
            Category = "MusicNet",
            Meaning = "returned by AuthenticateAccount if user is not eligible for the Argo service due to country, age, restrictions, etc.",
            CommonCause = "returned by AuthenticateAccount if user is not eligible for the Argo service due to country, age, restrictions, etc.",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80172008",
            Name = "XONLINE_E_WCMUSIC_ACCOUNT_NOT_PROVISIONED",
            Category = "MusicNet",
            Meaning = "returned by AuthenticateAccount if user has not been provisioned for Argo service",
            CommonCause = "returned by AuthenticateAccount if user has not been provisioned for Argo service",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80172009",
            Name = "XONLINE_E_WCMUSIC_INVALID_ARGUMENT",
            Category = "MusicNet",
            Meaning = "returned when an argument passed in to the API was not allowed/expected.",
            CommonCause = "returned when an argument passed in to the API was not allowed/expected.",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x8017200A",
            Name = "XONLINE_E_WCMUSIC_TRANSACTION_PENDING",
            Category = "MusicNet",
            Meaning = "returned from OrderItems when something went wrong and the purchase transaction is in an inderterminate state. Calling GetOrderByExternalOrderId should be called to get the updated status on the transaction.",
            CommonCause = "returned from OrderItems when something went wrong and the purchase transaction is in an inderterminate state. Calling GetOrderByExternalOrderId should be called to get the updated status on the transaction.",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x8017200B",
            Name = "XONLINE_E_WCMUSIC_DUPLICATE_EXTERNAL_ORDER_ID",
            Category = "MusicNet",
            Meaning = "returned from OrderItems if a duplicate orderId was passed into wcmusic. Calling GetOrderByExternalOrderId should be called to get the updated status on that transaction, if needed. Otherwise a new externalOrderID needs to be generated.",
            CommonCause = "returned from OrderItems if a duplicate orderId was passed into wcmusic. Calling GetOrderByExternalOrderId should be called to get the updated status on that transaction, if needed. Otherwise a new externalOrderID needs to be generated.",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x8017200C",
            Name = "XONLINE_E_WCMUSIC_RPS_TICKET_EXPIRED",
            Category = "MusicNet",
            Meaning = "returned from AuthenticateAccount when the Passport RPS ticket has expired",
            CommonCause = "returned from AuthenticateAccount when the Passport RPS ticket has expired",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x8017200D",
            Name = "XONLINE_E_WCMUSIC_INVALID_RETAILER_ID",
            Category = "MusicNet",
            Meaning = "returned by any API that takes a retailerID, if the value specified was not expected",
            CommonCause = "returned by any API that takes a retailerID, if the value specified was not expected",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x8017200E",
            Name = "XONLINE_E_WCMUSIC_MAX_CONSUMPTION_EXCEEDED",
            Category = "MusicNet",
            Meaning = "returned by orderItems if the maximum number of points (defined by DMP policy) has been reached for the user making the purchase",
            CommonCause = "returned by orderItems if the maximum number of points (defined by DMP policy) has been reached for the user making the purchase",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x8017200F",
            Name = "XONLINE_E_WCMUSIC_ACCOUNT_REQUIRES_MANAGEMENT",
            Category = "MusicNet",
            Meaning = "returned if the account is otherwise disabled, banned, suspended, etc. and requires management on Argo.com",
            CommonCause = "returned if the account is otherwise disabled, banned, suspended, etc. and requires management on Argo.com",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80172010",
            Name = "XONLINE_E_WCMUSIC_ACCOUNT_INVALID_USER",
            Category = "MusicNet",
            Meaning = "returned if the RPS ticket is valid but for a user without an account",
            CommonCause = "returned if the RPS ticket is valid but for a user without an account",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80172011",
            Name = "XONLINE_E_WCMUSIC_MUSICNET_ERROR",
            Category = "MusicNet",
            Meaning = "returned if there was some sort of error returned from MusicNet",
            CommonCause = "returned if there was some sort of error returned from MusicNet",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80172012",
            Name = "XONLINE_E_WCMUSIC_MUSICNET_COMMUNICATION_ERROR",
            Category = "MusicNet",
            Meaning = "returned if there was communication error talking to musicnet",
            CommonCause = "returned if there was communication error talking to musicnet",
            FixSuggestion = "Verify subscription state, account status, content rights, and retry when the music service is available."
        },

        new()
        {
            Code = "0x80173000",
            Name = "XONLINE_E_STS_ERROR",
            Category = "Xbox Live",
            Meaning = "Unsepecified STS error",
            CommonCause = "Unsepecified STS error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80173001",
            Name = "XONLINE_E_STS_CONFIGURATION_ERROR",
            Category = "Xbox Live",
            Meaning = "STS configuration error",
            CommonCause = "STS configuration error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80173002",
            Name = "XONLINE_E_STS_INVALID_ARGUMENT",
            Category = "Validation",
            Meaning = "STS invalid argument error",
            CommonCause = "STS invalid argument error",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80173003",
            Name = "XONLINE_E_STS_INVALID_PLATFORM_TYPE",
            Category = "Validation",
            Meaning = "STS token requested for unsupported platform type",
            CommonCause = "STS token requested for unsupported platform type",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80174000",
            Name = "XONLINE_E_CONFIGDB_INVALID_COMPONENT",
            Category = "Validation",
            Meaning = "Caller specified a component name that does not exist in t_components",
            CommonCause = "Caller specified a component name that does not exist in t_components",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80174001",
            Name = "XONLINE_E_CONFIGDB_INVALID_INSTANCE",
            Category = "Validation",
            Meaning = "Caller specified an instance name that does not exist in t_instances",
            CommonCause = "Caller specified an instance name that does not exist in t_instances",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80174002",
            Name = "XONLINE_E_CONFIGDB_NO_DEFAULT_INSTANCE",
            Category = "Xbox Live",
            Meaning = "This operation cannot be completed without a default instance specified in t_instances",
            CommonCause = "This operation cannot be completed without a default instance specified in t_instances",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80174003",
            Name = "XONLINE_E_CONFIGDB_INVALID_SERVER",
            Category = "Validation",
            Meaning = "Caller specified a server name that does not exist in t_servers",
            CommonCause = "Caller specified a server name that does not exist in t_servers",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80174004",
            Name = "XONLINE_E_CONFIGDB_INVALID_SETTING",
            Category = "Validation",
            Meaning = "Caller specified a setting name that does not exist in t_settings",
            CommonCause = "Caller specified a setting name that does not exist in t_settings",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80174005",
            Name = "XONLINE_E_CONFIGDB_INVALID_PARENT",
            Category = "Validation",
            Meaning = "Caller specified an instance that is already a child and cannot be used as a parent instance.",
            CommonCause = "Caller specified an instance that is already a child and cannot be used as a parent instance.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80174006",
            Name = "XONLINE_E_CONFIGDB_CANNOT_DEMOTE_PARENT",
            Category = "Xbox Live",
            Meaning = "Parent instances cannot be demoted back into children.",
            CommonCause = "Parent instances cannot be demoted back into children.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80174007",
            Name = "XONLINE_E_CONFIGDB_INVALID_INTERFACE",
            Category = "Validation",
            Meaning = "Caller specified an interface that does not exist in t_interfaces.",
            CommonCause = "Caller specified an interface that does not exist in t_interfaces.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80174008",
            Name = "XONLINE_E_CONFIGDB_NAME_TOO_LONG",
            Category = "Xbox Live",
            Meaning = "Caller specified a named item (component, server, setting, etc.) whose name exceeeded the allowable limit.",
            CommonCause = "Caller specified a named item (component, server, setting, etc.) whose name exceeeded the allowable limit.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80174009",
            Name = "XONLINE_E_CONFIGDB_NO_PARAMETERS_SPECIFIED",
            Category = "Xbox Live",
            Meaning = "Call specified no parameters where at least one is required.",
            CommonCause = "Call specified no parameters where at least one is required.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80175000",
            Name = "XONLINE_E_CATALOGWATCHER_INVALID_ID",
            Category = "Validation",
            Meaning = "The ID does not exist in the database",
            CommonCause = "The ID does not exist in the database",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80175001",
            Name = "XONLINE_E_CATALOGWATCHER_PRODUCER_ERROR",
            Category = "Xbox Live",
            Meaning = "Generic producer error",
            CommonCause = "Generic producer error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80175002",
            Name = "XONLINE_E_CATALOGWATCHER_PUBLISHER_ERROR",
            Category = "Xbox Live",
            Meaning = "Generic publisher error",
            CommonCause = "Generic publisher error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80175003",
            Name = "XONLINE_E_CATALOGWATCHER_WATCHER_ERROR",
            Category = "Xbox Live",
            Meaning = "Generic Watcher error",
            CommonCause = "Generic Watcher error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80175004",
            Name = "XONLINE_E_CATALOGWATCHER_ESPPUBLISHER_FAST_OPERATION_ERROR",
            Category = "Xbox Live",
            Meaning = "ESP operation error",
            CommonCause = "ESP operation error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80175005",
            Name = "XONLINE_E_CATALOGWATCHER_ESPPUBLISHER_FAST_OPERATION_WARNING",
            Category = "Xbox Live",
            Meaning = "ESP operation warning",
            CommonCause = "ESP operation warning",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80175006",
            Name = "XONLINE_E_CATALOGWATCHER_INVALID_DESTINATION_NAME",
            Category = "Validation",
            Meaning = "The destination name is invalid",
            CommonCause = "The destination name is invalid",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80175007",
            Name = "XONLINE_E_CATALOGWATCHER_INVALID_RESOURCE_NAME",
            Category = "Validation",
            Meaning = "The resource name is invalid",
            CommonCause = "The resource name is invalid",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80175008",
            Name = "XONLINE_E_CATALOGWATCHER_PRODUCER_INVALID_QUEUE_OPERATION",
            Category = "Validation",
            Meaning = "The producer did not add anything to the queue",
            CommonCause = "The producer did not add anything to the queue",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80175009",
            Name = "XONLINE_E_CATALOGWATCHER_PUBLISHER_UPDATE_LSN_FAILED",
            Category = "System Update",
            Meaning = "The publisher could not update the LSN",
            CommonCause = "The publisher could not update the LSN",
            FixSuggestion = "Update the dashboard/system files, verify title activation/update requirements, then retry."
        },

        new()
        {
            Code = "0x8017500A",
            Name = "XONLINE_E_CATALOGWATCHER_PUBLISHER_INVALID_QUEUE_OPERATION",
            Category = "Validation",
            Meaning = "The publisher did not dequeue",
            CommonCause = "The publisher did not dequeue",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8017500B",
            Name = "XONLINE_E_CATALOGWATCHER_ESPPUBLISHER_INCORRECT_LSN_ORDER",
            Category = "Xbox Live",
            Meaning = "LSN’s were not in monotonically increasing order",
            CommonCause = "LSN’s were not in monotonically increasing order",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8017500C",
            Name = "XONLINE_E_CATALOGWATCHER_MEDIA_EXTRACTOR_MISSING_DOCUMENT",
            Category = "Xbox Live",
            Meaning = "Missing Document",
            CommonCause = "Missing Document",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8017500D",
            Name = "XONLINE_E_CATALOGWATCHER_NULL_DOCUMENT_IN_QUEUE",
            Category = "Xbox Live",
            Meaning = "Null document in queue",
            CommonCause = "Null document in queue",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8017500E",
            Name = "XONLINE_E_CATALOGWATCHER_ZERO_QUEUE_SIZE",
            Category = "Xbox Live",
            Meaning = "Queue is being initialized with size 0",
            CommonCause = "Queue is being initialized with size 0",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8017500F",
            Name = "XONLINE_E_CATALOGWATCHER_GENERATE_DOCUMENTS_INVALID_COUNT",
            Category = "Validation",
            Meaning = "Count of <= 0 passed to GenerateDocuments",
            CommonCause = "Count of <= 0 passed to GenerateDocuments",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80175010",
            Name = "XONLINE_E_CATALOGWATCHER_PRODUCER_NOT_INITIALIZED_CORRECTLY",
            Category = "Xbox Live",
            Meaning = "Producer not initialized correctly",
            CommonCause = "Producer not initialized correctly",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80175011",
            Name = "XONLINE_E_CATALOGWATCHER_PUBLISHER_NEGATIVE_BUCKET",
            Category = "Xbox Live",
            Meaning = "Bucket identifier is negative",
            CommonCause = "Bucket identifier is negative",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80175012",
            Name = "XONLINE_E_CATALOGWATCHER_PUBLISHER_NULL_DOCUMENT",
            Category = "Xbox Live",
            Meaning = "Document is null",
            CommonCause = "Document is null",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80175013",
            Name = "XONLINE_E_CATALOGWATCHER_NULL_LSN",
            Category = "Xbox Live",
            Meaning = "LSN is null",
            CommonCause = "LSN is null",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80175014",
            Name = "XONLINE_E_CATALOGWATCHER_INVALID_INPUT_PARAMETER",
            Category = "Validation",
            Meaning = "Invalid input parameter",
            CommonCause = "Invalid input parameter",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80175015",
            Name = "XONLINE_E_CATALOGWATCHER_MISSING_PARAMETER",
            Category = "Xbox Live",
            Meaning = "Missing input parameter",
            CommonCause = "Missing input parameter",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80175016",
            Name = "XONLINE_E_CATALOGWATCHER_COMMAND_FAILURE",
            Category = "Xbox Live",
            Meaning = "Generic Command Failure",
            CommonCause = "Generic Command Failure",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80175017",
            Name = "XONLINE_E_CATALOGWATCHER_PRODUCER_INVALID_STATE",
            Category = "Validation",
            Meaning = "Producer method has been invoked either without initializing or after an exception was thrown.",
            CommonCause = "Producer method has been invoked either without initializing or after an exception was thrown.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80175018",
            Name = "XONLINE_E_CATALOGWATCHER_PRODUCER_NO_PDLC_CONFIG",
            Category = "Xbox Live",
            Meaning = "pdlc_mediatypes setting is absent in t_multisettings in npdb",
            CommonCause = "pdlc_mediatypes setting is absent in t_multisettings in npdb",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80175019",
            Name = "XONLINE_E_CATALOGWATCHER_PRODUCER_NO_RATEABLE_CONFIG",
            Category = "Xbox Live",
            Meaning = "ratings_mediatypes setting is absent in t_multisettings in npdb",
            CommonCause = "ratings_mediatypes setting is absent in t_multisettings in npdb",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8017501A",
            Name = "XONLINE_E_CATALOGWATCHER_PRODUCER_NOT_AN_INTEGER_SETTING",
            Category = "Xbox Live",
            Meaning = "The setting is not an integer",
            CommonCause = "The setting is not an integer",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8017501B",
            Name = "XONLINE_E_CATALOGWATCHER_PRODUCER_NO_EXEMPTGCFORTITLES",
            Category = "Xbox Live",
            Meaning = "Missing exempt game content for titles multisetting",
            CommonCause = "Missing exempt game content for titles multisetting",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8017501C",
            Name = "XONLINE_E_CATALOGWATCHER_PRODUCER_NO_EXEMPTMEDIATYPES",
            Category = "Xbox Live",
            Meaning = "Missing exempt media types multisetting",
            CommonCause = "Missing exempt media types multisetting",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8017501D",
            Name = "XONLINE_E_CATALOGWATCHER_DOCUMENT_TOO_LARGE",
            Category = "Xbox Live",
            Meaning = "Document has been generated that is larger then FAST ESP can store",
            CommonCause = "Document has been generated that is larger then FAST ESP can store",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176000",
            Name = "XONLINE_E_MIX_UNKNOWNERROR",
            Category = "Xbox Live",
            Meaning = "Unknown Error",
            CommonCause = "Unknown Error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176001",
            Name = "XONLINE_E_MIX_ENTITYFRAMEWORK_CONNECTION_ERROR",
            Category = "Network",
            Meaning = "SQL & Entity Framework Connection Error",
            CommonCause = "SQL & Entity Framework Connection Error",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80176002",
            Name = "XONLINE_E_MIX_CONFIGUREPRODUCT",
            Category = "Xbox Live",
            Meaning = "Configure Product Error",
            CommonCause = "Configure Product Error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176003",
            Name = "XONLINE_E_MIX_GETPRODUCT",
            Category = "Xbox Live",
            Meaning = "Get Product Error",
            CommonCause = "Get Product Error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176004",
            Name = "XONLINE_E_MIX_CONFIGUREOFFER",
            Category = "Marketplace",
            Meaning = "Configure Offer Error",
            CommonCause = "Configure Offer Error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80176005",
            Name = "XONLINE_E_MIX_GETOFFER",
            Category = "Marketplace",
            Meaning = "Get Offer Error",
            CommonCause = "Get Offer Error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80176006",
            Name = "XONLINE_E_MIX_WEBSGVALIDATIONERROR",
            Category = "Xbox Live",
            Meaning = "WebSG Validation Error",
            CommonCause = "WebSG Validation Error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176007",
            Name = "XONLINE_E_MIX_ARGUMENT_NULL",
            Category = "Xbox Live",
            Meaning = "Null input to mix",
            CommonCause = "Null input to mix",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176008",
            Name = "XONLINE_E_MIX_ARGUMENT_NULL_INTERNAL",
            Category = "Xbox Live",
            Meaning = "Null input within mix",
            CommonCause = "Null input within mix",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176009",
            Name = "XONLINE_E_MIX_NOT_FOUND_IN_CATALOG",
            Category = "Xbox Live",
            Meaning = "Item not found in catalog",
            CommonCause = "Item not found in catalog",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8017600A",
            Name = "XONLINE_E_MIX_INVALID_MEDIA_RELATIONSHIP_TYPE",
            Category = "Validation",
            Meaning = "MediaRelationShipType is invalid",
            CommonCause = "MediaRelationShipType is invalid",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8017600B",
            Name = "XONLINE_E_MIX_INVALID_OFFER_RELATIONSHIP_TYPE",
            Category = "Marketplace",
            Meaning = "Offer relationship type is invalid",
            CommonCause = "Offer relationship type is invalid",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8017600C",
            Name = "XONLINE_E_MIX_NEW_MEDIA_FOR_EXISTING_OFFER",
            Category = "Marketplace",
            Meaning = "New media id given for existing offer",
            CommonCause = "New media id given for existing offer",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8017600D",
            Name = "XONLINE_E_MIX_NEW_OFFER_FOR_EXISTING_OFFERINSTANCE",
            Category = "Marketplace",
            Meaning = "New offer id given for existing offer instance",
            CommonCause = "New offer id given for existing offer instance",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x8017600E",
            Name = "XONLINE_E_MIX_INVALID_DURATION_TYPE",
            Category = "Validation",
            Meaning = "DurationType is Invalid",
            CommonCause = "DurationType is Invalid",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8017600F",
            Name = "XONLINE_E_MIX_INVALID_OFFER_RELATIONSHIP_MEDIA",
            Category = "Marketplace",
            Meaning = "related Medias are Invalid",
            CommonCause = "related Medias are Invalid",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80176010",
            Name = "XONLINE_E_MIX_INVALID_OFFERTYPE_MEDIA",
            Category = "Marketplace",
            Meaning = "Offertype and MediaIds mismatch",
            CommonCause = "Offertype and MediaIds mismatch",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80176011",
            Name = "XONLINE_E_MIX_INVALID_TITLE_ID",
            Category = "Validation",
            Meaning = "Invalid Title ID Used",
            CommonCause = "Invalid Title ID Used",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80176012",
            Name = "XONLINE_E_MIX_INVALID_LEADERBOARD_ID",
            Category = "Validation",
            Meaning = "Invalid Leaderboard ID Used",
            CommonCause = "Invalid Leaderboard ID Used",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80176013",
            Name = "XONLINE_E_MIX_INVALID_XLAST",
            Category = "Validation",
            Meaning = "Invalid Xlast Used",
            CommonCause = "Invalid Xlast Used",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80176014",
            Name = "XONLINE_E_MIX_DECOMPRESSION_ERROR",
            Category = "Xbox Live",
            Meaning = "Decompression error when decompressing xlast",
            CommonCause = "Decompression error when decompressing xlast",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176015",
            Name = "XONLINE_E_MIX_INVALID_VERSION",
            Category = "Validation",
            Meaning = "Invalid Version Used",
            CommonCause = "Invalid Version Used",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80176016",
            Name = "XONLINE_E_MIX_INVALID_PLATFORM",
            Category = "Validation",
            Meaning = "Invalid Platform Used",
            CommonCause = "Invalid Platform Used",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80176017",
            Name = "XONLINE_E_MIX_INVALID_FILE_TYPE",
            Category = "Validation",
            Meaning = "Invalid File Type Used",
            CommonCause = "Invalid File Type Used",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80176018",
            Name = "XONLINE_E_MIX_INVALID_GUID",
            Category = "Validation",
            Meaning = "Empty GUID Used",
            CommonCause = "Empty GUID Used",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80176019",
            Name = "XONLINE_E_MIX_GAMEATTRIBUTE_INVALID_PROPERTY",
            Category = "Validation",
            Meaning = "Unknown game attribute property encountered",
            CommonCause = "Unknown game attribute property encountered",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8017601A",
            Name = "XONLINE_E_MIX_GAMEATTRIBUTE_INVALID_PROPERTY_TYPE",
            Category = "Validation",
            Meaning = "Unknown game attribute property type encountered",
            CommonCause = "Unknown game attribute property type encountered",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8017601B",
            Name = "XONLINE_E_MIX_GAMEATTRIBUTE_INVALID_PROPERTY_VALUE",
            Category = "Validation",
            Meaning = "Unknown game attribute property value encountered",
            CommonCause = "Unknown game attribute property value encountered",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8017601C",
            Name = "XONLINE_E_MIX_MEDIALOCALEMAPS_INVALID_MAPPING",
            Category = "Validation",
            Meaning = "Invalid mapping encountered processing MediaLocaleMaps",
            CommonCause = "Invalid mapping encountered processing MediaLocaleMaps",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8017601D",
            Name = "XONLINE_E_MIX_INVALID_CONSUMABLE_QUANTITY",
            Category = "Validation",
            Meaning = "Invalid quantity for game consumable offer",
            CommonCause = "Invalid quantity for game consumable offer",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8017601E",
            Name = "XONLINE_E_MIX_FORBIDDEN_ENVIRONMENT",
            Category = "Xbox Live",
            Meaning = "Environment not allowed",
            CommonCause = "Environment not allowed",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8017601F",
            Name = "XONLINE_E_MIX_INVALID_MEDIATYPE",
            Category = "Validation",
            Meaning = "Invalid MediaType used for a Product",
            CommonCause = "Invalid MediaType used for a Product",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80176020",
            Name = "XONLINE_E_MIX_NEW_LIVEOFFERID_FOR_EXISTING_OFFER",
            Category = "Marketplace",
            Meaning = "Attempt to change a live offer id mapping",
            CommonCause = "Attempt to change a live offer id mapping",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80176021",
            Name = "XONLINE_E_MIX_UNIQUE_RATINGSYSTEMS_ERROR",
            Category = "Xbox Live",
            Meaning = "The rating list contains at least more than one instance from the same rating system",
            CommonCause = "The rating list contains at least more than one instance from the same rating system",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176022",
            Name = "XONLINE_E_MIX_PRODUCT_LOCALIZATION_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "No localization text for this product is found the particular country for the offer instance",
            CommonCause = "No localization text for this product is found the particular country for the offer instance",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176023",
            Name = "XONLINE_E_MIX_CONCURRENCY_ERROR",
            Category = "Xbox Live",
            Meaning = "Concurrency issue occured in Entity Framework",
            CommonCause = "Concurrency issue occured in Entity Framework",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176024",
            Name = "XONLINE_E_MIX_INVALID_SERVICE_TYPE",
            Category = "Validation",
            Meaning = "invalid service type - xbox or zune",
            CommonCause = "invalid service type - xbox or zune",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80176025",
            Name = "XONLINE_E_MIX_INVALID_SUBSCRIPTION_TYPE",
            Category = "Validation",
            Meaning = "invalid subscription type - base, game, content",
            CommonCause = "invalid subscription type - base, game, content",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80176026",
            Name = "XONLINE_E_MIX_INVALID_TIER",
            Category = "Validation",
            Meaning = "invalid tier (0, 3, 6 …)",
            CommonCause = "invalid tier (0, 3, 6 …)",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80176027",
            Name = "XONLINE_E_MIX_INVALID_SERVICE_PRIVILEGE_SET",
            Category = "Validation",
            Meaning = "invalid service privilege set (e.g. gold, silver, family, zune pass, phantasy star …)",
            CommonCause = "invalid service privilege set (e.g. gold, silver, family, zune pass, phantasy star …)",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80176028",
            Name = "XONLINE_E_MIX_CATALOG_CONSTRAINT_ERROR",
            Category = "Xbox Live",
            Meaning = "Catalog SQL Constraint error occured",
            CommonCause = "Catalog SQL Constraint error occured",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176029",
            Name = "XONLINE_E_MIX_ZERO_GUID_NOT_ALLOWED",
            Category = "Xbox Live",
            Meaning = "The API does not support empty guids.",
            CommonCause = "The API does not support empty guids.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8017602A",
            Name = "XONLINE_E_MIX_GROUP_DOES_NOT_EXIST",
            Category = "Xbox Live",
            Meaning = "The group requested does not exist.",
            CommonCause = "The group requested does not exist.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8017602B",
            Name = "XONLINE_E_MIX_INVALID_PRODUCTFAMILY",
            Category = "Validation",
            Meaning = "product ->offerInstance mapping is happening even before service sets are defined",
            CommonCause = "product ->offerInstance mapping is happening even before service sets are defined",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8017602C",
            Name = "XONLINE_E_MIX_INVALID_SKU_DELETE",
            Category = "Validation",
            Meaning = "Invalid sku delete operation",
            CommonCause = "Invalid sku delete operation",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8017602D",
            Name = "XONLINE_E_MIX_DUPLICATE_GROUP_NAME_NOT_ALLOWED",
            Category = "Xbox Live",
            Meaning = "Group names must be unique",
            CommonCause = "Group names must be unique",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8017602E",
            Name = "XONLINE_E_MIX_INVALID_GROUP_NAME",
            Category = "Validation",
            Meaning = "The group name is invalid.",
            CommonCause = "The group name is invalid.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x8017602F",
            Name = "XONLINE_E_MIX_INVALID_GROUP_CREATOR",
            Category = "Validation",
            Meaning = "The group creator is invalid.",
            CommonCause = "The group creator is invalid.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80176030",
            Name = "XONLINE_E_MIX_ARGUMENT_EMPTY",
            Category = "Xbox Live",
            Meaning = "Empty string passed into mix that needs to be filled in",
            CommonCause = "Empty string passed into mix that needs to be filled in",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176031",
            Name = "XONLINE_E_MIX_INVALID_CONFIG",
            Category = "Validation",
            Meaning = "Invalid combination of configuration parameters.",
            CommonCause = "Invalid combination of configuration parameters.",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80176032",
            Name = "XONLINE_E_MIX_STATS_FORCE_REQUIRED",
            Category = "Xbox Live",
            Meaning = "Leaderboard update needs force parameter to be successful",
            CommonCause = "Leaderboard update needs force parameter to be successful",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176033",
            Name = "XONLINE_E_MIX_MATCH_MISSING_REMOVE_MODE",
            Category = "Xbox Live",
            Meaning = "MixMatch attemped to remove a mode with removeMode set to false",
            CommonCause = "MixMatch attemped to remove a mode with removeMode set to false",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176034",
            Name = "XONLINE_E_MIX_MATCH_NEW_MODE_FOUND",
            Category = "Xbox Live",
            Meaning = "MixMatch found a new mode added to the XLAST. This must be configured manually.",
            CommonCause = "MixMatch found a new mode added to the XLAST. This must be configured manually.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176035",
            Name = "XONLINE_E_MIX_MATCH_SQL_EXCEPTION",
            Category = "Database",
            Meaning = "MixMatch encountered a SQL exception. This may be due to dropTable being set to false.",
            CommonCause = "MixMatch encountered a SQL exception. This may be due to dropTable being set to false.",
            FixSuggestion = "Retry later, check backend/service status, and verify the request is not creating duplicate or invalid records."
        },

        new()
        {
            Code = "0x80176036",
            Name = "XONLINE_E_MIX_GROUP_MUST_BE_EMPTY",
            Category = "Xbox Live",
            Meaning = "The operation is not valid unless the group is empty.",
            CommonCause = "The operation is not valid unless the group is empty.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176037",
            Name = "XONLINE_E_MIX_STRING_TOO_LONG",
            Category = "Xbox Live",
            Meaning = "A string parameter is too long",
            CommonCause = "A string parameter is too long",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176038",
            Name = "XONLINE_E_MIX_MACHINE_NOT_IN_GROUP",
            Category = "Xbox Live",
            Meaning = "The machine was not in the given group.",
            CommonCause = "The machine was not in the given group.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176039",
            Name = "XONLINE_E_MIX_CANNOT_MIGRATE_TO_SELF",
            Category = "Xbox Live",
            Meaning = "Cannot migrate a group to itself.",
            CommonCause = "Cannot migrate a group to itself.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8017603A",
            Name = "XONLINE_E_MIX_DUPLICATE_XRL_NOT_ALLOWED",
            Category = "Xbox Live",
            Meaning = "Duplicate title update location XRLs aren’t allowed.",
            CommonCause = "Duplicate title update location XRLs aren’t allowed.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8017603B",
            Name = "XONLINE_E_MIX_FRONT_DOOR_RESET_ERROR",
            Category = "Xbox Live",
            Meaning = "Front Door Reset Error",
            CommonCause = "Front Door Reset Error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x8017603C",
            Name = "XONLINE_E_MIX_DUPLICATE_APP_NOT_ALLOWED",
            Category = "Xbox Live",
            Meaning = "Duplicate App Not Allowed",
            CommonCause = "Duplicate App Not Allowed",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176040",
            Name = "XONLINE_E_MIX_ARRAY_TOO_LONG",
            Category = "Xbox Live",
            Meaning = "An array parameter is too long",
            CommonCause = "An array parameter is too long",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176041",
            Name = "XONLINE_E_MIX_DUPLICATE_RANK_NOT_ALLOWED",
            Category = "Xbox Live",
            Meaning = "Duplicate title update location ranks aren’t allowed.",
            CommonCause = "Duplicate title update location ranks aren’t allowed.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176042",
            Name = "XONLINE_E_MIX_OFFER_INVALID_SUBSCRIPTIONFAMILY_CONFIG",
            Category = "Marketplace",
            Meaning = "Wrong LiveSubscriptionFamily Configuration",
            CommonCause = "Wrong LiveSubscriptionFamily Configuration",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80176043",
            Name = "XONLINE_E_MIX_OFFER_INVALID_SUBSCRIPTION_CONFIG",
            Category = "Marketplace",
            Meaning = "Wrong LiveSubscription Configuration",
            CommonCause = "Wrong LiveSubscription Configuration",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80176044",
            Name = "XONLINE_E_MIX_OFFER_NO_TIER_FOUND",
            Category = "Marketplace",
            Meaning = "No tier found",
            CommonCause = "No tier found",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80176045",
            Name = "XONLINE_E_MIX_OFFER_NO_FREQUENCY_FOUND",
            Category = "Marketplace",
            Meaning = "No Offer frequency found",
            CommonCause = "No Offer frequency found",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80176046",
            Name = "XONLINE_E_MIX_OFFER_ERROR_FAMILY_INGESTION",
            Category = "Marketplace",
            Meaning = "No family content ingestion supported",
            CommonCause = "No family content ingestion supported",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80176047",
            Name = "XONLINE_E_MIX_OFFER_ERROR_LEGACY_OFFER",
            Category = "Marketplace",
            Meaning = "No Legacy offer mapping",
            CommonCause = "No Legacy offer mapping",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80176048",
            Name = "XONLINE_E_MIX_DUPLICATE_CONTENTID",
            Category = "Marketplace",
            Meaning = "A ContentId can only be associated with one MediaInstance/ProductPackage",
            CommonCause = "A ContentId can only be associated with one MediaInstance/ProductPackage",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80176049",
            Name = "XONLINE_E_MIX_ERROR_INGESTING_MANIFEST",
            Category = "System Update",
            Meaning = "There was an error ingesting a new Etx manifest",
            CommonCause = "There was an error ingesting a new Etx manifest",
            FixSuggestion = "Update the dashboard/system files, verify title activation/update requirements, then retry."
        },

        new()
        {
            Code = "0x80176050",
            Name = "XONLINE_E_MIX_INVALID_VISIBILITY_STATUS",
            Category = "Validation",
            Meaning = "Invalid visibilitystatus (only allows 3,5 through mix)",
            CommonCause = "Invalid visibilitystatus (only allows 3,5 through mix)",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80176051",
            Name = "XONLINE_E_MIX_PRODUCT_DUPLICATE_IMAGE_INSTANCE_ID",
            Category = "Xbox Live",
            Meaning = "Duplicate Image Instance Id (ImageId, Lcid, SizeId, FormatId) not allowed through MIX",
            CommonCause = "Duplicate Image Instance Id (ImageId, Lcid, SizeId, FormatId) not allowed through MIX",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80176052",
            Name = "XONLINE_E_MIX_OPERATION_NOT_SUPPORTED",
            Category = "Network",
            Meaning = "Operation not supported or not yet implemented.",
            CommonCause = "Operation not supported or not yet implemented.",
            FixSuggestion = "Test Xbox network connection, check DNS/NAT/router settings, disable blocking rules, and retry."
        },

        new()
        {
            Code = "0x80177000",
            Name = "XONLINE_E_FML_UNKNOWN_ERROR",
            Category = "Xbox Live",
            Meaning = "Unknown Error",
            CommonCause = "Unknown Error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80177001",
            Name = "XONLINE_E_FML_ALREADY_EXECUTING",
            Category = "Xbox Live",
            Meaning = "Trying to start a new job while a job is currently executing.",
            CommonCause = "Trying to start a new job while a job is currently executing.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80177002",
            Name = "XONLINE_E_FML_ARGUMENT_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "An argument was not found during the running of a job.",
            CommonCause = "An argument was not found during the running of a job.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80177003",
            Name = "XONLINE_E_FML_JOB_FAILED",
            Category = "Xbox Live",
            Meaning = "A job encountered an exception and was stopped.",
            CommonCause = "A job encountered an exception and was stopped.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80177004",
            Name = "XONLINE_E_FML_OPERATION_FAILED",
            Category = "Xbox Live",
            Meaning = "A job encountered an exception and was stopped.",
            CommonCause = "A job encountered an exception and was stopped.",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80178000",
            Name = "XONLINE_E_ESP_ENGINE_UNKNOWN_ERROR",
            Category = "Xbox Live",
            Meaning = "Unknown error",
            CommonCause = "Unknown error",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80178001",
            Name = "XONLINE_E_ESP_ENGINE_INVALID_PARAMETER",
            Category = "Validation",
            Meaning = "An invalid datatype was used",
            CommonCause = "An invalid datatype was used",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80178002",
            Name = "XONLINE_E_ESP_ENGINE_INVALID_SYNTAX",
            Category = "Validation",
            Meaning = "Invalid syntax used for value type",
            CommonCause = "Invalid syntax used for value type",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80178003",
            Name = "XONLINE_E_ESP_ENGINE_INVALID_DATA_TYPE",
            Category = "Validation",
            Meaning = "Invalid data type was used",
            CommonCause = "Invalid data type was used",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80178004",
            Name = "XONLINE_E_ESP_ENGINE_PARAMETER_NOT_FOUND",
            Category = "Xbox Live",
            Meaning = "Could not find the parameter in the filter criteria",
            CommonCause = "Could not find the parameter in the filter criteria",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80178005",
            Name = "XONLINE_E_ESP_ENGINE_INVALID_RULE",
            Category = "Validation",
            Meaning = "Rule could not be processed and had invalid data",
            CommonCause = "Rule could not be processed and had invalid data",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80178006",
            Name = "XONLINE_E_ESP_ENGINE_SEARCH_TERM_TO_LONG",
            Category = "Xbox Live",
            Meaning = "Search term used is beyond the proper length allowed",
            CommonCause = "Search term used is beyond the proper length allowed",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        },

        new()
        {
            Code = "0x80178007",
            Name = "XONLINE_E_ESP_ENGINE_INVALID_PARAMETER_GROUPING",
            Category = "Validation",
            Meaning = "An invalid grouping of parameters was used",
            CommonCause = "An invalid grouping of parameters was used",
            FixSuggestion = "Verify request parameters, IDs, account, offer, product, and service identifiers."
        },

        new()
        {
            Code = "0x80179000",
            Name = "XONLINE_E_MARKETPLACECATALOG_UNKNOWN_ERROR",
            Category = "Marketplace",
            Meaning = "Unknown error",
            CommonCause = "Unknown error",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80179001",
            Name = "XONLINE_E_MARKETPLACECATALOG_PARAMETER_MISSING",
            Category = "Marketplace",
            Meaning = "A parameter is missing",
            CommonCause = "A parameter is missing",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80179002",
            Name = "XONLINE_E_MARKETPLACECATALOG_BAD_FORMAT",
            Category = "Marketplace",
            Meaning = "A parameter is not formatted properly",
            CommonCause = "A parameter is not formatted properly",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80179003",
            Name = "XONLINE_E_MARKETPLACECATALOG_DETAIL_LEVEL_MISSING",
            Category = "Marketplace",
            Meaning = "A detail level is missing for a search hit",
            CommonCause = "A detail level is missing for a search hit",
            FixSuggestion = "Check content ownership, license availability, region, marketplace availability, and retry the download/purchase."
        },

        new()
        {
            Code = "0x80180000",
            Name = "XONLINE_E_XBOXLIVETOKEN_INVALID_VERSION",
            Category = "Authentication",
            Meaning = "Token Major or Minor version number invalid",
            CommonCause = "Token Major or Minor version number invalid",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80180001",
            Name = "XONLINE_E_XBOXLIVETOKEN_INVALID_ISSUER",
            Category = "Authentication",
            Meaning = "Token Issuer invalid",
            CommonCause = "Token Issuer invalid",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80180002",
            Name = "XONLINE_E_XBOXLIVETOKEN_INVALID_STATEMENT_COUNT",
            Category = "Authentication",
            Meaning = "Token contains an unexpected number of statements",
            CommonCause = "Token contains an unexpected number of statements",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80180003",
            Name = "XONLINE_E_XBOXLIVETOKEN_INVALID_STATEMENT_TYPE",
            Category = "Authentication",
            Meaning = "Token statement is not of the expected type",
            CommonCause = "Token statement is not of the expected type",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80180004",
            Name = "XONLINE_E_XBOXLIVETOKEN_INVALID_THUMBPRINT",
            Category = "Authentication",
            Meaning = "Token not signed by expected certificate",
            CommonCause = "Token not signed by expected certificate",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80180005",
            Name = "XONLINE_E_XBOXLIVETOKEN_INVALID_CLIENTTHUMBPRINT",
            Category = "Authentication",
            Meaning = "Token not being used with the certificate for which it was issued",
            CommonCause = "Token not being used with the certificate for which it was issued",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80180006",
            Name = "XONLINE_E_XBOXLIVETOKEN_INVALID_PLATFORMTYPE",
            Category = "Authentication",
            Meaning = "Platform type not valid or inconsistent with token claims",
            CommonCause = "Platform type not valid or inconsistent with token claims",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80180020",
            Name = "XONLINE_E_XBOXLIVETOKEN_CERT_CONFIG",
            Category = "Authentication",
            Meaning = "Configuration problem caused GetCertificate failure",
            CommonCause = "Configuration problem caused GetCertificate failure",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80180101",
            Name = "XONLINE_E_AAINFO_TOKEN_DATA_NOTFOUND",
            Category = "Authentication",
            Meaning = "AA Token Data not found",
            CommonCause = "AA Token Data not found",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80180102",
            Name = "XONLINE_E_AAINFO_TOKEN_FIELD_NOTVALID",
            Category = "Authentication",
            Meaning = "AA Token field not valid with Token",
            CommonCause = "AA Token field not valid with Token",
            FixSuggestion = "Sign out and back in, verify account/profile state, clear cache, check token/session validity, and retry."
        },

        new()
        {
            Code = "0x80190218",
            Name = "XONLINE_E_CTP_BDK_E_EXCEEDS_MAXIMUM_DURATION",
            Category = "Xbox Live",
            Meaning = "“Error subscription duration exceeds max duration.”",
            CommonCause = "“Error subscription duration exceeds max duration.”",
            FixSuggestion = "Check Xbox Live service status, retry later, and verify profile/network/title state."
        }

        };

    }

}