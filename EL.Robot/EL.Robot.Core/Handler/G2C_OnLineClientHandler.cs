using EL.Async;
using Protos;
using Utils;

namespace EL.Robot.Core.Handler
{
    [MessageHandler]
    public class G2C_OnLineClientHandler : W_AMHandler<G2C_OnLine>
    {
        protected override async ELTask Run(WChannel channel, G2C_OnLine message)
        {
            await ELTask.CompletedTask;
            RoboatServerComponent.Add(message);
        }
    }
}
