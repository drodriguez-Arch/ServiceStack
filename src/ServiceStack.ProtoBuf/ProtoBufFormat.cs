﻿using System;
using System.IO;
using ProtoBuf.Meta;
using ServiceStack.Text;
using ServiceStack.Web;

namespace ServiceStack.ProtoBuf
{
    public class ProtoBufFormat : IPlugin, IProtoBufPlugin, Model.IHasStringId
    {
        public string Id { get; set; } = Plugins.ProtoBuf;
        public void Register(IAppHost appHost)
        {
            appHost.ContentTypes.Register(MimeTypes.ProtoBuf, Serialize, Deserialize);
        }

        private static RuntimeTypeModel model;
        public static RuntimeTypeModel Model => model ??= RuntimeTypeModel.Create();

        public static void Serialize(IRequest requestContext, object dto, Stream outputStream)
        {
            Serialize(dto, outputStream);
        }

        public static void Serialize(object dto, Stream outputStream)
        {
            Model.Serialize(outputStream, dto);
        }

        public static T Deserialize<T>(Stream fromStream) => (T) Deserialize(typeof(T), fromStream);
        public static object Deserialize(Type type, Stream fromStream)
        {
            // Current 3.0.0-alpha.152 fails to deserialize if using RecyclableMemoryStream directly 
            if (fromStream is RecyclableMemoryStream rms)
            {
                using var ms = new MemoryStream(rms.GetBuffer(), 0, (int) rms.Length); 
                var obj = Model.Deserialize(ms, null, type);
                return obj;
            }
            else
            {
                var obj = Model.Deserialize(fromStream, null, type);
                return obj;
            }
        }

        public string GetProto(Type type)
        {
            return Model.GetSchema(type);
        }
    }
}
