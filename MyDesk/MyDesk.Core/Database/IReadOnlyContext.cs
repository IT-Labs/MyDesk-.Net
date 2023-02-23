using System.Linq.Expressions;

namespace MyDesk.Core.Database
{
    /// <summary>
    /// 
    /// </summary>
    public interface IReadOnlyContext
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
    }
}