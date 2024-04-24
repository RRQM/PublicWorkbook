using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;

namespace Demo
{
    public class ProcessStruct
    {
        public Bitmap Icon { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public class BitmapFastBinaryConverter : FastBinaryConverter<Bitmap>
    {
        protected override Bitmap Read(byte[] buffer, int offset, int len)
        {
            var byteBlock = new ValueByteBlock(buffer);
            byteBlock.Pos = offset;
            byte[] byteArray = byteBlock.ReadBytesPackage();
            using (var stream = new MemoryStream(byteArray))
            {
                return new Bitmap(stream);
            }
        }

        protected override int Write(ByteBlock byteBlock, Bitmap obj)
        {
            var pos = byteBlock.Pos;
            using (var stream = new MemoryStream())
            {
                obj.Save(stream, ImageFormat.Png);
                byte[] byteArray = stream.ToArray();

                byteBlock.WriteBytesPackage(byteArray);
            }
            return byteBlock.Pos - pos;
        }
    }
}
