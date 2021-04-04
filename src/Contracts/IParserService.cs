namespace LispParser.Contracts
{
    public interface IParserService
    {
        string RemoveCommentFromRow(string row);
        string RemoveComments(string contents);
        string RemoveNextQuoteSet(string contents, int index);
        string RemoveQuotes(string contents);
        void ValidateCount(string content);
        void ValidateNesting(string contents);
    }
}
