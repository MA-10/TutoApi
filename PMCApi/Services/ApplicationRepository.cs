using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Test.Models;

namespace Test.Services
{
    public class ApplicationRepository<T> : IApplicationRepository<T> where T : class, IApplicationEntity
    {
        private DataContext _context;
        private UserManager<IdentityUser> _userManager;

        public ApplicationRepository(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IQueryable<T> Get()
        {
            return _context.Set<T>();
        }

        public T Get(Guid id)
        {
            return  Get().SingleOrDefault(e => e.Id == id);
        }

        public async Task<T> GetAsync(Guid id)
        {
            return await Get().SingleOrDefaultAsync(e => e.Id == id);
        }

        public void Create(T record)
        {
            try
            {
                var now = DateTime.UtcNow;
                record.CreatedOn = now;
                record.ModifiedOn = now;


                _context.Add(record);
                //_context.Entry(record).Property<DateTime>("CreatedOn").CurrentValue = now;
                //_context.Entry(record).Property<DateTime>("ModifiedOn").CurrentValue = now;
              
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Update(T record)
        {
            _context.Set<T>().Attach(record);
            _context.Entry(record).Property<DateTime>("ModifiedOn").CurrentValue = DateTime.UtcNow;
           // _context.Entry(record).State = EntityState.Modified;
        }

        public void Delete(Guid id)
        {
            var record = Get(id);

            if (record != null)
            {
                _context.Remove(record);
            }
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        public async Task<List<T>> GetAllPageableAsync(PaginationFilter paginationFilter = null,
            params SearchParameter[] filter)
        {
            var queryable = _context.Set<T>().AsQueryable();
            if (paginationFilter == null)
                return await queryable.ToListAsync();
            queryable = await AddFiltersOnQuery(filter, queryable);
            var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
            return await queryable
                .Skip(skip).Take(paginationFilter.PageSize).ToListAsync();
        }


        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }
        private static async Task<IQueryable<T>> AddFiltersOnQuery(SearchParameter[] filter,
            IQueryable<T> queryable)
        {
            if (!filter.Any()) return queryable;
            queryable = StringToLambdaFilter(filter, queryable);
            return queryable;
        }
        private static IQueryable<T> StringToLambdaFilter(SearchParameter[] filter, IQueryable<T> queryable)
        {
            var filterEx = Expression.Parameter(typeof(T), "item");
            Expression fil = Expression.Constant(true);
            var aggregate = filter.Aggregate(fil,
                (expression, parameter) => Expression.AndAlso(expression,
                    ToExpression(parameter, filterEx)));
            var whereCallExpression = WhereExpression(queryable, aggregate, filterEx);
            return queryable.Provider.CreateQuery<T>(whereCallExpression);
        }

        private static MethodCallExpression WhereExpression(IQueryable<T> queryable, Expression aggregate,
            ParameterExpression filterEx)
            => Expression.Call(
                typeof(Queryable),
                "Where",
                new[] { typeof(T) },
                queryable.Expression,
                Expression.Lambda<Func<T, bool>>(aggregate, filterEx));


        private static Expression ToExpression(SearchParameter parameter, Expression filterEx)
        {
            var nameProperty = Expression.Property(filterEx, parameter.FieldName);
            var valeurProperty = Expression.Constant(parameter.Value, parameter.Value.GetType());
            return CreateExpression(nameProperty, valeurProperty, parameter.SqlOperator);
        }

        private static Expression CreateExpression(MemberExpression nameProperty, ConstantExpression valeurProperty,
            string parameterSqlOperator)
        {
            switch (@parameterSqlOperator)
            {
                case "=":
                    return Expression.Equal(nameProperty, valeurProperty);
                case "<>":
                    return Expression.NotEqual(nameProperty, valeurProperty);
                case ">":
                    return Expression.GreaterThan(nameProperty, valeurProperty);
                case "<":
                    return Expression.LessThan(nameProperty, valeurProperty);
                case ">=" :
                    return Expression.GreaterThanOrEqual(nameProperty, valeurProperty);
                case "<=":
                    return Expression.LessThanOrEqual(nameProperty, valeurProperty);
                default:
                    throw new InvalidOperationException();


            }
            
        }
        #endregion
    }
}