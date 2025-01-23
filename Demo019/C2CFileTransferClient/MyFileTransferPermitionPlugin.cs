using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp.FileTransfer;
using TouchSocket.Dmtp;

namespace C2CFileTransferClient
{
    internal class MyFileTransferPermitionPlugin : PluginBase, IDmtpFileTransferringPlugin, IDmtpFileTransferredPlugin
    {
        private readonly ILog m_logger;

        public MyFileTransferPermitionPlugin(ILog logger)
        {
            this.m_logger = logger;
        }

        /// <summary>
        /// 该方法，会在每个文件被请求（推送）结束时触发。传输不一定成功，具体信息需要从e.Result判断状态。
        /// 其次，该方法也不一定会被执行，例如：在传输过程中，直接断网，则该方法将不会执行。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task OnDmtpFileTransferred(IDmtpActorObject client, FileTransferredEventArgs e)
        {
            //传输结束，但是不一定成功，甚至该方法都不一定会被触发，具体信息需要从e.Result判断状态。
            this.m_logger.Info($"传输文件结束，请求类型={e.TransferType}，文件名={e.ResourcePath}，请求状态={e.Result}");
            await e.InvokeNext();
        }


        /// <summary>
        /// 该方法，会在每个文件被请求（推送）时第一时间触发。
        /// 当请求文件时，可以重新指定请求的文件路径，即对e.ResourcePath直接赋值。
        /// 当推送文件时，可以重新指定保存文件路径，即对e.SavePath直接赋值。
        /// 
        /// 注意：当文件夹不存在时，需要手动创建。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task OnDmtpFileTransferring(IDmtpActorObject client, FileTransferringEventArgs e)
        {
            foreach (var item in e.Metadata.Keys)
            {
                Console.WriteLine($"Key={item},Value={e.Metadata[item]}");
            }
            e.IsPermitOperation = true;//每次传输都需要设置true，表示允许传输
                                       //有可能是上传，也有可能是下载
            this.m_logger.Info($"请求传输文件，请求类型={e.TransferType}，请求文件名={e.ResourcePath}");
            await e.InvokeNext();
        }
    }

}
