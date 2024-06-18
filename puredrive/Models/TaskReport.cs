using puredrive.Constants;

namespace puredrive.Models
{
    public enum ErrorLayer
    {
        None,        // нет причины ошибки
        Data,        // Sql Server 
        Drive,       // файловая система
        Frontend,    // razor
        Crypto       // сжатие
    }

    /// <summary>
    /// Класс, представляющий возможность создавать подробный отчет о операции.
    /// </summary>
    public class TaskReport
    {
        /// <summary>
        /// Шаблон завершенной задачи
        /// </summary>
        public static TaskReport CompletedTask { get; } = new TaskReport();
        
        /// <summary>
        /// Шаблон отчета о завершенных задачах
        /// </summary>
        public static TaskReport[] Perfect 
        {
            get => new TaskReport[1] { CompletedTask };                
        }

        public TaskReport(ErrorLayer source, string? message, string? content)
        {
            Source = source; 
            Message = message;
            Content = content;
        }

        public TaskReport()
        {
            Source = ErrorLayer.None;
            Message = Error.NO_EXCEPTIONS;
            Content = Error.NO_EXCEPTIONS_CONTENT;
        }


        /// <summary>
        /// Слой проекта, где произошла ошибка
        /// </summary>
        public ErrorLayer Source { get; private set; }

        /// <summary>
        /// Главное сообщение
        /// </summary>
        public string? Message { get; private set; }
        
        /// <summary>
        /// Подробная информация
        /// </summary>
        public string? Content { get; private set; }

        public static bool HasErrors(TaskReport[] reference) 
        {
            foreach (TaskReport task in reference)
            {
                if (task.Source != ErrorLayer.None)
                    return false;
            }

            return true;
        }
    }
}
