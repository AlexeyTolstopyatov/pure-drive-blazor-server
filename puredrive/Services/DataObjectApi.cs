using Microsoft.Data.SqlClient;
using puredrive.Constants;
using puredrive.Models;
using puredrive.Services.Base;
using System.Diagnostics;
using System.Data;

namespace puredrive.Services
{
    /// <summary>
    /// В первую очередь этот класс представляет программный интерфейс настроенной базы данных
    /// и предоставляет возможности манипуляции с данными именно этого объекта данных
    /// </summary>
    public class DataObjectApi : Base.IDataObject, IDataObjectManipulative
    {
        /// <summary>
        /// Строка подключения к базе данных
        /// </summary>
        public static string ConnectionString { get; set; } = "server=nidompc\\sqlexpress;database=stroy;integrated security=true;trustservercertificate=true";

        /// <summary>
        /// Получает разглашаемые личные данные пользователей, хранящихся в таблице
        /// (имя и фамилия). Закрытые данные пользователя возможно получить иначе
        /// </summary>
        public static DataTable Users
        {
            get 
            {
                DataTable users = new DataTable();
                users.Columns.Add("Имя");
                users.Columns.Add("Фамилия");

                // соединение
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand command = new SqlCommand("", conn))
                {
                    conn.Open();
                    command.CommandText = "SELECT name, sname FROM [Users]";
                    
                    // заполнение
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        users.Rows.Add(reader.GetString(0), reader.GetString(1));
                    }
                }

