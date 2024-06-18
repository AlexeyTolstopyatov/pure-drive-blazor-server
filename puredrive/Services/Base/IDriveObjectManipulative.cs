using System.Xml.Serialization;

namespace puredrive.Services.Base
{
    /// <summary>
    /// Интерфейс Службы манипуляции объектами диска
    /// </summary>
    public interface IDriveObjectManipulative
    {
        static abstract Task<string[]> Schemas { get; } 
        static abstract Task<string> SchemaCatalog { get; }
        static abstract Task<string> StyleCatalog { get; }

    }
}
