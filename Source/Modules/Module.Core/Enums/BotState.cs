namespace Module.Core.Enums
{
    public enum BotState
    {
        Unknown = 0,
        Created = 1,
        Logging = 2,
        LoggingError = -2,
        Logged = 3,
        Connecting = 4,
        ConnectingError = -4,
        Connected = 5,
        Disconnecting = 6,
        Disconnected = 7,
        LoggingOut = 8,
        LoggedOut = 9,
        Idle = 10,
        Ready = 11
    }
}