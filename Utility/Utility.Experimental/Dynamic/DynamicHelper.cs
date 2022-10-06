using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;

namespace Utility.Dynamic
{
    internal static class DynamicHelper
    {
        public static AssemblyBuilder Assembly { get; }

        static DynamicHelper()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(typeof(DynamicHelper).FullName);
            sb.Append('_');
            GetMd5Name(typeof(DynamicHelper).Assembly.FullName, sb);
            sb.Append('_').Append(Convert.ToString(AppDomain.CurrentDomain.Id, 16));

            Assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(sb.ToString()), AssemblyBuilderAccess.Run);
        }

        private static ModuleBuilder CreateModule(Type creatorType)
        {
            return Assembly.DefineDynamicModule(GetEncodedTypeName(creatorType) + "_DynamicModule");
        }

        public static string GetMd5Name(string name)
        {
            return GetMd5Name(name, new StringBuilder(32)).ToString();
        }

        public static string GetEncodedTypeName(Type type)
        {
            return GetEncodedTypeName(type, new StringBuilder()).ToString();
        }

        public static StringBuilder GetEncodedTypeName(Type type, StringBuilder builder)
        {
            builder.Append('_');

            builder.Append(Convert.ToString(AppDomain.CurrentDomain.Id, 16));
            builder.Append(Convert.ToString(type.Module.MetadataToken, 16));
            builder.Append(Convert.ToString(type.MetadataToken, 16));

            return builder;
        }

        public static StringBuilder GetMd5Name(string name, StringBuilder builder)
        {
            using (var md5 = MD5.Create())
            {
                byte[] namemd5 = md5.ComputeHash(Encoding.Unicode.GetBytes(name));

                foreach (byte b in namemd5)
                {
                    builder.Append((char)('a' + (b >> 4)));
                    builder.Append((char)('a' + (b & 0xf)));
                }
            }

            return builder;
        }

        public static ModuleBuilder GetAssociatedModuleBuilder<T>()
        {
            return Helper<T>.Module;
        }

        private sealed class Helper<T>
        {
            private static object _syncObject;

            private static ModuleBuilder _module;

            static Helper()
            {
                _syncObject = new object();
            }

            public static ModuleBuilder Module
            {
                get
                {
                    if (_module == null)
                    {
                        lock (_syncObject)
                        {
                            if (_module == null)
                            {
                                _module = DynamicHelper.CreateModule(typeof(DelegateHandler));
                            }
                        }
                    }

                    return _module;
                }
            }
        }
    }
}
