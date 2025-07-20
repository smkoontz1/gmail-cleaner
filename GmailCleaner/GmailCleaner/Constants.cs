namespace GmailCleaner;

public static class Constants
{
    public static readonly string AppName = nameof(GmailCleaner);
    public static readonly string GmailUserId = "smkoontz1@gmail.com";
    
    public static class LocalStorageKeys
    {
        public static readonly string GoogleAccessToken = "googleAccessToken";
        public static readonly string GoogleAccessTokenExpiry = "googleAccessTokenExpiry";

        public static readonly string PostRefreshPath = "postRefreshPath";
    }
}