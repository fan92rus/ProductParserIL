using System;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;

namespace ProductParserIL
{
    public class HtmlDomTools
    {
        public ILogger logger { get; }

        public HtmlDomTools(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Получение распаршенного Angle документа
        /// </summary>
        /// <param name="content">строка с html для парсинга</param>
        /// <returns>IDocument (распершеное DOM дерево)</returns>
        public async Task<IDocument> GetDocument(string content)
        {
            try
            {
                if (content == null)
                    return null;

                var context = BrowsingContext.New(Configuration.Default);
                return await context.OpenAsync(req => req.Content(content));
            }
            catch (Exception e)
            {
                logger.LogError("",e);
            }

            return null;
        }
    }
}