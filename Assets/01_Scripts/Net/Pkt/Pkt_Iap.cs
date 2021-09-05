/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace SH.Net.Pkt
{
    public class ReqIapVerifyReceipt : IRequest
    {
        public string productID;
        public string purchase_type;
        public string purchase_data;
        public string data_signature;

        public string currency_code;
        public string currency_price;
        
        public ReqIapVerifyReceipt() : base("shop/buy/cash")
        {
        }
    }

    public class RecvIapVerifyReceipt : Recv
    {
            
    }
}