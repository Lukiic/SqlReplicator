﻿using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLReplicator.Services.LoggerServices
{
    public class LoggerService
    {
        public static void InitializeLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "[{Timestamp:dd.MM.yyyy] [HH:mm:ss}] [{Level:u3}] {Message}{NewLine}{Exception}")
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:dd.MM.yyyy] [HH:mm:ss}] [{Level:u3}] {Message}{NewLine}{Exception}")
                .CreateLogger();
        }

        public static void CloseLogger()
        {
            Log.CloseAndFlush();
        }
    }
}
