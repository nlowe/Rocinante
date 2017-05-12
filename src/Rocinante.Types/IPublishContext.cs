namespace Rocinante.Types
{
    public interface IPublishContext
    {
         IPublishContext UseContentEngine<T>() where T : IContentEngine;
    }
}