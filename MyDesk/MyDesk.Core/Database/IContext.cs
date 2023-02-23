using System.Linq.Expressions;

namespace MyDesk.Core.Database
{
    /// <summary>
    /// Context Interface
    /// </summary>
    public interface IContext
    {
        /// <summary>B
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQueryable<T> AsQueryable<T>() where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQueryable<T> AsNoTracking<T>() where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void Insert<T>(T item) where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        void Insert<T>(IEnumerable<T> items) where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        void Update<T>(IEntity<T> item) where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        void UpdateAsync<T>(IEntity<T> item) where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void Modify<T>(T item) where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TField"></typeparam>
        void UpdateMany<T, TField>(Expression<Func<T, bool>> expression, Expression<Func<T, TField>> field, TField value)
            where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <param name="expression"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        void UpdateManyAsync<T, TField>(Expression<Func<T, bool>> expression, Expression<Func<T, TField>> field, TField value) where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        void UpdateMany<T>(IEnumerable<T> items) where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        void Delete<T>(IEntity<T> item) where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <param name="exp"></param>
        /// <typeparam name="T"></typeparam>
        void Delete<T>(Expression<Func<T, bool>> exp) where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <param name="exp"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> FindItems<T>(Expression<Func<T, bool>> exp) where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        long Count<T>(Expression<Func<T, bool>> exp) where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <param name="exp"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>(Expression<Func<T, bool>> exp) where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IList<T> AllItems<T>() where T : class;

        /// <summary>
        ///
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        void ExecuteSqlQuery(string query, IEnumerable<object> parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        string ExecuteScalar(string query);

        /// <summary>
        ///
        /// </summary>
        object BeginTransaction();
    }
}
