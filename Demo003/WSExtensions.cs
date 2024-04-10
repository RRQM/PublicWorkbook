using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Http;

namespace WSDemo
{
    internal static class WSExtensions
    {

        public static readonly DependencyProperty<string> FlagProperty =
             DependencyProperty<string>.Register("FlagProperty", default);

        public static void SetFlag(this IHttpClientBase socketClient, string value)
        {
            socketClient.SetValue(FlagProperty, value);
        }

        public static string GetFlag(this IHttpClientBase socketClient)
        {
            return socketClient.GetValue(FlagProperty);
        }
    }
}
