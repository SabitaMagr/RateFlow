namespace Assignment.Data
{
    public struct ConCredential
    {
        public ConnectionType ConType;
        public string Server;
        public string UserName;
        public string Password;
        public string Database;
        public bool IsLocalDB;
        public string Datapath;
    }

    public enum ConnectionType
    {
        Access = 1,
        SqlServer,
        Oracle
    }
}