                return (users);
            }
        }

        /// <summary>
        /// Получает список мета-данных документов в виде структуры таблицы.
        /// </summary>
        public static DataTable Documents 
        {
            get 
            {
                DataTable documents = new DataTable();
                documents.Columns.Add("Название");
                documents.Columns.Add("Тип");
                documents.Columns.Add("Размер");
                documents.Columns.Add("Номер");

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    // name content size action
                    connection.Open();
                    command.CommandText = "SELECT * FROM [Documents]";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                            {
                                documents.Rows.Add(reader.GetValue(1), reader.GetValue(2), reader.GetValue(4), reader.GetValue(0));
                            }
                        else
                        {
                            documents.Rows.Add("Ничего не нашлось", "не определен", "0", "0");
                        }
                    }
                }

                return (documents);
            }
        }

        /// <summary>
        /// Получает название зарегистрированных схем определения
        /// </summary>
        public static Task<string[]> DefinitionSchemas
        {
            get
            {
                List<string> schemas = new List<string>();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand comm = new SqlCommand("SELECT DISTINCT [DType] FROM [Documents]", conn))
                {
                    conn.OpenAsync();

                    SqlDataReader reader = comm.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        schemas.Add("Нет схем");
                        return Task.FromResult(schemas.ToArray());
                    }
                    while (reader.Read())
                    {
                        schemas.Add(reader.GetString(0));
                    }

                    return Task.FromResult(schemas.ToArray());
                }
            }
        }

        /// <summary>
        /// Получает и возвращает колличество строк в указанной таблице
        /// </summary>
        /// <param name="table">название таблицы</param>
        /// <returns>Колличество строк</returns>
        public static Task<int> Count(string table)
        {
            int count = 0;
            using (SqlConnection  conn = new SqlConnection(ConnectionString)) 
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"SELECT COUNT(*) FROM [{table}]";
                count = int.Parse(cmd.ExecuteScalar().ToString()!);
            }

            return Task.FromResult(count);
        }

        /// <summary>
        /// Проверяет данные модели пользователя на наличие записи личности в базе данных
        /// </summary>
        /// <param name="data">Результат проверки личности</param>
        /// <returns></returns>
        public static Task<TaskReport> ValidateUser(Models.User data)
        {
            // 1) initialize connection
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    connection.Open();
                    command.CommandText = "SELECT COUNT(*) FROM [Users] WHERE login = @name AND pass = @based";
                    command.Parameters.AddWithValue("@based", data.Password);
                    command.Parameters.AddWithValue("@name", data.Login);

                    object count =
                        command.ExecuteScalar();

                    // 2) write result
                    if (( int )count == 1)
                    {
                        Debug.WriteLine("Passed.");
                        return Task.FromResult(new TaskReport(ErrorLayer.None, "Проверка пройдена", "Совпадения обнаружены"));

                    }
                    else
                    {
                        // 3) if counter has errors, write it!
                        // Отчет об ошибке
                        string content = @$"name={data.Login}\npassword={data.Password}\ncounter={count}";
                        TaskReport state = 
                            new TaskReport(ErrorLayer.Data, Error.DATA_LAYER_EXCEPTION_HEADER, "Логин или Пароль не правильно указаны");

                        return Task.FromResult(state);
                    }
                }
            }
            catch (Exception ex)
            {
                // 3) if query has errors, write it TOO!
                TaskReport exception = new
                    (ErrorLayer.Data, "Исключение на уровне базы данных", ex.ToString());

                Debug.WriteLine(exception.Message);

                return Task.FromResult(exception);
            }
        }

        /// <summary>
        /// Возвращает Открытую модель данных пользователя
        /// </summary>
        /// <param name="parameter">столбец</param>
        /// <param name="value">значение</param>
        /// <returns></returns>
        public static async Task<Models.User> User(string parameter, object value)
        {
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand("", connection))
            {
                // 1) configure query
                string query = $"SELECT ID, Name, SName, Login, Type FROM [Users] WHERE {parameter} = '{value}'";
                var personal = new Models.User();
                try 
                {
                    command.CommandText = query;

                    await connection.OpenAsync();

                    var reader = await command.ExecuteReaderAsync();


                    while (reader.Read()) 
                    {
                        personal.Id = reader.GetInt32(0);
                        personal.Name = reader.GetString(1);
                        personal.SurName = reader.GetString(2);
                        personal.Login = reader.GetString(3);
                        personal.Gid = reader.GetInt32(4);
                    }
                }
                catch (Exception e) 
                {
                    Debug.WriteLine(e.Message);
                }

                return personal;
            }
        }

        /// <summary>
        /// Получает название группы по номеру
        /// </summary>
        /// <param name="i">номер группы</param>
        /// <returns></returns>
        public static Task<string> Group(int i)
        {
            string count = "no";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"SELECT Content FROM [Types] WHERE ID = @id";
                cmd.Parameters.AddWithValue("@id", i);

                Debug.WriteLine(i);
                count = cmd.ExecuteScalar().ToString()!; // null
            }

            return Task.FromResult(count);
        }

        /// <summary>
        /// Получает описание группы по номеру
        /// </summary>
        /// <param name="i">номер группы</param>
        /// <returns></returns>
        public static Task<string> GroupDescription(int i)
        {
            string count = "no";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = $"SELECT Description FROM [Types] WHERE ID = @id";
                cmd.Parameters.AddWithValue("@id", i);

                Debug.WriteLine(i);
                count = cmd.ExecuteScalar().ToString()!; // null
            }

            return Task.FromResult(count);
        }

        /// <summary>
        /// Добавляет метаданные документа в базу данных
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static Task<TaskReport> AddDocument(Document document)
        {
            return Task.FromResult(TaskReport.CompletedTask);
        }

        /// <summary>
        /// Удаляет метаданные документа из базы данных
        /// </summary>
        /// <param name="i">номер документа в базе данных</param>
        /// <returns></returns>
        public static Task<TaskReport> RemoveDocument(int i)
        {
            // wowowowow
            return Task.FromResult(new TaskReport(ErrorLayer.Data, "Не указан номер документа", "Укажите номер больше нуля"));
        }
        
        /// <summary>
        /// Выполняет DM запросы
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static async Task<TaskReport> ExecuteAsync(string query)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();

                command.ExecuteNonQuery();
            }
            
            return TaskReport.CompletedTask;
        }

        /// <summary>
        /// Выполняет запрос
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Список объектов</returns>
        public static async Task<string[]> ExecuteVectorAsync(string query)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();

                List<string> list = new List<string>();

                SqlDataReader reader = await command.ExecuteReaderAsync();
                
                while (reader.Read()) 
                {
                    list.Add(reader.GetString(0));
                }

                return list.ToArray();
            }

            //return new object[1] { Error.DATA_LAYER_EXCEPTION_HEADER };
        }

        public async static Task<object> ExecuteSingleResultAsync(string query)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();

                object result = 
                    await command.ExecuteScalarAsync();

                return (result == null) ? new object[1] { 
                    Error.DATA_LAYER_EXCEPTION_HEADER
                } : result;
            }
        }
    }
}
