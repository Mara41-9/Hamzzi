public static class DBConfig
{
    public static string Server = "127.0.0.1";
    public static string Port = "3307";
    public static string Database = "Hamzzi";
    public static string User = "root";
    public static string Password = "asdf123,./";

    public static string GameUserTable = "gameuser";

    public static string ConnectionString
    {
        get
        {
            return $"Server={Server};Port={Port};Database={Database};Uid={User};Pwd={Password};SslMode=None;";
        }
    }
}