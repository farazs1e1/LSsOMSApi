using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSServices.Data
{
	public class MessageProtocol
	{
		public static void GetHeaderTags(ref IDictionary<string, object> Msg, string _MsgType)
		{
			//if (string.IsNullOrEmpty(Msg["OriginatingUserDesc"].ToString()))
			//	Msg["OriginatingUserDesc"] = Msg["Traders"];
			//Msg["ClientID"] = Msg["Account"];
			//Msg["Boothid"] = "MXA";
			//Msg["Destination"] = "D0";
			if (_MsgType == MsgType.ORDERCANCELREQUEST)
			{
				Msg["MessageTag"] = (short)510;
				Msg["MsgType"] = 510;
				Msg["SubMessageTag"] = (short)Encoding.ASCII.GetBytes(_MsgType)[0];
			}
			else if (_MsgType == MsgType.CANCELREPLACEREQUEST)
			{
				Msg["MessageTag"] = (short)510;
				Msg["MsgType"] = 510;
				Msg["SubMessageTag"] = (short)Encoding.ASCII.GetBytes(_MsgType)[0];
			}
			else if (_MsgType == MsgType.NEWORDER)
			{
				Msg["MessageTag"] = (short)Encoding.ASCII.GetBytes(_MsgType)[0];
				Msg["MsgType"] = (int)Encoding.ASCII.GetBytes(_MsgType)[0];
			}

			Msg["ByteOrder"] = (char)1;
			Msg["BoothIdent"] = 'B';
		}

	}

	public struct MsgType
	{
		public const string LOGON = "A";
		public const string LOGOUT = "5";
		public const string REJECT = "3";
		public const string NEWORDER = "D";
		public const string EXECUTIONREPORT = "8";
		public const string CANCELREPLACEREQUEST = "G";
		public const string ORDERCANCELREQUEST = "F";
		public const string ORDERCANCELREJECT = "9";

	}
}
