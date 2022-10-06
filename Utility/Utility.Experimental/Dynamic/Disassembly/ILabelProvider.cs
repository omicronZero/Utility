namespace Utility.Dynamic.Disassembly
{
    public interface ILabelProvider
    {
        int GetRelativeAddress(int index, int instructionIndex);
        int GetByteAddress(int index);
        int GetIndex(int instructionAddress);
        int GetIndex(int instructionAddress, int address);
        int InstructionCount { get; }
    }
}