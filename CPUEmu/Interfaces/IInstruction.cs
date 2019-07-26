namespace CPUEmu.Interfaces
{
    public interface IInstruction
    {
        int Position { get; }

        void Execute(IEnvironment env);
    }
}
