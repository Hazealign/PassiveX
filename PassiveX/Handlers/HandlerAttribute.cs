﻿using System;

namespace PassiveX.Handlers
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    sealed class HandlerAttribute : Attribute
    {
        public string Hostname { get; private set; }
        public int Port { get; private set; }

        internal HandlerAttribute(string hostname, int port)
        {
            Hostname = hostname;
            Port = port;
        }
    }
}
