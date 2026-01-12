
using System.Linq.Expressions;


namespace LandingPageApp.Domain.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Trả về IQueryable để service có thể thực hiện các thao tác query động:
        /// filter, sort, include, pagination...
        /// Repo không xử lý business logic, mà chỉ cung cấp nguồn dữ liệu thô.
        /// </summary>
        IQueryable<T> Query();
        IQueryable<T> QueryNoTracking();
        /// <summary>
        /// Lấy một entity theo Id.
        /// Trả về null nếu không tìm thấy.
        /// Dùng FindAsync để tận dụng EF Core tracking và cache.
        /// </summary>
        Task<T?> GetByIdAsync(long id, CancellationToken cancellation = default);
        Task AddAsync(T entity, CancellationToken cancellation = default);
        void Update(T entity);
        void Delete(T entity);

        /// <summary>
        /// Lấy danh sách entity thỏa predicate (biểu thức điều kiện).
        /// Ví dụ: FindAsync(x => x.Status == true)
        /// Phù hợp khi cần filter trước khi trả về.
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate,CancellationToken cancellation = default);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate ,CancellationToken cancellation = default);

        /// <summary>
        /// Lấy toàn bộ dữ liệu của entity.
        /// Hữu ích cho các tình huống đơn giản, admin, hoặc các dropdown.
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellation = default);
    }
}
