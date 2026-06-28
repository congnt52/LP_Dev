using Renci.SshNet;
namespace _5GAutoTool
{
    class UploadFIle
    {
        private static string host = "127.0.0.10";
        // sftp username
        private static string username = "";

        private static string password = "";

        public static int Send(string fileName)
        {
            var connectionInfo = new ConnectionInfo( host, "sftp", new PasswordAuthenticationMethod(username, password));
            // Upload File
            using (var sftp = new SftpClient(connectionInfo))
            {
                sftp.Connect();
                // sftp.ChangeDrirectory
                //using (var upfileStream = System.IO.File.OpenRead(fileName))
                //{
                //    sftp.UploadFile(upfileStream, fileName, true);
                //}
                //sftp.Disconnect();
            }
            return 0;
        }
    }
}
