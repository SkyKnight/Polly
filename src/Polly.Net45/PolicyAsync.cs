﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Polly.Extensions;

namespace Polly
{
    public partial class Policy
    {
        private readonly Func<Func<Task>, Task> _asyncExceptionPolicy;

        internal Policy(Func<Func<Task>, Task> asyncExceptionPolicy)
        {
            if (asyncExceptionPolicy == null) throw new ArgumentNullException("asyncExceptionPolicy");

            _asyncExceptionPolicy = asyncExceptionPolicy;
        }

        internal Policy(Func<Func<Task>, Task> asyncExceptionPolicy, Action<Action> exceptionPolicy)
        {
            if (exceptionPolicy == null) throw new ArgumentNullException("exceptionPolicy");
            if (asyncExceptionPolicy == null) throw new ArgumentNullException("asyncExceptionPolicy");

            _exceptionPolicy = exceptionPolicy;
            _asyncExceptionPolicy = asyncExceptionPolicy;
        }

        /// <summary>
        ///     Executes the specified asynchronous action within the policy.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        [DebuggerStepThrough]
        public Task ExecuteAsync(Func<Task> action)
        {
            if (_asyncExceptionPolicy == null) throw new InvalidOperationException
                ("Please use the asynchronous RetryAsync, RetryForeverAsync, WaitAndRetryAsync or CircuitBreakerAsync methods when calling the asynchronous Execute method.");

            return _asyncExceptionPolicy(action);
        }

        /// <summary>
        ///     Executes the specified asynchronous action within the policy and returns the result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="action">The action to perform.</param>
        /// <returns>The value returned by the action</returns>
        [DebuggerStepThrough]
        public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action)
        {
            if (_asyncExceptionPolicy == null) throw new InvalidOperationException(
                "Please use the asynchronous RetryAsync, RetryForeverAsync, WaitAndRetryAsync or CircuitBreakerAsync methods when calling the asynchronous Execute method.");

            TResult result = default(TResult);
            await _asyncExceptionPolicy(async () => { result = await action().NotOnCapturedContext(); }).NotOnCapturedContext();
            return result;
        }
    }
}