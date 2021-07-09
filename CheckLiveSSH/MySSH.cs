using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.Net;
using System.IO;

namespace CheckLiveSSH
{
    class MySSH
    {
        SshClient client;
        ForwardedPortLocal fwPort;
        public async Task checkSSH(string ip, string user, string pwd, string host, string timeout)
        {
            return await Task.Run(() =>
            {
                try
                {
                    //uint port = (uint)new Random().Next(1080, 9999);
                    uint port = 1080;
                    ConnectionInfo ConnNfo = new ConnectionInfo(ip, 22, user, new AuthenticationMethod[]{
                               new PasswordAuthenticationMethod(user, pwd) });
                    fwPort = new ForwardedPortLocal("127.0.0.1", port, host, 80);
                    client = new SshClient(ConnNfo);
                    client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(int.Parse(timeout));

                    client.Connect();
                    if (client.IsConnected)
                    {
                        client.AddForwardedPort(fwPort);
                        fwPort.Start();
                        if (fwPort.IsStarted)
                        {

                            closeAll();
                            return "live";

                        }

                    }


                }
                catch (Exception)
                {

                }
                return "Die";
            });
        }

        private void closeAll()
        {
            client.RemoveForwardedPort(fwPort);
            fwPort.Dispose();
            client.Disconnect();
            client.Dispose();
        }
    }
}
