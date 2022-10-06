using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Xunit;

namespace Utility.Tests
{
    public class SerializationInfoExtensionsTests
    {
        [Fact]
        public void TestGetValue()
        {
            SerializationInfo info = new SerializationInfo(typeof(Mock), new FormatterConverter());

            info.AddValue("Value", 10);

            Assert.Equal(10, info.GetValue<int>("Value"));
        }

        [Fact]
        public void TestTryGetValue()
        {
            SerializationInfo info = new SerializationInfo(typeof(Mock), new FormatterConverter());

            info.AddValue("Int32", 10);
            info.AddValue("Date", DateTime.Now);
        }

        [Fact]
        public void TestPrimitives()
        {
            SerializationInfo info = new SerializationInfo(typeof(Mock), new FormatterConverter());

            bool vBoolean;
            info.AddValue("Boolean", true);
            Assert.True(info.TryGetBoolean("Boolean", out vBoolean));
            Assert.False(info.TryGetBoolean("BooleanNonexistent", out _));
            byte vByte;
            info.AddValue("Byte", 1);
            Assert.True(info.TryGetByte("Byte", out vByte));
            Assert.False(info.TryGetByte("ByteNonexistent", out _));
            char vChar;
            info.AddValue("Char", '1');
            Assert.True(info.TryGetChar("Char", out vChar));
            Assert.False(info.TryGetChar("CharNonexistent", out _));
            DateTime vDateTime;
            info.AddValue("DateTime", DateTime.Now);
            Assert.True(info.TryGetDateTime("DateTime", out vDateTime));
            Assert.False(info.TryGetDateTime("DateTimeNonexistent", out _));
            decimal vDecimal;
            info.AddValue("Decimal", 1.0m);
            Assert.True(info.TryGetDecimal("Decimal", out vDecimal));
            Assert.False(info.TryGetDecimal("DecimalNonexistent", out _));
            double vDouble;
            info.AddValue("Double", 1.0);
            Assert.True(info.TryGetDouble("Double", out vDouble));
            Assert.False(info.TryGetDouble("DoubleNonexistent", out _));
            short vInt16;
            info.AddValue("Int16", (short)1);
            Assert.True(info.TryGetInt16("Int16", out vInt16));
            Assert.False(info.TryGetInt16("Int16Nonexistent", out _));
            int vInt32;
            info.AddValue("Int32", 1);
            Assert.True(info.TryGetInt32("Int32", out vInt32));
            Assert.False(info.TryGetInt32("Int32Nonexistent", out _));
            long vInt64;
            info.AddValue("Int64", 1L);
            Assert.True(info.TryGetInt64("Int64", out vInt64));
            Assert.False(info.TryGetInt64("Int64Nonexistent", out _));
            sbyte vSByte;
            info.AddValue("SByte", (sbyte)1);
            Assert.True(info.TryGetSByte("SByte", out vSByte));
            Assert.False(info.TryGetSByte("SByteNonexistent", out _));
            float vSingle;
            info.AddValue("Single", 1f);
            Assert.True(info.TryGetSingle("Single", out vSingle));
            Assert.False(info.TryGetSingle("SingleNonexistent", out _));
            string vString;
            info.AddValue("String", "test");
            Assert.True(info.TryGetString("String", out vString));
            Assert.False(info.TryGetString("StringNonexistent", out _));
            ushort vUInt16;
            info.AddValue("UInt16", (ushort)1);
            Assert.True(info.TryGetUInt16("UInt16", out vUInt16));
            Assert.False(info.TryGetUInt16("UInt16Nonexistent", out _));
            uint vUInt32;
            info.AddValue("UInt32", 1u);
            Assert.True(info.TryGetUInt32("UInt32", out vUInt32));
            Assert.False(info.TryGetUInt32("UInt32Nonexistent", out _));
            ulong vUInt64;
            info.AddValue("UInt64", 1UL);
            Assert.True(info.TryGetUInt64("UInt64", out vUInt64));
            Assert.False(info.TryGetUInt64("UInt64Nonexistent", out _));

            Assert.True(info.TryGetBoolean("Boolean") == vBoolean);
            Assert.True(info.TryGetBoolean("BooleanNonexistent") == null);
            Assert.True(info.TryGetByte("Byte") == vByte);
            Assert.True(info.TryGetByte("ByteNonexistent") == null);
            Assert.True(info.TryGetChar("Char") == vChar);
            Assert.True(info.TryGetChar("CharNonexistent") == null);
            Assert.True(info.TryGetDateTime("DateTime") == vDateTime);
            Assert.True(info.TryGetDateTime("DateTimeNonexistent") == null);
            Assert.True(info.TryGetDecimal("Decimal") == vDecimal);
            Assert.True(info.TryGetDecimal("DecimalNonexistent") == null);
            Assert.True(info.TryGetDouble("Double") == vDouble);
            Assert.True(info.TryGetDouble("DoubleNonexistent") == null);
            Assert.True(info.TryGetInt16("Int16") == vInt16);
            Assert.True(info.TryGetInt16("Int16Nonexistent") == null);
            Assert.True(info.TryGetInt32("Int32") == vInt32);
            Assert.True(info.TryGetInt32("Int32Nonexistent") == null);
            Assert.True(info.TryGetInt64("Int64") == vInt64);
            Assert.True(info.TryGetInt64("Int64Nonexistent") == null);
            Assert.True(info.TryGetSByte("SByte") == vSByte);
            Assert.True(info.TryGetSByte("SByteNonexistent") == null);
            Assert.True(info.TryGetSingle("Single") == vSingle);
            Assert.True(info.TryGetSingle("SingleNonexistent") == null);
            Assert.True(info.TryGetUInt16("UInt16") == vUInt16);
            Assert.True(info.TryGetUInt16("UInt16Nonexistent") == null);
            Assert.True(info.TryGetUInt32("UInt32") == vUInt32);
            Assert.True(info.TryGetUInt32("UInt32Nonexistent") == null);
            Assert.True(info.TryGetUInt64("UInt64") == vUInt64);
            Assert.True(info.TryGetUInt64("UInt64Nonexistent") == null);

            Assert.True(info.TryGetString("String") == vString);
            Assert.True(info.TryGetString("StringNonExistent") == null);
        }

        [Serializable]
        private sealed class Mock : ISerializable
        {
            public int Value { get; }

            public Mock(int value)
            {
                Value = value;
            }

            public Mock(SerializationInfo info, StreamingContext context)
            {
                Value = info.GetInt32(nameof(Value));
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(nameof(Value), Value);
            }
        }
    }
}
