using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace ChickenFarmModel
{
    class Decoder
    {
        public static Order decode(string orderStr)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;

            Order order = new Order();
            //Console.Out.WriteLine(orderStr.Substring(0, (orderStr.IndexOf(';'))-1));
            //Console.Out.WriteLine(orderStr.Substring(0, 1));
            //Console.Out.WriteLine(orderStr.Substring(orderStr.IndexOf(';')+1, 4));
            //Console.Out.WriteLine(orderStr.Substring(orderStr.LastIndexOf(';') + 1));
            //Console.Out.WriteLine(orderStr.Substring(orderStr.IndexOf(';')+1, (orderStr.LastIndexOf(';'))-1));
            //Console.Out.WriteLine(orderStr.Substring((orderStr.LastIndexOf(';')) + 1));
            order.setThreadId(Int32.Parse(orderStr.Substring(0, (orderStr.IndexOf(';')))));
            //order.setThreadId(Int32.Parse(orderStr.Substring(0, 1)));
            //order.setCardNum(Int32.Parse(orderStr.Substring(orderStr.IndexOf(';')+1, (orderStr.LastIndexOf(';'))-1)));
            order.setCardNum(Int32.Parse(orderStr.Substring(orderStr.IndexOf(';') + 1, 4)));
            //Console.Out.WriteLine("REALLY? " + orderStr);
            //Console.Out.WriteLine("WERWER" + Int32.Parse(orderStr.Substring((orderStr.LastIndexOf(';')) + 1)));
            //Console.Out.WriteLine(orderStr.Substring((orderStr.LastIndexOf(';'))+1, orderStr.IndexOf('=') - orderStr.LastIndexOf(';')-1));
            order.setAmount(Int32.Parse(orderStr.Substring((orderStr.LastIndexOf(';'))+1, orderStr.IndexOf('=') - orderStr.LastIndexOf(';')-1)));

            //Console.Out.WriteLine("We're ALMOST there.");
            //Console.Out.WriteLine(orderStr);
            order.setStartTime(DateTime.ParseExact(orderStr.Substring(orderStr.IndexOf('=')+1), Order.myDateFormat, provider));
            //order.setStartTime(DateTime.Parse(orderStr.Substring(orderStr.IndexOf('=')+1), new DateTimeFormatInfo()));
            //Console.Out.WriteLine("Now we have to output the TIME!");
            //Console.Out.WriteLine(order.getStartTime());
            //Console.Out.WriteLine(orderStr);
            //Console.Out.WriteLine(order.ToString());
            //Console.Out.WriteLine(order.ToString());

            return order;
        }
    }
}
