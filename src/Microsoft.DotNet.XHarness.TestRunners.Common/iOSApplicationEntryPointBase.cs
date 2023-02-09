// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Threading.Tasks;

#nullable enable
namespace Microsoft.DotNet.XHarness.TestRunners.Common;

public abstract class iOSApplicationEntryPointBase : ApplicationEntryPoint
{
    public override async Task RunAsync()
    {
        var options = ApplicationOptions.Current;
        TcpTextWriter? writer;
        TcpTextWriter? streamer;

        try
        {
            writer = options.UseTunnel
                ? TcpTextWriter.InitializeWithTunnelConnection(options.HostPort)
                : TcpTextWriter.InitializeWithDirectConnection(options.HostName, options.HostPort);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to initialize TCP writer. Continuing on console." + Environment.NewLine + ex);
            writer = null; // null means we will fall back to console output
        }

        try
        {
            streamer = TcpTextWriter.InitializeWithDirectConnection(options.HostName, options.HostStreamingPort);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to initialize TCP writer for results. Results will not be streamed." + Environment.NewLine + ex);
            streamer = null;
        }

        using (writer)
        using (streamer)
        {
            var logger = (writer == null || options.EnableXml) ? new LogWriter(Device) : new LogWriter(Device, writer);
            logger.MinimumLogLevel = MinimumLogLevel.Info;

            await InternalRunAsync(options, writer, writer, streamer);
        }
    }
}
