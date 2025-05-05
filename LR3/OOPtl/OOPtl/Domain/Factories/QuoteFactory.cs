using OOPtl.Application.DTOs;

namespace OOPtl.Domain.Factories
{
    public static class QuoteFactory
    {
        public static QuoteDTO Create(string content, string author)
        {
            return new QuoteDTO(content, author);
        }
    }
}
