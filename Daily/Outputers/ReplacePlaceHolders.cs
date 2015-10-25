namespace Daily
{
    public class ReplacePlaceHolders
    {
        public const string LINE = "{0}";
        public const string SPACE = "{1}";
        public const string SPAN_SMALL = "{2}";
        public const string SPAN_RED = "{3}";
        public const string SPAN_GREEN = "{4}";
        public const string CLOSE_SPAN = "{5}";
        public const string DIV_BOLD_UNDERLINE = "{6}";
        public const string CLOSE_DIV = "{7}";
        public const string START_LINK_OPEN = "{8}";
        public const string START_LINK_CLOSE = "{9}";
        public const string CLOSE_LINK = "{10}";


        private MessageBuilder _messageBuilder;

        public ReplacePlaceHolders(MessageBuilder messageBuilder)
        {
            _messageBuilder = messageBuilder;
        }

        public string GetTextMessage()
        {
            return _messageBuilder.Message
                .Replace(SPAN_SMALL, "")
                .Replace(SPAN_RED, "")
                .Replace(SPAN_GREEN, "")
                .Replace(CLOSE_SPAN, "")
                .Replace(LINE, "\n")
                .Replace(DIV_BOLD_UNDERLINE, "")
                .Replace(CLOSE_DIV, "")
                .Replace(SPACE, " ")
                .Replace(START_LINK_OPEN, " ")
                .Replace(START_LINK_CLOSE, " ")
                .Replace(CLOSE_LINK, " ")
            ;
        }

        public string GetHtmlMessage()
        {
            return _messageBuilder.Message.ToRawHtml()
                .Replace(SPAN_SMALL, "<span style='font-size: 10pt'>")
                .Replace(SPAN_RED, "<span style = 'color:red'>")
                .Replace(SPAN_GREEN, "<span style = 'color:green'>")
                .Replace(CLOSE_SPAN, "</span>")
                .Replace(LINE, "<br>")
                .Replace(DIV_BOLD_UNDERLINE, "<div style='text-decoration: underline; font-weight: bold;'>")
                .Replace(CLOSE_DIV, "</div>")
                .Replace(SPACE, "&nbsp")
                .Replace(START_LINK_OPEN, "<a href ='")
                .Replace(START_LINK_CLOSE, "'>")
                .Replace(CLOSE_LINK, "</a>");
        }
    }
}