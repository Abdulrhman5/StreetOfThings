using CommonLibrary;
using System.Threading.Tasks;

namespace Transaction.BusinessLogic.ReceivingCommands
{
    public interface IObjectReceivingAdder
    {
        Task<CommandResult> AddReceiving(AddReceivingDto addReceivingDto);
    }
}