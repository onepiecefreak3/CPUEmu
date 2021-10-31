using System.IO;
using CpuContract.Executor;

namespace CpuContract
{
    public interface IAssembly
    {
        #region Properies

        /// <summary>
        /// The name of the architecture used in this adapter.
        /// This will decide the instruction set, cpu state settings.
        /// </summary>
        string Architecture { get; }

        /// <summary>
        /// Describes if this assembly format can be uniquely identified.
        /// </summary>
        bool CanIdentify { get; }

        #endregion

        #region Methods

        bool Identify(Stream assembly);

        void LoadPayload(Stream assembly, IInstructionParser instructionParser);

        #endregion

        // TODO: Remove when execution environment got set up in UI
        // TODO: Add possibility to choose or create interrupt broker from UI
        DeviceEnvironment CreateExecutionEnvironment(Stream assembly, IExecutor executor);
    }
}
