using puredrive.Models;
using puredrive.Constants;

namespace puredrive.Services
{
    /// <summary>
    /// Проверяет существующую личность (в базе данных)
    /// </summary>
    public class DriveAuthentication
    {
        public DriveAuthentication() 
        {
            Report = new List<TaskReport>();
        }

        public List<TaskReport> Report { get; set; } 

        /// <summary>
        /// Изменяемая модель пользователя
        /// </summary>
        public Models.User Identity { get; set; } = new Models.User();

        /// <summary>
        /// Служба аутентификации. Если Отчет отрицательный (Не сопоставляется с шаблоном), операция выполнена с ошибками
        /// </summary>
        public async Task Run() 
        {
            // чтобы не было NULL
            await Fill();
            await Transform();
            await Compare();
        }
        #region бизнесс.....логика

        // 1) проверка данных на пустоту
        private Task Fill()
        {
            if (Identity is null)
            {
                Report.Add(new TaskReport(ErrorLayer.Frontend, "Модель данных пуста", "Инициализирована но не реализована. NULL"));
                Identity = new Models.User();
            }

            if (Identity.Login == Constants.User.NO_ID && Identity.Password == Constants.User.NO_PASSWORD)
            {
                Report.Add(new TaskReport(ErrorLayer.Frontend, "Пользователя нет в системе", "Логин пользователя не может быть NID"));
            }

            Report.Add(new TaskReport(ErrorLayer.None, "Данные заполнены", ""));

            return Task.CompletedTask;
        }

        // 2) Трансформация данных в формат базы
        private Task Transform()
        {
            Identity.Login = Deflate.FastCompress(Identity.Login);
            Identity.Password = Deflate.FastCompress(Identity.Password);

            Report.Add(new TaskReport(ErrorLayer.None, "Данные изменены", "Данные для переадресации в базу данных готовы."));

            return Task.CompletedTask;
        }

        // 3) соедиение с SQL Server
        private async Task Compare()
        {
            TaskReport dbo = await DataObjectApi.ValidateUser(Identity);
            
            Report.Add(dbo);
        }
        #endregion
    }
}
