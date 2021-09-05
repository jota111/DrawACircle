/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using UnityEngine;

namespace SH.Iap
{
    public static class IapHelper
    {
        public static (string purchase, string signature) GetReceiptInfo(string receipt)
        {
            var purchaseData = string.Empty;
            var dataSignature = string.Empty;

#if UNITY_EDITOR
            return (purchaseData, dataSignature);
#endif

#if UNITY_ANDROID && !UNITY_AWS
            var aosReceipt = new AOSReceipt(receipt);
            var receiptSignedData = aosReceipt.GetSignedData();
            purchaseData = receiptSignedData.Json;
            dataSignature = aosReceipt.payload.signature;

#elif UNITY_ANDROID && UNITY_AWS
        NewReceipt awsReceipt = JsonUtility.FromJson<NewReceipt>(receipt);
        purchaseData = awsReceipt.Payload;


#if __DEV && !UNITY_AWS
            Debug.Log(receiptSignedData.Json);
            Debug.Log(aosReceipt.payload.signature);
#endif

#elif UNITY_IOS
        NewReceipt iOSReceipt = JsonUtility.FromJson<NewReceipt>(receipt);
        /*
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        var appleConfig = builder.Configure<IAppleConfiguration>();
        var receiptData = System.Convert.FromBase64String(appleConfig.appReceipt);
        AppleReceipt receipt2 = new AppleValidator(AppleTangle.Data()).Validate(receiptData);
        */
        purchaseData = iOSReceipt.Payload;
        dataSignature = "";
#endif

        return (purchaseData, dataSignature);
        }
    }
}