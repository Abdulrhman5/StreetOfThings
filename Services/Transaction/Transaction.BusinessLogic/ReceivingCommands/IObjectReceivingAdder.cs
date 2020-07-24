using CommonLibrary;
using System.Threading.Tasks;

namespace Transaction.BusinessLogic.ReceivingCommands
{
    public interface IObjectReceivingAdder
    {
        Task<CommandResult<ObjectReceivingResultDto>> AddReceiving(AddReceivingDto addReceivingDto);
    }
}