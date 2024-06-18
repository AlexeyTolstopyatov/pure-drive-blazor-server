using System.Data;

namespace puredrive.Services.Base
{
    /// <summary>
    /// Содержит функции(объекты) базы данных
    /// </summary>
    public interface IDataObject
    {
        public static abstract DataTable Documents { get; }
        public static abstract DataTable Users { get; }
        public static abstract Task<string[]> DefinitionSchemas { get; }
    }
}
