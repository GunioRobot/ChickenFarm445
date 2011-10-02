using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace ChickenFarmModel
{
    //The Decoder class takes an encoded order and turns it into an order object
    class Decoder
    {
        public static Order decode(string orderStr)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;

            Order order = new Order();
            /* begin parsing orderStr */
            order.setThreadId(Int32.Parse(orderStr.Substring(0, (orderStr.IndexOf(';')))));
            order.setCardNum(Int32.Parse(orderStr.Substring(orderStr.IndexOf(';') + 1, 4)));
            order.setAmount(Int32.Parse(orderStr.Substring((orderStr.LastIndexOf(';'))+1, orderStr.IndexOf(',') - orderStr.LastIndexOf(';')-1)));
            order.setUnitPrice(Int32.Parse(orderStr.Substring((orderStr.IndexOf(',')+1), orderStr.IndexOf('=') - orderStr.IndexOf(',') - 1)));
            order.setStartTime(DateTime.ParseExact(orderStr.Substring(orderStr.IndexOf('=')+1), Order.myDateFormat, provider));
            /* end parsing orderStr */

            return order;
        }
    }
}
