using EL.Async;
using Protos;
using Utils;

namespace EL.Robot.Core.Handler
{
    [MessageHandler]
    public class G2C_OffLineClientHandler : W_AMHandler<G2C_OffLine>
    {
        protected override async ELTask Run(WChannel channel, G2C_OffLine message)
        {
            await ELTask.CompletedTask;
            RoboatServerComponent.Remove(message);
        }
    }
}
