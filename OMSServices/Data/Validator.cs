using Client.LibBO.Transactional;
using OMSServices.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace OMSServices.Data
{
    public static class Validator
    {
        public static string ValidateFeildsAndFillData(ref IDictionary<string, object> dataDict, PropertyDescriptorCollection properties, object sourceObject)
        {
            string ErrorMsg = string.Empty;

            var orderTypevalue = properties.Find("OrdType", true)?.GetValue(sourceObject) as string;

            bool IsStop = orderTypevalue == "4" || orderTypevalue == "3";
            bool IsLmt = orderTypevalue == "4" || orderTypevalue == "2";

            foreach (PropertyDescriptor property in properties)
            {
                var ObjValue = property.GetValue(sourceObject);
                //switch (property.Name)
                //{
                //case "Side":
                //    if (ObjValue is null)
                //        ErrorMsg += "Invalid Side Selected!\r";
                //    break;
                //case "OrderQty":
                //    if ((long)ObjValue <= 0)
                //        ErrorMsg += "Invalid Quantity Provided!\r";
                //    break;
                //case "Symbol":
                //    if (string.IsNullOrEmpty(ObjValue as string))
                //        ErrorMsg += "Symbol Not Provided!\r";
                //    break;
                //case "Price":
                //    if (orderTypevalue != null && Convert.ToDecimal(ObjValue) <= 0 && IsLmt)
                //        ErrorMsg += "Invalid Price Provided!\r";
                //    break;
                //case "Account":
                //case "Destination":
                //    if (string.IsNullOrEmpty(ObjValue as string))
                //        ErrorMsg += "Invalid Account/Destination Provided!\r";
                //    break;
                //case "Rule80A":
                //    ObjValue = 'P';
                //    break;
                //case "StopPx":
                //    if (orderTypevalue != null && Convert.ToDecimal(ObjValue) <= 0 && IsStop)
                //        ErrorMsg += "Invalid Stop Price Provided!\r";
                //    break;
                //}

                dataDict[property.Name] = ObjValue;
            }

            return ErrorMsg;
        }

        //public static string FormatQuantity(object setting, long orderQty)
        //{
        //    switch (setting)
        //    {
        //        case TextFormatter textFormatter:
        //            return orderQty.ToString(textFormatter.QuantityFormat);
        //        case TicketSettings ticketSettings:
        //            return orderQty.ToString(ticketSettings.QuantityFormat);
        //        default:
        //            return orderQty.ToString();
        //    }

        //}

        //public static string FormatPrice(object setting, decimal price)
        //{
        //    if (price == 0)
        //        return "Mkt";
        //    switch (setting)
        //    {
        //        case TextFormatter textFormatter:
        //            return price.ToString(textFormatter.PriceDecimalPlaces);
        //        case TicketSettings ticketSettings:
        //            return price.ToString(ticketSettings.PriceDecimalPlaces);
        //        default:
        //            return price.ToString();
        //    }
        //}

        //public static string GetOrderSummary(BindableOEMessage OEMessage, object setting)
        //{
        //    bool IsModify = OEMessage.QOrderID != 0;

        //    return "Are you sure you want to " +
        //        ((!IsModify) ? "send the following order to '" + StaticData.Instance.CmbDestinations[OEMessage.Destination].GetValueOrDefault<string>("Name") + "' ?" + "\n\n" : "modify your order as below" + " ? " + "\n\n") +
        //        "Side         : " + StaticData.Instance.CmbSides[OEMessage.Side].GetValueOrDefault<string>("ShortDesc") + "\n" +
        //        "Symbol    : " + OEMessage.Symbol + "\n" +
        //        "Quantity  : " + FormatQuantity(setting, OEMessage.OrderQty) + "\n" +
        //        "Price        : " + FormatPrice(setting, OEMessage.Price) + "\n" +
        //        "Account   : " + StaticData.Instance.CmbAccounts[OEMessage.Account].GetValueOrDefault<string>("Name") + "\n";
        //}

        //public static string GetOrderSummary(ExpandoObject order, object setting)
        //{
        //    bool IsModify = order.GetValueOrDefault<long>("QOrderID") != 0;

        //    return "Are you sure you want to " +
        //        ((!IsModify) ? "send the following order to '" + StaticData.Instance.CmbDestinations[order.GetValueOrDefault<string>("Destination")].GetValueOrDefault<string>("Name") + "' ?" + "\n\n" : "modify your order as below" + " ? " + "\n\n") +
        //        "Side         : " + StaticData.Instance.CmbSides[order.GetValueOrDefault<string>("Side")].GetValueOrDefault<string>("ShortDesc") + "\n" +
        //        "Symbol    : " + order.GetValueOrDefault<string>("Symbol") + "\n" +
        //        "Quantity  : " + FormatQuantity(setting, order.GetValueOrDefault<long>("OrderQty")) + "\n" +
        //        "Price        : " + FormatPrice(setting, Convert.ToDecimal(order.GetValueOrDefault<object>("Price"))) + "\n" +
        //        "Account   : " + StaticData.Instance.CmbAccounts[order.GetValueOrDefault<string>("Account")].GetValueOrDefault<string>("Name") + "\n";
        //}

        public static string ValidateQuoteFeildsAndFillData(ref IDictionary<string, object> dataDict, PropertyDescriptorCollection properties, object sourceObject, bool bLocate)
        {
            string ErrorMsg = string.Empty;
            foreach (PropertyDescriptor property in properties)
            {
                var ObjValue = property.GetValue(sourceObject);
                switch (property.Name)
                {
                    case "Side":
                        if (ObjValue is null)
                            ErrorMsg += "Invalid Side Selected!\r";
                        break;
                    case "OrderQty":
                        if ((bLocate && (long)ObjValue <= 0) || ((long)ObjValue % 100) > 0)
                            ErrorMsg += "Invalid Quantity Provided! Quantity should be in 100 lots.\r";
                        break;
                    case "Symbol":
                        if (string.IsNullOrEmpty(ObjValue as string))
                            ErrorMsg += "Symbol Not Provided!\r";
                        break;
                }

                dataDict[property.Name] = ObjValue;
            }

            return ErrorMsg;
        }

        public static ServerResponse ProcessServerResponse(string stResponse)
        {
            var response = new ServerResponse();
            Dictionary<string, string> keyValuePairs = ParseMessage(stResponse);

            string stValue = "";

            if (GetField(keyValuePairs, (int)ENServerReplyFields.SERVER_REPLY_TYPE, ref stValue))
                response.ReplyType = (ENServerReplyType)Convert.ToInt32(stValue);
            else
                response.ReplyType = ENServerReplyType.SERVER_REPLY_NONE;

            if (GetField(keyValuePairs, (int)ENServerReplyFields.SERVER_REPLY_ON_OPERATION, ref stValue))
                response.Operation = (ENServerReplyOperations)Convert.ToInt32(stValue);
            else
                response.Operation = ENServerReplyOperations.OPERATION_NONE;

            if (GetField(keyValuePairs, (int)ENServerReplyFields.SERVER_REPLY_MESSAGE, ref stValue))
                response.Message = stValue;
            else
                response.Message = stResponse;

            if (GetField(keyValuePairs, (int)ENServerReplyFields.SERVER_REPLY_EXTRA_PARAMS, ref stValue))
            {
                response.ExtraParamsAvailable = true;
                response.ExtraParams = stValue;
            }
            else
                response.ExtraParamsAvailable = false;

            return response;
        }

        public static bool GetField(Dictionary<string, string> keyValuePairs, int serverReplyType, ref string stValue)
        {
            if (!keyValuePairs.ContainsKey(serverReplyType.ToString()))
                return false;

            stValue = keyValuePairs[serverReplyType.ToString()];
            return true;
        }

        private static readonly string s_regex_format_keyValueSplitter = string.Format("(?:(?:(?:(?:([\"'])(?<Key>(?:(?=(\\\\?))\\2.)*?)\\1)|(?<Key>[^\\\"{0}{1}]*))(\\=)(?:(?:([\"'])(?<Value>(?:(?=(\\\\?))\\2.)*?)\\1)|(?<Value>[^\\\"{1}]*)))({1}|$))+", Regex.Escape("=" /*KeyValueSeparator*/), Regex.Escape("|" /*ItemSeparator*/));
        private static readonly Regex s_regex_keyValueSplitter = new Regex(s_regex_format_keyValueSplitter, RegexOptions.Compiled);
        public static Dictionary<string, string> ParseMessage(string stringResponse)
        {
            var keyValuePairs = new Dictionary<string, string>();

            var match = s_regex_keyValueSplitter.Match(stringResponse);
            if (match.Groups["Key"].Captures.Count != match.Groups["Value"].Captures.Count)
            {
                return keyValuePairs;
            }

            var lstKeyValuePair = match.Groups["Key"].Captures.OfType<dynamic>().Zip(match.Groups["Value"].Captures.OfType<dynamic>(), (key, value) => new { Key = key.Value as string, Value = value.Value as string }).ToList();
            foreach (var keyValuepair in lstKeyValuePair)
            {
                if (keyValuePairs.ContainsKey(keyValuepair.Key))
                    keyValuePairs[keyValuepair.Key] = keyValuepair.Value;
                else
                    keyValuePairs.Add(keyValuepair.Key, keyValuepair.Value);
            }
            return keyValuePairs;
        }

        private static Dictionary<string, string> ParseMessageOld(string stResponse, string KeyValueSeparator = "=", string ItemSeparator = "|")
        {
            var keyValuePairs = new Dictionary<string, string>();

            string regExKeyValue = string.Format("(?:(?:(?:(?:([\"'])(?<Key>(?:(?=(\\\\?))\\2.)*?)\\1)|(?<Key>[^\\\"{0}{1}]*))(\\=)(?:(?:([\"'])(?<Value>(?:(?=(\\\\?))\\2.)*?)\\1)|(?<Value>[^\\\"{1}]*)))({1}|$))+", Regex.Escape(KeyValueSeparator), Regex.Escape(ItemSeparator));
            var keyValueSplit = new Regex(regExKeyValue, RegexOptions.Compiled);
            var match = keyValueSplit.Match(stResponse);
            if (match.Groups["Key"].Captures.Count != match.Groups["Value"].Captures.Count)
            {
                return keyValuePairs;
            }

            var lstKeyValuePair = match.Groups["Key"].Captures.OfType<dynamic>().Zip(match.Groups["Value"].Captures.OfType<dynamic>(), (key, value) => new { Key = key.Value as string, Value = value.Value as string }).ToList();
            foreach (var keyValuepair in lstKeyValuePair)
            {
                if (keyValuePairs.ContainsKey(keyValuepair.Key))
                    keyValuePairs[keyValuepair.Key] = keyValuepair.Value;
                else
                    keyValuePairs.Add(keyValuepair.Key, keyValuepair.Value);
            }
            return keyValuePairs;
        }

        //public static void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        //{
        //    ILog logger = LogManager.GetLogger("ClientUILogger");
        //    if (sender is Window window && window.Tag is string strTag)
        //        logger.Info($"Window: {strTag} raised an exception");
        //    else
        //        logger.Info($"Window: {sender.GetType().Name} raised an exception");
        //    // unable to log exception object directly in elastic search using log4net.Elasticsearch.Async thats why we use .ToString()
        //    logger.Error(e.Exception.ToString());
        //    e.Handled = false;
        //}

        internal static void FillData(ref IDictionary<string, object> dataDict, PropertyDescriptorCollection propertyDescriptorCollection, Order sourceObject)
        {
            foreach (PropertyDescriptor property in propertyDescriptorCollection)
            {
                var ObjValue = property.GetValue(sourceObject);
                dataDict[property.Name] = ObjValue;
            }
        }
    }
}
