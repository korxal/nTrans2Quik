using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace nTrans2Quik
{
    /// <summary>
    /// Контейнер для синхрнонизации запросов
    /// </summary>
    class AsyncContainer : IDisposable
    {
        public SemaphoreSlim Lock = new SemaphoreSlim(0, 1);  //Семафор. Должен быть заблокирован, пока не будет заполнены данные
        public object data;                                   //Возвращаемые данные запроса. Должны быть заполнены до того как отпустится семафор
        public Type DataType;                                 // Тип возвращаемых данных запроса
        public void Dispose() => Lock.Dispose();
    }


    /// <summary>
    /// Синхрноизирует запросы
    /// </summary>
    public static class RequestDispatcher
    {
        private static ConcurrentDictionary<long, AsyncContainer> RequestPool = new ConcurrentDictionary<long, AsyncContainer>();

        /// <summary>
        /// Обработчик сообщзений на которые не было запроса
        /// </summary>
        /// <param name="message"></param>
        public delegate void DeadLetterHandlerDelegate(object message);
        public static event DeadLetterHandlerDelegate DeadLetterArrived;

        static RequestDispatcher()
        {
            //ThreadPool.SetMinThreads(10, 10);
        }

        /// <summary>
        /// Ожидает, пока не придут данные на запрос посланный ранее.
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип ответа</typeparam>
        /// <param name="id">Id запроса</param>
        /// <returns>Ответ на запрос</returns>
        public static async Task<T> AwaitAsync<T>(long id)
        {
            using (AsyncContainer c = new AsyncContainer())
            {
                c.DataType = typeof(T);
                RequestPool.TryAdd(id, c);
                await c.Lock.WaitAsync();
                return (T)c.data;
            }
        }


        /// <summary>
        /// Ожидает, пока не придут данные на запрос посланный ранее.
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип ответа</typeparam>
        /// <param name="id">Id запроса</param>
        /// <returns>Ответ на запрос</returns>
        public static async Task<T> AwaitAsync<T>(long id, TimeSpan timeout, CancellationToken cancellationToken)
        {
            using (AsyncContainer c = new AsyncContainer())
            {
                c.DataType = typeof(T);
                RequestPool.TryAdd(id, c);
                await c.Lock.WaitAsync(timeout, cancellationToken);
                return (T)c.data;
            }
        }

        /// <summary>
        /// Запросить данные
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип ответа</typeparam>
        /// <param name="id">Id запроса</param>
        /// <returns>Ответ на запрос</returns>
        public static T Await<T>(long id)
        {
            using (AsyncContainer c = new AsyncContainer())
            {
                c.DataType = typeof(T);
                RequestPool.TryAdd(id, c);
                c.Lock.Wait();
                return (T)c.data;
            }
        }

        /// <summary>
        /// Запросить данные
        /// </summary>
        /// <typeparam name="T">Ожидаемый тип ответа</typeparam>
        /// <param name="id">Id запроса</param>
        /// <param name="timeout">Таймаут</param>
        /// <returns>Ответ на запрос</returns>
        public static T Await<T>(long id, TimeSpan timeout)
        {
            using (AsyncContainer c = new AsyncContainer())
            {
                c.DataType = typeof(T);
                RequestPool.TryAdd(id, c);
                c.Lock.Wait(timeout);
                return (T)c.data;
            }
        }


        /// <summary>
        /// Вызывается когда что-то приходит извне. Данные (теоритически) могут прийти без запроса.
        /// Если был запрос на эти данные, то данные будут возвращены ожидающему их запросу.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        public static void Dispatch(long id, object dto)
        {
            RequestPool.TryRemove(id, out AsyncContainer l);
            if (l != null)
            {
                if (dto.GetType() == l.DataType)
                    l.data = dto;
                else l.data = null;
                l.Lock.Release();
            }
            else
            {
                //Пришли данные, но на них не было запроса. Если есть обработчик таких данных, отдадим ему.
                if (DeadLetterArrived != null) DeadLetterArrived(dto);
            }
        }
    }
}
