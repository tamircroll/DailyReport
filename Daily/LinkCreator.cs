namespace Daily
{
    class LinkCreator
    {
        public string makeLink(string name, string link)
        {
            return string.Format("{0}{1}{2}{3}{4}",
                ReplacePlaceHolders.START_LINK_OPEN,
                link,
                ReplacePlaceHolders.START_LINK_CLOSE, 
                name,
                ReplacePlaceHolders.CLOSE_LINK);
        }
    }
}
