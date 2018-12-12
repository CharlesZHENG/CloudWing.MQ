﻿using System;
using System.Threading.Tasks;
using System.IO;
using CloudWing.MQ.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CloudWing.MQ.Core
{
    public abstract class SubHandler<TMessageWrapper> : ISubHandler
        where TMessageWrapper : IMessageWrapper
    {
        readonly IServiceProvider serviceProvider;
        public SubHandler(IServiceProvider svProvider)
        {
            serviceProvider = svProvider;
        }
        public virtual Task Notice(byte[] wrapBytes, byte[] dataBytes, TMessageWrapper message, object data)
        {
            return data is IMessage msgData ? Tell(wrapBytes, dataBytes, msgData, message) : Task.CompletedTask;
        }
        public Task Notice(byte[] bytes)
        {
            var serializer = serviceProvider.GetService<ISerializer>();
            using (var ms = new MemoryStream(bytes))
            {
                var msg = serializer.Deserialize<TMessageWrapper>(ms);
                if (!MessageTypeMapper.TryGetValue(msg.TypeCode, out var type))
                {
                    throw new Exception($"{ msg.TypeCode } does not exist");
                }
                using (var ems = new MemoryStream(msg.BinaryBytes))
                {
                    return Notice(bytes, msg.BinaryBytes, msg, serializer.Deserialize(type, ems));
                }
            }
        }

        public abstract Task Tell(byte[] wrapBytes, byte[] dataBytes, IMessage data, TMessageWrapper msg);

    }
}
