using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetGenQueueService;
using log4net;
using EasyNetQ;
using System.Configuration;
using System.Threading;
using System.Diagnostics;

namespace RabbitMQPluginLibrary
{
    [DebuggerDisplay("Instance ID = {_instanceID} InstanceAliasName = {_instanceAliasName}")]
    public class RabbitMQConsumerPluginBase<T> : IPlugin, IDisposable where T :class 
    {
        protected ILog log;

        protected WorkerContainer<T, Dummy> workers ;
        protected IBus bus;
        protected int _instanceID;
        protected string _instanceAliasName;
        protected bool isAsync;
        protected CancellationTokenSource cancelationTokenSource;
        public RabbitMQConsumerPluginBase() 
        {
            log = LogManager.GetLogger(GetType());
            isAsync = false;
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

            if (isAsync)
            {
                workers = new WorkerContainer<T, Dummy>(cancelationTokenSource);
                InitWorkers();
            }

            //NB - обяснение защо нямам error handling тук
            //извадка от документацията на easynetq
            //////EasyNetQ implements a lazy connection strategy. 
            //////It assumes that the broker will not always be available. 
            //////When you first connect to a broker using RabbitHutch.CreateBus, 
            //////EasyNetQ enters a connection try loop and if no broker is available at the address you specify in 
            //////the connection string, you will see information messages logged saying ‘Trying to Connect’. 
            //////Subscribers can subscribe using bus.Subscribe even when a broker is not available. 
            //////The subscription details are cached by EasyNetQ. When a broker becomes available the connection loop succeeds, 
            //////a connection with the broker is established, and all the cached subscriptions are created.
            //////Similarly when EasyNetQ loses a connection to the broker, it returns to the connection-loop and 
            //////you will see ‘Trying to Connect’ messages in the log. Once the connection is re-established 
            //////the cached subscribers are once again created. The upshot of this is that you can 
            //////leave your subscribers running in an environment where the network connection is unreliable or where you need to 
            //////bounce your RabbitMQ broker.

            if (!isAsync)
            {
                bus.Subscribe<T>(_instanceAliasName, (m) => _Consume(m));
            }
            else
            {
                bus.SubscribeAsync<T>(_instanceAliasName, (m) => workers.ExecuteVoid(m));
            }
        }


        private void _Consume(T m)
        {
            try
            {
                Consume(m);
            }
            catch (Exception ex)
            {
                if (!cancelationTokenSource.IsCancellationRequested)
                {
                    throw ex;
                }
             }          
        }

        protected virtual void Consume(T m)
        {
            throw new NotImplementedException();
        }


        protected virtual void InitWorkers()
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
