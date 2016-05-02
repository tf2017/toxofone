namespace Toxofone.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using SharpTox.Av;
    using SharpTox.Core;
    using Toxofone.UI;

    public class ConnectionManager : IToxManager
    {
        private Tox tox;

        public ConnectionManager()
        {
        }

        public void InitManager(Tox tox, ToxAv toxAv)
        {
            if (tox == null)
            {
                throw new ArgumentNullException("tox");
            }

            this.tox = tox;
            this.tox.OnConnectionStatusChanged += this.OnToxConnectionStatusChanged;
        }

        public void DoBootstrap()
        {
            var nodes = Config.Instance.Nodes;

            if (nodes.Length >= 4)
            {
                var random = new Random();
                var indices = new List<int>();

                for (int i = 0; i < 4;)
                {
                    int index = random.Next(nodes.Length);
                    if (indices.Contains(index))
                    {
                        continue;
                    }

                    var node = nodes[index];
                    if (this.Bootstrap(nodes[index]))
                    {
                        indices.Add(index);
                        i++;
                    }
                }
            }
            else
            {
                foreach (var node in nodes)
                {
                    this.Bootstrap(node);
                }
            }

            this.WaitAndBootstrap(20000);
        }

        private void OnToxConnectionStatusChanged(object sender, ToxEventArgs.ConnectionStatusEventArgs e)
        {
            if (e.Status == ToxConnectionStatus.None)
            {
                this.WaitAndBootstrap(2000);
            }

            Logger.Log(LogLevel.Info, string.Format("Connection status changed to: {0}", e.Status));

            MainForm.Instance.NotifyToxConnectionStatusChanged(e);
        }

        private void WaitAndBootstrap(int delay)
        {
            new Thread(new ThreadStart(() =>
            {
                // wait 'delay' seconds, check if we're connected, if not, bootstrap again
                Thread.Sleep(delay);

                if (!this.tox.IsConnected)
                {
                    Logger.Log(LogLevel.Info, "We're still not connected, bootstrapping again");
                    this.DoBootstrap();
                }
            })) { IsBackground = true }.Start();
        }

        private bool Bootstrap(ToxConfigNode node)
        {
            var toxNode = new ToxNode(node.Address, node.Port, new ToxKey(ToxKeyType.Public, node.PublicKey));
            var error = ToxErrorBootstrap.Ok;
            bool success = this.tox.Bootstrap(toxNode, out error);

            if (success)
            {
                Logger.Log(LogLevel.Info, string.Format("Bootstrapped off of {0}:{1}", node.Address, node.Port));
            }
            else
            {
                Logger.Log(LogLevel.Error, string.Format("Could not bootstrap off of {0}:{1}, error: {2}", node.Address, node.Port, error));
            }

            // even if adding the tcp relay fails for some reason (while it shouldn't...), we'll consider this successful.
            if (this.tox.AddTcpRelay(toxNode, out error))
            {
                Logger.Log(LogLevel.Info, string.Format("Added TCP relay:    {0}:{1}", node.Address, node.Port));
            }
            else
            {
                Logger.Log(LogLevel.Error, string.Format("Could not add TCP relay {0}:{1}, error: {2}", node.Address, node.Port, error));
            }

            return success;
        }
    }
}
