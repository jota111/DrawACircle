/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using UnityEngine;

namespace SH.Iap
{
    [Serializable]
    public class NewReceipt
    {
        public string Store;
        public string TransactionID;
        public string Payload;

#if __DEV
        public void Log()
        {
            Debug.Log("<< NewReceipt >>");
            Debug.Log("Store : " + Store);
            Debug.Log("TransactionID : " + TransactionID);
            Debug.Log("Payload : " + Payload);
        }
#endif
    }

    [Serializable]
    public class Payload
    {
        public string json;
        public string signature;
        public string skuDetails;
        public bool isPurchaseHistorySupported;

#if __DEV
        public void Log()
        {
            Debug.Log("<< Payload >>");
            Debug.Log("json : " + json);
            Debug.Log("signature : " + signature);
            Debug.Log("skuDetails : " + skuDetails);
            Debug.Log("isPurchaseHistorySupported : " + isPurchaseHistorySupported);
        }
#endif
    }

    [Serializable]
    public class PayLoadJson
    {
        public string orderId;
        public string packageName;
        public string productId;
        public long purchaseTime;
        public int purchaseState;
        public string developerPayload;
        public string purchaseToken;

#if __DEV
        public void Log()
        {
            Debug.Log("<< PayLoadJson >>");
            Debug.Log("orderId : " + orderId);
            Debug.Log("packageName : " + packageName);
            Debug.Log("productId : " + productId);
            Debug.Log("purchaseTime : " + purchaseTime);
            Debug.Log("purchaseState : " + purchaseState);
            Debug.Log("developerPayload : " + developerPayload);
            Debug.Log("purchaseToken : " + purchaseToken);
        }
#endif
    }

    [Serializable]
    public class DeveloperPayload
    {
        public string developerPayload;
        public bool is_free_trial;
        public bool has_introductory_price_trial;
        public bool is_updated;
        public string accountId;

#if __DEV
        public void Log()
        {
            Debug.Log("<< DeveloperPayload >>");
            Debug.Log("developerPayload : " + developerPayload);
            Debug.Log("is_free_trial : " + is_free_trial);
            Debug.Log("has_introductory_price_trial : " + has_introductory_price_trial);
            Debug.Log("is_updated : " + is_updated);
            Debug.Log("accountId : " + accountId);
        }
#endif
    }

    [Serializable]
    public class SKUDetails
    {
        public string skuDetailsToken;
        public string productId;
        public string type;
        public string price;
        public long price_amount_micros;
        public string price_currency_code;
        public string title;
        public string description;

#if __DEV
        public void Log()
        {
            Debug.Log("<< SKUDetails >>");
            Debug.Log("skuDetailsToken : " + skuDetailsToken);
            Debug.Log("productId : " + productId);
            Debug.Log("type : " + type);
            Debug.Log("price : " + price);
            Debug.Log("price_amount_micros : " + price_amount_micros);
            Debug.Log("price_currency_code : " + price_currency_code);
            Debug.Log("title : " + title);
            Debug.Log("description : " + description);
        }
#endif
    }

    [Serializable]
    public class ReceiptSignedData
    {
        public string orderId;
        public string packageName;
        public string productId;
        public long purchaseTime;
        public int purchaseState;
        public string developerPayload;
        public string purchaseToken;

        public ReceiptSignedData(PayLoadJson json, SKUDetails skuDetails, string devPayLoad)
        {
            orderId = json.orderId;
            packageName = json.packageName;
            productId = skuDetails.productId;
            purchaseTime = json.purchaseTime;
            purchaseState = json.purchaseState;
            developerPayload = devPayLoad;
            purchaseToken = json.purchaseToken;
        }

        public string Json
        {
            get { return UnityEngine.Networking.UnityWebRequest.EscapeURL(JsonUtility.ToJson(this)); }
        }
    }

    public class AOSReceipt
    {
        public NewReceipt newReceipt = null;
        public Payload payload = null;
        public PayLoadJson payLoadJson = null;
        public SKUDetails skuDetails = null;
        public DeveloperPayload developerPayload = null;

        public AOSReceipt(string rawReceipt)
        {
            newReceipt = JsonUtility.FromJson<NewReceipt>(rawReceipt);
            //newReceipt.Log();

            payload = JsonUtility.FromJson<Payload>(newReceipt.Payload);
            //payload.Log();

            payLoadJson = JsonUtility.FromJson<PayLoadJson>(payload.json);
            //payLoadJson.Log();

            skuDetails = JsonUtility.FromJson<SKUDetails>(payload.skuDetails);
            //skuDetails.Log();

            developerPayload = JsonUtility.FromJson<DeveloperPayload>(payLoadJson.developerPayload);
            //developerPayload.Log();
        }

        public ReceiptSignedData GetSignedData()
        {
            if (payLoadJson == null || skuDetails == null)
                return null;

            return new ReceiptSignedData(payLoadJson, skuDetails, JsonUtility.ToJson(developerPayload));
        }
    }
}