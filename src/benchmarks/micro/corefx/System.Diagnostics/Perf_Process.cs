﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BenchmarkDotNet.Attributes;
using MicroBenchmarks;

namespace System.Diagnostics
{
    [BenchmarkCategory(Categories.CoreFX)]
    public class Perf_Process
    {
        private readonly string _nonExistingName = Guid.NewGuid().ToString();
        private int _currentProcessId;
        
        [Benchmark]
        public void GetCurrentProcess() => Process.GetCurrentProcess().Dispose();
        
        [GlobalSetup(Target = nameof(GetProcessById))]
        public void SetupGetProcessById() => _currentProcessId = Process.GetCurrentProcess().Id;
        
        [Benchmark]
        public void GetProcessById() => Process.GetProcessById(_currentProcessId).Dispose();

        [Benchmark]
        public void EnterLeaveDebugMode()
        {
            Process.EnterDebugMode();
            Process.LeaveDebugMode();
        }
        
        [Benchmark]
        public void GetProcesses()
        {
            foreach (var process in Process.GetProcesses())
            {
                process.Dispose();
            }
        }
        
        [Benchmark]
        public void GetProcessesByName()
        {
            foreach (var process in Process.GetProcessesByName(_nonExistingName))
            {
                process.Dispose();
            }
        }
        
        [Benchmark]
        public void StartAndWaitForExitDotNetVersion()
        {
            using (var dotnet = Process.Start(CreateStartInfo()))
            {
                dotnet.WaitForExit();
            }
        }
        
        [Benchmark]
        public void StartAndKillDotNetVersion()
        {
            using (var dotnet = Process.Start(CreateStartInfo()))
            {
                dotnet.Kill();
            }
        }

        private static ProcessStartInfo CreateStartInfo()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "--version",
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                RedirectStandardInput = false,
                CreateNoWindow = true
            };

            // this benchmark will run on CI machines where there is no dotnet in PATH
            processStartInfo.EnvironmentVariables["DOTNET_MULTILEVEL_LOOKUP"] = "0"; 

            return processStartInfo;
        }
    }
}