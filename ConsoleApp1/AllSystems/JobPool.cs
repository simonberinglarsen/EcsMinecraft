namespace ConsoleApp1.AllSystems
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ConsoleApp1.AllSystems.Services;
    using ConsoleApp1.Components;

    public class JobPool
    {
        private readonly BlockingCollection<InternalJob> renderJobQueue = new BlockingCollection<InternalJob>();
        private readonly BlockingCollection<InternalJob> updateJobQueue = new BlockingCollection<InternalJob>();
        private ConcurrentDictionary<Guid, object> tokenToResultMap = new ConcurrentDictionary<Guid, object>();
        private PerformanceService performance;

        public JobPool(PerformanceComponent performance)
        {
            CreateJobPump(updateJobQueue);
            CreateJobPump(renderJobQueue);
            CreateJobPump(renderJobQueue);
            this.performance = new PerformanceService(performance);
        }

        public int RenderJobCount => renderJobQueue.Count;

        public int UpdateJobCount => updateJobQueue.Count;

        public int PendingResults => tokenToResultMap.Count;

        public Guid AddRenderJob(Func<object, object> jobFunction, object jobArguments = null)
        {
            performance.Increase(PerformanceService.Section.JOB_ADDED_RENDER);
            return AddJob(renderJobQueue, jobFunction, jobArguments);
        }

        public Guid AddUpdateJob(Func<object, object> jobFunction, object jobArguments = null)
        {
            performance.Increase(PerformanceService.Section.JOB_ADDED_UPDATE);
            return AddJob(updateJobQueue, jobFunction, jobArguments);
        }

        public bool IsProcessing(Guid? taskToken)
        {
            return taskToken.HasValue && !IsDone(taskToken.Value);
        }

        public T GetResult<T>(Guid jobToken)
        {
            object result;
            tokenToResultMap.Remove(jobToken, out result);
            return (T)result;
        }

        public bool IsDone(Guid jobToken)
        {
            return tokenToResultMap.ContainsKey(jobToken);
        }

        public bool PendingResult(Guid? taskToken)
        {
            return taskToken.HasValue && IsDone(taskToken.Value);
        }

        private void CreateJobPump(BlockingCollection<InternalJob> q)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var job = q.Take();
                    var result = job.JobFunction(job.JobArguments);
                    tokenToResultMap[job.JobToken] = result;
                }
            });
        }

        private Guid AddJob(BlockingCollection<InternalJob> q, Func<object, object> jobFunction, object jobArguments = null)
        {
            Guid jobToken = Guid.NewGuid();
            var t = new InternalJob
            {
                JobFunction = jobFunction,
                JobArguments = jobArguments,
                JobToken = jobToken,
            };
            q.Add(t);
            return jobToken;
        }

        private class InternalJob
        {
            public Func<object, object> JobFunction { get; set; }

            public Guid JobToken { get; set; }

            public object JobArguments { get; internal set; }
        }
    }
}