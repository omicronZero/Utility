using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace Utility.Dynamic.Disassembly
{
    public unsafe sealed class ILDisassembly : IDisposable
    {
        private bool _disposed;

        private readonly byte[] _ilData;
        private int _index;
        private byte* _ilDataPointer;
        private GCHandle _gcHandle;
        private short _opCode;
        public Module Module { get; }

        public ILDisassembly(Module module, byte[] ilData)
            : this(module, ilData, 0, (ilData ?? throw new ArgumentNullException(nameof(ilData))).Length)
        { }

        public ILDisassembly(Module module, MethodBody method)
            : this(module, (method ?? throw new ArgumentNullException(nameof(method))).GetILAsByteArray())
        { }

        public ILDisassembly(MethodBase method)
            : this((method ?? throw new ArgumentNullException(nameof(method))).Module, method.GetMethodBody())
        { }

        public ILDisassembly(Module module, byte[] ilData, int index, int count)
        {
            if (ilData == null)
                throw new ArgumentNullException(nameof(ilData));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative value expected in parameter count.");
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative value expected in parameter index.");

            if (index + count > ilData.Length)
                throw new ArgumentException("Indicated range exceeds boundaries of the array stored in parameter ilData.");

            _ilData = new byte[count];
            Buffer.BlockCopy(ilData, index, _ilData, 0, count);
            ilData = _ilData;

            _index = -1;

            _gcHandle = GCHandle.Alloc(ilData, GCHandleType.Pinned);
            Module = module;

            try
            {
                _ilDataPointer = (byte*)_gcHandle.AddrOfPinnedObject();
            }
            catch
            {
                _gcHandle.Free();
                throw;
            }
        }

        public ILInstruction CurrentInstruction
        {
            get { return new ILInstruction(OpCode, (ILOperand)Operand); }
        }

        public bool Initialized
        {
            get { return _index >= 0; }
        }

        public OpCode OpCode
        {
            get
            {
                CheckValidPosition();

                return OpCodeHelper.GetOpCode(_opCode);
            }
        }

        public ILDisassemblyOperand Operand
        {
            get
            {
                CheckValidPosition();

                OpCode opc = OpCode;

                return new ILDisassemblyOperand(this, _ilDataPointer, opc.OperandType);
            }
        }

        private byte ReadByte()
        {
            if (_index == _ilData.Length)
                throw UnexpectedEndOfIL();
            _index++;
            return *_ilDataPointer++;
        }

        private void Advance(int amount)
        {
            if (_index + amount >= _ilData.Length)
                throw UnexpectedEndOfIL();
            _index += amount;
            _ilDataPointer += amount;
        }

        private Exception UnexpectedEndOfIL()
        {
            return new InvalidOperationException("Unexpected end of IL instruction.");
        }

        public bool MoveNext() => MoveNext(true);

        public bool MoveNext(bool throwOnError)
        {
            if (_index >= 0)  //skip operand size, if there was a previous instruction              
                Advance(OpCodeHelper.SizeOf(OpCode.OperandType));
            else
                _index = 0;

            if (_index == _ilData.Length)
                return false;

            short opCodeValue = ReadByte();

            if (OpCodeHelper.IsPrefix((byte)opCodeValue))
            {
                if (!throwOnError && _index == _ilData.Length)
                    return false;
                opCodeValue = (short)((opCodeValue << 8) | ReadByte());
            }

            _opCode = opCodeValue;

            OpCode opCode = OpCodeHelper.GetOpCode(opCodeValue);

            if (_index + OpCodeHelper.SizeOf(opCode.OperandType) >= _ilData.Length)
            {
                if (throwOnError)
                    throw UnexpectedEndOfIL();
                else
                {
                    _ilDataPointer += _ilData.Length - _index;
                    _index = _ilData.Length;
                    return false;
                }
            }

            return true;
        }

        private void CheckValidPosition()
        {
            if (_index < 0)
                throw new InvalidOperationException("Instance has not been initialized.");
            if (_index == _ilData.Length)
                throw new InvalidOperationException("Instance has already reached the end.");
        }

        public bool IsDisposed
        {
            get { return _disposed; }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                _gcHandle.Free();
            }
        }

        ~ILDisassembly()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
