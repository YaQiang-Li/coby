using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CefSharp.Wpf.Copy.Helpers
{
    class ParseHtmlHelper
    {
        public const string RegexForTradeID = @"订单编号：(\d+)";
        public const string RegexForTime_stamp = @"成交时间：(.*)</span>";
        public const string RegexForSubTradeID = @"<tr id=""item(\d+)";
        public const string RegexForItem = @"title=""查看宝贝详情"" target=""_blank"">(\s+){0,1}(.*)";
        public const string RegexForPrice = @"<td class=""price"" title=""(\d+\.\d+)"">";
        public const string RegexForNum = @"<td class=""num"" title=""(\d+)"">";
        public const string RegexForTrouble = @"<td class=""trouble"">";   /* 暂时忽略 */
        public const string RegexForContact = @"<a class=""nickname"" href="".*userID=.*?>(.*).?</a>";
        public const string RegexForTrade_status = @"<strong class=""J_TradeStatus.*?>(\s+){0,1}(.*)";    /* 有问题，有些后面会带</strong> */
        public const string RegexForOrder_price = @"<strong class=""J_OrderPrice"">(.*)</strong>";
        public const string RegexForRemark = @"<td class=""remark"" rowspan=""1"" sumrows=""1"">";   /* 暂时忽略 */
        public const string RegexForMessage = @"{\s+""message"":""(.*)"",";

        public const string RegexForMessageUrl = @"<span class=""name J_UserInfo"" data=""(.*)"">";  /* 取的数据所在地址，要通过http拿下来 */
        public MatchCollection MessageUrlMatchs;

        public MatchCollection[] RegexMatchs;
        public string strInput;

        public ParseHtmlHelper()
        {
            RegexMatchs = new MatchCollection[12];
        }

        public ParseHtmlHelper(string innerHtml)
        {
            RegexMatchs = new MatchCollection[12];
            strInput = innerHtml;
        }

        public void SetInput(string innerHtml)
        {
            strInput = innerHtml;
        }

        public bool ExecuteRegex(int m_index, string strRegex)
        {
            MatchCollection m_match = Regex.Matches(strInput, strRegex);
            RegexMatchs[m_index] = m_match;
            return true;
        }

        public bool ExecuteRegex(int m_index, string input, string strRegex)
        {
            MatchCollection m_match = Regex.Matches(input, strRegex);
            RegexMatchs[m_index] = m_match;
            return true;
            return true;
        }

        public bool ExecuteAllRegex()
        {
            if (ExecuteRegex(0, RegexForTradeID) == false) return false;
            if (ExecuteRegex(1, RegexForTime_stamp) == false) return false;
            if (ExecuteRegex(2, RegexForSubTradeID) == false) return false;
            if (ExecuteRegex(3, RegexForItem) == false) return false;
            if (ExecuteRegex(4, RegexForPrice) == false) return false;
            if (ExecuteRegex(5, RegexForNum) == false) return false;
            if (ExecuteRegex(6, RegexForTrouble) == false) return false;
            if (ExecuteRegex(7, RegexForContact) == false) return false;
            if (ExecuteRegex(8, RegexForTrade_status) == false) return false;
            if (ExecuteRegex(9, RegexForOrder_price) == false) return false;
            if (ExecuteRegex(10, RegexForRemark) == false) return false;
            if (ExecuteRegex(11, MainWindow.strMessageHtml, RegexForMessage) == false) return false;

            return true;
        }

        public bool GetMessageUrl()
        {
            MatchCollection m_match = Regex.Matches(strInput, RegexForMessageUrl);
            MessageUrlMatchs = m_match;
            return true;
        }

        public bool ExecuteAllRegex(string input)
        {

            return true;
        }
    }
}
