using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;
using System.Net;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Runtime.Serialization.Json;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using System.IO;

namespace Microsoft.WindowsAzure.ActiveDirectory.GraphClient
{
    // text formatter for concise output
    public class GraphHelperEventTextFormatter : IEventTextFormatter
    {
        public GraphHelperEventTextFormatter() { }

        private static string[] logFields = { "strContext", "strCode", "strMessage", "strDetail" };
        
        public void WriteEvent(EventEntry eventEntry, TextWriter writer)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            writer.WriteLine("{0} {1}", eventEntry.GetFormattedTimestamp("yyyy.MM.dd hh.mm.ss"), FormatPayload(eventEntry));
            Console.ResetColor();
        }
        
        private static string FormatPayload(EventEntry entry)
        {
            string strContext = null;
            string strCode = null;
            string strMessage = null;
            string strDetail = null;
            var eventSchema = entry.Schema;
            var sb = new StringBuilder();
            for(int i = 0; i < entry.Payload.Count; i++)
            {
                switch ((string)eventSchema.Payload[i])
                {
                    case "strContext":
                        strContext = (string)entry.Payload[i];
                        break;
                    case "strCode":
                        strCode = (string)entry.Payload[i];
                        break;
                    case "strMessage":
                        strMessage = (string)entry.Payload[i];
                        break;
                    case "strDetail":
                        strDetail = (string)entry.Payload[i];
                        break;
                }
            }
            switch(entry.EventId)
            {
                case 1:
                    // auth failure
                    sb.AppendFormat("{0}: {1}\r\n", strMessage, strDetail);
                    break;
                case 2:
                    // web failure
                    sb.AppendFormat("{0}: {1}\r\n{2}\r\n", strCode, strDetail, strContext);
                    break;
            }
            return sb.ToString();
        }
    }

    public class GraphHelperEventSourceLogger
    {
        static public void Log(AdalException authException, ref string strErrors)
        {
            string strMessage = authException.Message;
            string strDetail = null;
            if (authException.InnerException != null)
            {
                // You should implement retry and back-off logic per the guidance given here:http://msdn.microsoft.com/en-us/library/dn168916.aspx
                // InnerException.Message contains the HTTP error status codes mentioned in the link above
                strDetail = authException.InnerException.Message;
                strErrors += strDetail;
            }
            GraphHelperEventSource.Log.AuthFailure(strMessage, strDetail);
        }
        static public void Log(WebException webException, ref string strErrors)
        {
            string strContext = webException.ToString();
            var errorStream = webException.Response.GetResponseStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ODataError));
            ODataError getError = (ODataError)(ser.ReadObject(errorStream));
            string strCode = getError.error.code;
            string strDetail = getError.error.message.value;
            strErrors += strDetail;
            GraphHelperEventSource.Log.WebFailure(strContext, strCode, strDetail);
        }
    }

    [EventSource(Name = "GraphHelper")]
    public class GraphHelperEventSource : EventSource
    {
        private static GraphHelperEventSource _log = new GraphHelperEventSource();
        private GraphHelperEventSource() { }
        public static GraphHelperEventSource Log { get { return _log; } }
        [Event(1, Message = "")]
        public void AuthFailure(string strMessage, string strDetail)
        {
            this.WriteEvent(1, strMessage, strDetail);
        }
        [Event(2, Message = "")]
        public void WebFailure(string strContext, string strCode, string strDetail)
        {
            this.WriteEvent(2, strContext, strCode, strDetail);
        }
    }
 }