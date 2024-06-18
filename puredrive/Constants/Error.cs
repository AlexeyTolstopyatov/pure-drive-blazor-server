namespace puredrive.Constants
{
    public static class Error
    {
        public const string NO_EXCEPTIONS = "Задача выполнена без ошибок";
        public const string NO_EXCEPTIONS_CONTENT = "Error<TResult> вернул None";

        public const string DATA_LAYER_EXCEPTION_HEADER = "Ошибка на уровне базы данных";
        public const string DRIVE_LAYER_EXCEPTION_HEADER = "Ошибка на уровне файловой системы";
        public const string CRYPTO_FUNCTION_EXCEPTION_HEADER = "Ошибка на уровне сжатия запроса";
        public const string FRONTEND_LAYER_EXCEPTION_HEADER = "Ошибка в параметрах, переданных элементу";

    }
}
