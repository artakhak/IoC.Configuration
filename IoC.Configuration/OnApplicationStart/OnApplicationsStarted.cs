// This software is part of the IoC.Configuration library
// Copyright © 2018 IoC.Configuration Contributors
// http://oroptimizer.com
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.OnApplicationStart
{
    public class OnApplicationsStarted : IOnApplicationsStarted, IDisposable
    {
        #region Member Variables

        private bool _applicationsStarted;
        private bool _applicationsStopped;

        [NotNull]
        private readonly object _lockObject = new object();

        [NotNull]
        private readonly IPluginDataRepository _pluginDataRepository;

        [NotNull]
        [ItemNotNull]
        private readonly IEnumerable<IStartupAction> _startupActions;

        private const int MinMillisecondsToWaitForStop = 2000;

        #endregion

        #region  Constructors

        public OnApplicationsStarted([NotNull] [ItemNotNull] IEnumerable<IStartupAction> startupActions, [NotNull] IPluginDataRepository pluginDataRepository)
        {
            _startupActions = startupActions;
            _pluginDataRepository = pluginDataRepository;
        }

        #endregion

        #region IDisposable Interface Implementation

        public void Dispose()
        {
            StopStartupActions(15000, null);

            foreach (var pluginData in _pluginDataRepository.Plugins)
                pluginData.Plugin.Dispose();
        }

        #endregion

        #region IOnApplicationsStarted Interface Implementation

        public void StartStartupActions()
        {
            lock (_lockObject)
            {
                if (_applicationsStopped)
                {
                    LogHelper.Context.Log.Error("Applications cannot be started after being stopped.");
                    return;
                }

                if (_applicationsStarted)
                {
                    LogHelper.Context.Log.ErrorFormat($"Multiple calls to '{GetType().FullName}.{nameof(StartStartupActions)}'.");
                    return;
                }

                foreach (var startupAction in _startupActions)
                    startupAction.Start();

                foreach (var pluginData in _pluginDataRepository.Plugins)
                {
                    LogHelper.Context.Log.InfoFormat("Starting plugin {0}.", pluginData.Plugin.GetType().FullName);
                    pluginData.Plugin.Initialize();
                    LogHelper.Context.Log.InfoFormat("Started plugin {0}.", pluginData.Plugin.GetType().FullName);
                }

                _applicationsStarted = true;
            }
        }

        public void StopStartupActions(int maxMillisecondsToWait, Action onAllStartupActionsStopped)
        {
            if (_applicationsStopped || !_applicationsStarted)
                return;

            lock (_lockObject)
            {
                if (_applicationsStopped || !_applicationsStarted)
                    return;

                _applicationsStopped = true;
                if (maxMillisecondsToWait < MinMillisecondsToWaitForStop)
                    maxMillisecondsToWait = MinMillisecondsToWaitForStop;

                var startTime = DateTime.Now;
                while (true)
                {
                    var nonStoppedActionsExist = false;
                    foreach (var startupAction in _startupActions)
                        if (!startupAction.ActionExecutionCompleted)
                        {
                            startupAction.Stop();

                            if (!startupAction.ActionExecutionCompleted)
                                nonStoppedActionsExist = true;
                        }

                    if (!nonStoppedActionsExist || (DateTime.Now - startTime).TotalMilliseconds > maxMillisecondsToWait)
                    {
                        onAllStartupActionsStopped?.Invoke();
                        return;
                    }

                    Thread.Sleep(300);
                }
            }
        }

        #endregion
    }
}