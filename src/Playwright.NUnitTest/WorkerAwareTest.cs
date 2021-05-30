/*
 * MIT License
 *
 * Copyright (c) Microsoft Corporation.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Microsoft.Playwright.NUnitTest
{
    public class WorkerAwareTest
    {
        private static ConcurrentStack<WorkerServices> workerServicesPool_ = new ConcurrentStack<WorkerServices>();

        public WorkerServices Services { get; private set; }

        [SetUp]
        public void WorkerSetup()
        {
            WorkerServices services;
            if (workerServicesPool_.TryPop(out services))
            {
                Services = services;
                return;
            }

            Services = new WorkerServices();
        }

        [TearDown]
        public async Task WorkerTeardown()
        {
            if (TestOk())
            {
                workerServicesPool_.Push(Services);
                await Services.ResetAsync();
            }
            else
            {
                await Services.DisposeAsync();
            }
        }

        public bool TestOk()
        {
            return
                TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed ||
                TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Skipped;
        }
    }
}