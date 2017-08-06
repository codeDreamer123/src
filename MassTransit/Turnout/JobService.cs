﻿// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Turnout
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Commands;
    using Contracts;
    using GreenPipes;
    using Logging;


    public class JobService :
        IJobService
    {
        static readonly ILog _log = Logger.Get<JobService>();

        readonly Uri _managementAddress;
        readonly IJobRegistry _registry;
        readonly TimeSpan _superviseInterval;
        bool _stopping;

        public JobService(IJobRegistry jobRegistry, Uri inputAddress, Uri managementAddress, TimeSpan superviseInterval)
        {
            _superviseInterval = superviseInterval;
            InputAddress = inputAddress;
            _registry = jobRegistry;
            _managementAddress = managementAddress;
        }

        public Uri InputAddress { get; }

        async Task<JobHandle<T>> IJobService.CreateJob<T>(ConsumeContext context, Guid jobId, T command, IJobFactory<T> jobFactory)
        {
            if (_stopping)
                throw new InvalidOperationException("The job service is stopping.");

            var jobContext = new ConsumerJobContext<T>(context, jobId, command);

            var babyTask = Run(jobContext, jobFactory);

            var jobHandle = new ConsumerJobHandle<T>(jobContext, babyTask);

            _registry.Add(jobHandle);

            await ScheduleSupervision(jobContext, jobContext.Command, jobHandle).ConfigureAwait(false);

            return jobHandle;
        }

        public Task ScheduleSupervision<T>(ConsumeContext context, T job, JobHandle jobHandle)
            where T : class
        {
            var utcNow = DateTime.UtcNow;
            var scheduledTime = utcNow + _superviseInterval;

            var check = new SuperviseJobCommand<T>(jobHandle.JobId, job, utcNow, jobHandle.Status);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Scheduled Job Supervision: {0}-{1}", jobHandle.JobId.ToString("N"), typeof(T).Name);

            return context.ScheduleSend(_managementAddress, scheduledTime, check);
        }

        public async Task Stop()
        {
            _stopping = true;

            ICollection<JobHandle> pendingJobs = _registry.GetAll();

            foreach (var jobHandle in pendingJobs)
            {
                if (jobHandle.Status == JobStatus.Created || jobHandle.Status == JobStatus.Running)
                {
                    try
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Cancelling job: {0}", jobHandle.JobId);

                        await jobHandle.Cancel().ConfigureAwait(false);

                        JobHandle removed;
                        _registry.TryRemoveJob(jobHandle.JobId, out removed);

                        await jobHandle.NotifyCanceled("Job Service Stopped").ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        if (_log.IsErrorEnabled)
                            _log.Error($"Failed to cancel job: {jobHandle.JobId:N}", ex);
                    }
                }
            }
        }

        async Task Run<T>(ConsumerJobContext<T> jobContext, IJobFactory<T> jobFactory)
            where T : class
        {
            try
            {
                IPipe<JobContext<T>> pipe = Pipe.New<JobContext<T>>(cfg =>
                {
                    cfg.UseRetry(r =>
                    {
                        r.Ignore<TaskCanceledException>();
                        r.Ignore<OperationCanceledException>();
                        r.Interval(1, 1000);
                    });

                    cfg.UseExecuteAsync(context => context.NotifyStarted(_managementAddress));

                    cfg.UseInlineFilter(jobFactory.Execute);

                    cfg.UseExecuteAsync(context => context.NotifyCompleted());
                });

                await pipe.Send(jobContext).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                await jobContext.NotifyCanceled("Task canceled").ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                await jobContext.NotifyCanceled("Operation canceled").ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await jobContext.NotifyFaulted(exception).ConfigureAwait(false);
            }
            finally
            {
                jobContext.Dispose();
            }
        }
    }
}