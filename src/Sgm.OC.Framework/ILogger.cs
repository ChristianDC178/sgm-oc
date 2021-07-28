﻿using System;

namespace Sgm.OC.Log
{
    public interface ILogger
    {
        void Debug(string message, Exception ex = null);
        void Info(string message, Exception ex = null);
        void Warn(string message, Exception ex = null);
        void Error(string message, Exception ex = null);
        void Fatal(string message, Exception ex = null);

        void Flush();
    }

}

