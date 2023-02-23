namespace MyDesk.Core.Database
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntity<T>
    {
        /// <summary>
        /// </summary>
        T Id { get; set; }
    }
}
