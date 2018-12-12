﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CloudWing.MQ.Core
{
    public interface ISerializer
    {
        object Deserialize(Type type, Stream source);
        T Deserialize<T>(Stream source);
        void Serialize<T>(Stream destination, T instance);
    }
}
