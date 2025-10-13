using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;

namespace RPCServer
{

    public class RPC : SingletonRpcServer
    {
        [DmtpRpc]
        public int Test(int a, int b)
        {
            return a + b;
        }
    }
}
