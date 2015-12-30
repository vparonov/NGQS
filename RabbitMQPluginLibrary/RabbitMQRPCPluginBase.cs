using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NetGenQueueService;
using System.Threading;
using EasyNetQ;
using log4net;

namespace RabbitMQPluginLibrary
{

    [DebuggerDisplay("Instance ID = {_instanceID} InstanceAliasName = {_instanceAliasName}")]
    public class RabbitMQRPCPluginBase<RequestType, ResponseType> : IPlugin, IDisposable where RequestType:class where ResponseType:class 
    {
        protected ILog log; 

        protected IBus bus;
        protected int _instanceID;
        protected string _instanceAliasName;
        protected CancellationTokenSource cancelationTokenSource;

        public RabbitMQRPCPluginBase() 
        {
            log = LogManager.GetLogger(GetType());
        }

        public void Init(string instanceAliasName, int instanceID, string[] args, Dictionary<string, string> settings)
        {
            _instanceID = instanceID;
            _instanceAliasName = instanceAliasName;

            string enableLoggingString ;
            settings.TryGetValue("enableLogging", out enableLoggingString) ;
            bool enableLogging = false ;
            if (enableLoggingString != null)
            {
                enableLogging = Boolean.Parse(enableLoggingString) ;
            }

            bus = RabbitHutch.CreateBus(settings["connectionString"],
                x => x.Register<IEasyNetQLogger>(_ => new PluginLogger(log, enableLogging)));

            OnInit(args, settings);
        }

        public void Start(CancellationTokenSource _cancelationTokenSource)
        {
            cancelationTokenSource = _cancelationTokenSource;
            bus.Respond<RequestType, ResponseType>(request => _Response(request)); 
            //bus.Respond<RequestType, ResponseType>// .Subscribe<T>(_instanceAliasName, (m) => _Consume(m));
        }


        private ResponseType _Response(RequestType request)
        {
            try
            {
                return Response(request);
            }
            catch (Exception ex)
            {
                if (!cancelationTokenSource.IsCancellationRequested)
                {
                    throw ex;
                }
            }
            return default(ResponseType) ;
        }

        protected virtual ResponseType Response(RequestType request)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnStop() 
        { 
        } 

        public void Stop()
        {
            OnStop();
            bus.Dispose();
            bus = null;
        }

        public void Dispose()
        {
            if (bus != null)
            {
                bus.Dispose();
            }
        }

        protected virtual void OnInit(string[] args, Dictionary<string, string> settings)
        {

        }
    }
}
