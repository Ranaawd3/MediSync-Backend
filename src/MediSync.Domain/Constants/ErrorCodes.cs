namespace MediSync.Domain.Constants;

public static class ErrorCodes
{
    // Auth
    public const string AuthInvalidCredentials = "AUTH_INVALID_CREDENTIALS";
    public const string AuthTokenExpired       = "AUTH_TOKEN_EXPIRED";
    public const string AuthRefreshExpired     = "AUTH_REFRESH_EXPIRED";
    public const string AuthForbidden          = "AUTH_FORBIDDEN";

    // Medications
    public const string MedicationNotFound     = "MEDICATION_NOT_FOUND";
    public const string AllergyDetected        = "ALLERGY_DETECTED";
    public const string InteractionDetected    = "INTERACTION_DETECTED";
    public const string InteractionCheckFailed = "INTERACTION_CHECK_FAILED";
    public const string StockRunningLow        = "STOCK_RUNNING_LOW";

    // OCR
    public const string OcrQualityLow          = "OCR_QUALITY_LOW";
    public const string OcrProcessingFailed    = "OCR_PROCESSING_FAILED";

    // Reminders
    public const string ReminderNotFound       = "REMINDER_NOT_FOUND";

    // Family
    public const string FamilyInviteExpired    = "FAMILY_INVITE_EXPIRED";
    public const string FamilyLinkNotFound     = "FAMILY_LINK_NOT_FOUND";

    // General
    public const string ValidationError        = "VALIDATION_ERROR";
    public const string RateLimitExceeded      = "RATE_LIMIT_EXCEEDED";
}