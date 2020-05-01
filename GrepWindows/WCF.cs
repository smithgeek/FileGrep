using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace GrepWindows
{
    [ServiceContract]
    public interface IGrepApp
    {
        [OperationContract]
        void setSearchParams(String pattern, String directory);

        [OperationContract]
        void connect(long uniqueId);
    }

    [ServiceContract]
    public interface IExternalApp
    {
        [OperationContract]
        void openFile(String filePath, int lineNumber);
    }

    public class GrepApp : IGrepApp
    {
        public void setSearchParams(String pattern, String directory)
        {
        }

        public void connect(long uniqueId)
        {
        }
    }

    public class ExternalApp : IExternalApp
    {
        public void openFile(String filePath, int lineNumber)
        {
        }
    }


    class GrepServer
    {
        private ServiceHost mHost;

        public void start()
        {
            mHost = new ServiceHost(typeof(GrepApp), new Uri[] { new Uri("BSGrep://localhost") });
            mHost.AddServiceEndpoint(typeof(IGrepApp), new NetNamedPipeBinding(), "setSearchParams");
            mHost.Open();
        }

        public void stop()
        {
            mHost.Close();
        }
    }

    class GrepClient
    {
        IGrepApp mApp;

        public void start()
        {
            ChannelFactory<IGrepApp> pipeFactory = new ChannelFactory<IGrepApp>(new NetNamedPipeBinding(), new EndpointAddress("BSGrep://localhost/setSearchParams"));
            mApp = pipeFactory.CreateChannel();
        }

        public void sendSearchParams(String pattern, String directory)
        {
            mApp.setSearchParams(pattern, directory);
        }
    }

    class ExternalServer
    {
        private ServiceHost mHost;
        private long mUniqueId;

        public void start()
        {
            mUniqueId = DateTime.Now.Ticks;
            mHost = new ServiceHost(typeof(GrepApp), new Uri[] { new Uri("BSGrepExternal" + mUniqueId + "://localhost") });
            mHost.AddServiceEndpoint(typeof(IGrepApp), new NetNamedPipeBinding(), "openFile");
            mHost.Open();
        }

        public void stop()
        {
            mHost.Close();
        }
    }

    class ExternalClient
    {
        IExternalApp mApp;

        public void start(long uniqueId)
        {
            ChannelFactory<IExternalApp> pipeFactory = new ChannelFactory<IExternalApp>(new NetNamedPipeBinding(), new EndpointAddress("BSGrepExternal" + uniqueId + "://localhost/openFile"));
            mApp = pipeFactory.CreateChannel();
        }

        public void sendFileToOpen(String filePath, int lineNumber)
        {
            mApp.openFile(filePath, lineNumber);
        }
    }

}
