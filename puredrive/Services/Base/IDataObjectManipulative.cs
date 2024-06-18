using puredrive.Models;

namespace puredrive.Services.Base
{
    public interface IDataObjectManipulative
    {
        static abstract Task<TaskReport> AddDocument(Document document);
        static abstract Task<TaskReport> RemoveDocument(int i);

    }
}
