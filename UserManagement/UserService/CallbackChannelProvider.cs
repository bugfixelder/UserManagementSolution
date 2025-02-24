using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace UserService
{
    public interface ICallbackChannelProvider
    {
        IUserCallback GetCallbackChannel();
    }
    public class CallbackChannelProvider : ICallbackChannelProvider
    {
        public IUserCallback GetCallbackChannel()
        {
            return OperationContext.Current.GetCallbackChannel<IUserCallback>();
        }
    }
}
