using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp.RouterPackage;
using TouchSocket.Rpc;

namespace Server
{
    public class MyRequestPackage : DmtpRouterPackage
    {
        /// <summary>
        /// 包尺寸大小。此值并非需要精准数值，只需要估计数据即可。其作用为申请内存池。所以数据应当大小合适。
        /// </summary>
        public override int PackageSize => 1024 * 1024;
        /// <inheritdoc/>
        protected bool IncludedRouter => true;

        /// <summary>
        /// 反馈类型
        /// </summary>
        public FeedbackType Feedback { get; set; }

        /// <summary>
        /// 参数是否包含引用类型
        /// </summary>
        public bool IsByRef { get; set; }

        /// <summary>
        /// 函数名
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 参数数据
        /// </summary>
        public List<byte[]> ParametersBytes { get; set; }

        /// <summary>
        /// 返回参数数据
        /// </summary>
        public byte[] ReturnParameterBytes { get; set; }

        /// <summary>
        /// 序列化类型
        /// </summary>
        public SerializationType SerializationType { get; set; }

        public override void PackageBody(in ByteBlock byteBlock)
        {
            base.PackageBody(byteBlock);
            if (!this.IncludedRouter)
            {
                byteBlock.Write(this.Sign);
                byteBlock.Write(this.Status);
            }
            byteBlock.Write((byte)this.SerializationType);
            byteBlock.Write((byte)this.Feedback);
            byteBlock.Write(this.IsByRef);
            byteBlock.Write(this.MethodName);
            byteBlock.WriteBytesPackage(this.ReturnParameterBytes);

            if (this.ParametersBytes != null && this.ParametersBytes.Count > 0)
            {
                byteBlock.Write((byte)this.ParametersBytes.Count);
                foreach (var item in this.ParametersBytes)
                {
                    byteBlock.WriteBytesPackage(item);
                }
            }
            else
            {
                byteBlock.Write((byte)0);
            }

            byteBlock.WritePackage(this.Metadata);
        }

        public override void UnpackageBody(in ByteBlock byteBlock)
        {
            base.UnpackageBody(byteBlock);
            if (!this.IncludedRouter)
            {
                this.Sign = byteBlock.ReadInt64();
                this.Status = (byte)byteBlock.ReadByte();
            }
            this.SerializationType = (SerializationType)byteBlock.ReadByte();
            this.Feedback = (FeedbackType)byteBlock.ReadByte();
            this.IsByRef = byteBlock.ReadBoolean();
            this.MethodName = byteBlock.ReadString();
            this.ReturnParameterBytes = byteBlock.ReadBytesPackage();

            var countPar = (byte)byteBlock.ReadByte();
            this.ParametersBytes = new List<byte[]>();
            for (var i = 0; i < countPar; i++)
            {
                this.ParametersBytes.Add(byteBlock.ReadBytesPackage());
            }

            if (!byteBlock.ReadIsNull())
            {
                var package = new Metadata();
                package.Unpackage(byteBlock);
                this.Metadata = package;
            }
        }
    }

}
