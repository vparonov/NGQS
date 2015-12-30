using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace RabbitMQPluginLibrary
{
    public class WorkerContainer<T, U>
    {
        private BlockingCollection<IWorker<T, U>> workers;
        private CancellationTokenSource cancelationTokenSource;
        public WorkerContainer(CancellationTokenSource cts)
        {
            cancelationTokenSource = cts;
            workers = new BlockingCollection<IWorker<T, U>>();
        }

        public void AddWorker(IWorker<T, U> wrkr)
        {
            workers.Add(wrkr);
        }

        public IWorker<T, U> RemoveWorker()
        {
            return workers.Take();
        }

        public Task<U> Execute(T param)
        {
            return Task.Factory.StartNew
            (
                () =>
                {
                    U rtn = default(U) ;
                    var worker = workers.Take();
                    try
                    {
                        rtn = worker.Execute(param);
                    }
                    catch (Exception ex)
                    {
                        if (!cancelationTokenSource.IsCancellationRequested)
                        {
                            throw ex;
                        }
                    }
                    finally
                    {
                        workers.Add(worker);
                    }

                    return rtn;
                }
            );
        }

        public Task ExecuteVoid(T param)
        {
            return Task.Factory.StartNew
            (
                () =>
                {
                    var worker = workers.Take();
                    try
                    {
                        worker.ExecuteVoid(param);
                    }
                    finally
                    {
                        workers.Add(worker);
                    }
                }
            );
        }

    }
}
