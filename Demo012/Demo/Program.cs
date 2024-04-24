using Demo.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;

namespace Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //添加转换器
            FastBinaryFormatter.AddFastBinaryConverter(typeof(Bitmap),new BitmapFastBinaryConverter());

            var process = new ProcessStruct
            {
                Icon = Resources.program,
                Name = "Program",
                Path = "/program"
            };
            byte[] data = SerializeConvert.FastBinarySerialize(process);
            var de_data = SerializeConvert.FastBinaryDeserialize<ProcessStruct>(data);
            Console.WriteLine(de_data.Name);
        }
    }
}
