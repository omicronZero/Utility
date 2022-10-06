using Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Dynamic.Disassembly
{
    public static class SignatureDecoder
    {
        //See ECMA-335, II.23.2 Blobs and signatures

        //TODO: remove unused parameters (e.g. Module from ReadType, etc.)

        public static unsafe Signature DecodeSignature(Module module, byte[] signatureBlob)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            if (signatureBlob == null)
                throw new ArgumentNullException(nameof(signatureBlob));

            var blob = new ByteArrayReader(signatureBlob, 0);

            byte header = blob.ReadByte();

            CallType callType = (CallType)(header & 0xf);

            if (callType == CallType.C ||
                callType == CallType.Default ||
                callType == CallType.FastCall ||
                callType == CallType.StdCall ||
                callType == CallType.ThisCall ||
                callType == CallType.VarArg)
            {
                return DecodeMethod(module, blob, header);
            }
            else if (callType == CallType.Field)
            {
                return DecodeField(module, blob, header);
            }
            else if (callType == CallType.Property)
            {
                return DecodeProperty(module, blob, header);
            }
            else
                throw new NotSupportedException("Unsupported signature type detected.");
        }

        private static MethodSignature DecodeMethod(Module module, ByteArrayReader blob, byte header)
        {
            CallType callType = (CallType)(header & 0xf);
            MethodFlags m = (MethodFlags)(header & 0xf0);

            CallingConvention conv = (CallingConvention)callType;


            if ((m & MethodFlags.HasThis) == MethodFlags.HasThis)
                conv |= CallingConvention.HasThis;
            if ((m & MethodFlags.ExplicitThis) == MethodFlags.ExplicitThis)
                conv |= CallingConvention.ExplicitThis;

            int genParamCount = 0;

            if ((m & MethodFlags.Generic) == MethodFlags.Generic)
                genParamCount = (int)DecompressUInt29(blob);

            int paramCount = (int)DecompressUInt29(blob);

            ParameterDescription returnDescription = ReadParameter(module, blob);


            int sentinelIndex;
            ParameterDescription[] parameterDescription = ReadParameters(module, blob, paramCount, (callType == CallType.VarArg || callType == CallType.C), out sentinelIndex);

            return new MethodSignature(module, conv, returnDescription, parameterDescription, sentinelIndex);
        }

        private static FieldSignature DecodeField(Module module, ByteArrayReader blob, byte header)
        {
            List<CustomModifier> mods;
            ReadCustomMods(module, blob, true, out mods);

            TypeDescription fieldType = ReadType(module, blob);

            return new FieldSignature(module, fieldType, mods);
        }

        private static PropertySignature DecodeProperty(Module module, ByteArrayReader blob, byte header)
        {
            var m = (MethodFlags)(header & 0xf0);
            int paramCount = unchecked((int)DecompressUInt29(blob));
            List<CustomModifier> mods;
            int temp;

            ReadCustomMods(module, blob, true, out mods);

            TypeDescription propertyType = ReadType(module, blob);
            ParameterDescription[] param = ReadParameters(module, blob, paramCount, false, out temp);

            return new PropertySignature(module, (m & MethodFlags.HasThis) == MethodFlags.HasThis, propertyType, mods, param);
        }

        private static TypeDescription ReadType(Module module, ByteArrayReader blob)
        {
            //TODO: make use of TypeDescription methods instead of manually instantiating
            var e = (ElementType)blob.ReadByte();

            if (e == ElementType.Array)
            {
                //ARRAY Type ArrayShape
                TypeDescription type = ReadType(module, blob);
                ArrayShape arrayShape = ReadArrayShape(blob);

                return new TypeDescription(module, e, 0, 0, new ArrayDescription(type, arrayShape));
            }
            else if (e == ElementType.Class || e == ElementType.ValueType)
            {
                //CLASS TypeDefOrRefOrSpecEncoded
                //VALUETYPE TypeDefOrRefOrSpecEncoded
                int metadataToken = unchecked((int)DecompressUInt29(blob));

                return new TypeDescription(module, e, metadataToken, 0, null);
            }
            else if (e == ElementType.GenericInst)
            {
                //GENERICINST (CLASS | VALUETYPE) TypeDefOrRefSpecEncoded GenArgCount Type*
                //GenArgCount : Encoded unsigned integer
                ElementType cv = (ElementType)blob.ReadByte();
                if (cv != ElementType.Class && cv != ElementType.ValueType)
                    throw new ArgumentException("Invalid signature detected: Class or ValueType expected in generic instance declaration in type.");

                int metadataToken = unchecked((int)DecompressUInt29(blob));
                uint genArgCount = DecompressUInt29(blob);
                var genParam = new TypeDescription[genArgCount];

                for (uint i = 0; i < genArgCount; i++)
                    genParam[i] = ReadType(module, blob);

                return new TypeDescription(module, e, (int)cv, metadataToken, genParam);
            }
            else if (e == ElementType.MVar || e == ElementType.Var)
            {
                //MVAR number
                //VAR number
                //number : encoded unsigned integer

                int number = unchecked((int)DecompressUInt29(blob));

                return new TypeDescription(module, e, number, 0, null);
            }
            else if (e == ElementType.Ptr)
            {
                //PTR CustomMod* Type
                //PTR CustomMod* VOID
                List<CustomModifier> mods;
                ReadCustomMods(module, blob, true, out mods);

                TypeDescription tp = ReadType(module, blob);

                return new TypeDescription(module, e, 0, 0, mods);
            }
            else if (e == ElementType.SzArray)
            {
                //SZARRAY CustomMod* Type
                List<CustomModifier> customMods;
                ReadCustomMods(module, blob, true, out customMods);

                TypeDescription type = ReadType(module, blob);

                return new TypeDescription(module, e, 0, 0, new VectorArrayDescription(type, customMods));
            }
            else if (e == ElementType.FnPtr)
            {
                //FNPTR MethodDefSig
                //FNPTR MethodRefSig

                MethodSignature signature = DecodeMethod(module, blob, blob.ReadByte());

                return new TypeDescription(module, e, 0, 0, signature);
            }
            else
                return new TypeDescription(module, e, 0, 0, null);
        }

        private static ArrayShape ReadArrayShape(ByteArrayReader blob)
        {
            uint rank = DecompressUInt29(blob);
            uint numSizes = DecompressUInt29(blob);
            var sizes = new uint[rank];

            for (uint i = 0; i < numSizes; i++)
                sizes[i] = DecompressUInt29(blob);

            uint numLoBounds = DecompressUInt29(blob);
            var loBounds = new int[numLoBounds];

            for (uint i = 0; i < numLoBounds; i++)
            {
                loBounds[i] = DecompressInt29(blob);
            }
            return new ArrayShape(rank, sizes, loBounds);
        }

        private static ParameterDescription ReadParameter(Module module, ByteArrayReader blob)
        {
            List<CustomModifier> mods;
            ReadCustomMods(module, blob, true, out mods);
            ElementType e = (ElementType)blob.ReadByte();
            TypeDescription type;

            if (e == ElementType.ByRef)
                type = ReadType(module, blob);
            else if (e == ElementType.TypedByRef)
                type = new TypeDescription(module, ElementType.TypedByRef, 0, 0, null);
            else
                throw new ArgumentException("Invalid signature detected: BYREF or TYPEDBYREF expected in param signature.");

            return new ParameterDescription(type, mods);
        }

        private static void ReadCustomMods(Module module, ByteArrayReader blob, bool createList, out List<CustomModifier> mods)
        {
            mods = null;
            while (true)
            {
                var p = (ElementType)blob.PeekByte();

                if (p != ElementType.CModOpt
                        && p != ElementType.CModReqd)
                    break;

                CustomModifier mod = ReadCustomMod(module, blob);

                if (createList)
                {
                    if (mods == null)
                        mods = new List<CustomModifier>();

                    mods.Add(mod);
                }
            }
        }

        private static ParameterDescription[] ReadParameters(Module module, ByteArrayReader blob, int parameterCount, bool allowSentinel, out int sentinelIndex)
        {
            sentinelIndex = -1;

            var param = new ParameterDescription[parameterCount];

            for (int i = 0; i < parameterCount; i++)
            {
                if (allowSentinel && (ElementType)blob.PeekByte() == ElementType.Sentinel) //Sentinel
                {
                    if (sentinelIndex != -1)
                        throw new ArgumentException("Invalid signature detected: Multiple sentinel elements within parameters detected.");
                    sentinelIndex = i;
                    blob.Advance();
                }
                param[i] = ReadParameter(module, blob);
            }
            return param;
        }

        private static CustomModifier ReadCustomMod(Module module, ByteArrayReader blob)
        {
            //returns TypeDefOrRefOrSpecEncoded
            ElementType e = (ElementType)blob.ReadByte();
            bool required;
            if (e == ElementType.CModOpt)
                required = false;
            else if (e == ElementType.CModReqd)
                required = true;
            else
                throw new ArgumentException("Signature is invalid: Custom modifier should be optional or recommended. Other values are not allowed.");

            return new CustomModifier(unchecked((int)DecompressUInt29(blob)), required);
        }

        private static uint DecompressUInt29(ByteArrayReader reader)
        {
            //WARNING: null string is 0xff?
            uint cb = reader.ReadByte();

            if ((cb & 0x80) == 0x80) //if highest bit is set, the field size is extended to at least 2 bytes
            {
                if ((cb & 0x40) == 0x40) //field consists of 4 bytes
                {
                    if ((cb & 0x20) == 0x20)
                        throw new ArgumentException("Expected clear bit in compressed unsigned int.", nameof(reader));

                    return (cb & 0x1fu << 24) | ((uint)reader.ReadUInt16() << 8) | reader.ReadByte();
                }
                else
                    return (cb & 0x3fu << 8) | reader.ReadByte();
            }
            else
                return cb;
        }
        //TODO: delete comment
        //https://referencesource.microsoft.com/#mscorlib/system/reflection/emit/signaturehelper.cs,187e82b3339749e7,references
        /*  - If the value lies between -2^6 and 2^6-1 inclusive:
                - Represent the value as a 7-bit 2’s complement number, giving 0x40 (-2^6) to 0x3F (2^6-1);
                - Rotate this value 1 bit left, giving 0x01 (-2^6) to 0x7E (2^6-1);
                - Encode as a one-byte integer, bit 7 clear, rotated value in bits 6 through 0, giving 0x01 (-26) to 0x7E (26-1).
            - If the value lies between -2^13 and 2^13-1 inclusive:
                - Represent the value as a 14-bit 2’s complement number, giving 0x2000 (-2^13) to 0x1FFF (2^13-1);
                - Rotate this value 1 bit left, giving 0x0001 (-2^13) to 0x3FFE (2^13-1);
                - Encode as a two-byte integer: bit 15 set, bit 14 clear, rotated value in bits 13 through 0, giving 0x8001 (-2^13) to 0xBFFE (2^13-1).
            - If the value lies between -2^28 and 2^28-1 inclusive:
                - Represent the value as a 29-bit 2’s complement representation, giving 0x10000000 (-2^28) to 0xFFFFFFF (2^28-1);
                - Rotate this value 1-bit left, giving 0x00000001 (-2^28) to 0x1FFFFFFE (2^28-1);
                - Encode as a four-byte integer: bit 31 set, 30 set, bit 29 clear, rotated value in bits 28 through*/

        private static int DecompressInt29(ByteArrayReader reader)
        {
            int cb = reader.ReadByte();
            int fill = ~0x7f;

            if ((cb & 0x80) == 0x80)
            {
                if ((cb & 0x40) == 0x40)
                {
                    if ((cb & 0x20) == 0x20)
                        throw new ArgumentException("Expected clear bit in compressed int.", nameof(reader));

                    cb = (cb & 0x1f << 24) | (reader.ReadInt16() << 8) | reader.ReadByte();
                    fill = ~0x1fffffff;
                }
                else
                {
                    cb = (cb & 0x3f << 8) | reader.ReadByte();
                    fill = ~0x3fff;
                }
            }
            return (cb >> 1) | ((cb & 1) * fill);
        }

        private enum CallType : byte
        {
            Default = 0,
            C = 1,
            FastCall = 2,
            StdCall = 3,
            ThisCall = 4,
            VarArg = 5,
            Field = 6,
            Property = 8
        }

        private enum MethodFlags : byte
        {
            Default = 0,
            Generic = 0x10,
            HasThis = 0x20,
            ExplicitThis = 0x40,
        }
    }
}
