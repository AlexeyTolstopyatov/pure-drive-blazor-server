using System.Text;

namespace puredrive.Models
{
    public enum Flag
    {
        Common,
        DefinitionSchema,
        StyleSchema
    }

    /// <summary>
    /// Я сам пока что не понимаю что это.
    /// </summary>
    public class File
    {
        /// <summary>
        /// Системный флаг нужен для определения файла
        /// Если системный флаг стоит, значит 
        /// </summary>
        public Flag SystemFlag { get; set; } = Flag.Common;
        
        /// <summary>
        /// Номер файла в базе данных
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Рассчитывает контрольную сумму файла
        /// </summary>
        public string Checksum
        {
            get 
            {
                try 
                {
                    return "0x00";
                }
                catch
                {
                    return "0x00";
                }
            }
        }

        public File(int id)
        {
            Id = id;
        }
    }
}
