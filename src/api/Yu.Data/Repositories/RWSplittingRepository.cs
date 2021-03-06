﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Yu.Data.Entities;

namespace Yu.Data.Repositories
{
    public class RWSplittingRepository<TEntity, TPrimaryKey, TReadDbContext, TWriteDbContext> : IRepository<TEntity, TPrimaryKey>
        where TEntity : BaseEntity<TPrimaryKey>
        where TReadDbContext : DbContext
        where TWriteDbContext : DbContext
    {
        private readonly DbContext _readContext;
        private readonly DbContext _writeContext;

        private readonly DbSet<TEntity> _readDataSet;
        private readonly DbSet<TEntity> _writeDataSet;

        public RWSplittingRepository(IHttpContextAccessor httpContextAccessor)
        {
            _readContext = httpContextAccessor.HttpContext.RequestServices.GetService<TReadDbContext>();
            _readDataSet = _readContext.Set<TEntity>();

            _writeContext = httpContextAccessor.HttpContext.RequestServices.GetService<TWriteDbContext>();
            _writeDataSet = _readContext.Set<TEntity>();
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="entity">删除的数据</param>
        public void Delete(TEntity entity)
        {
            _writeDataSet.Remove(entity);
        }

        /// <summary>
        /// 根据Id删除
        /// </summary>
        /// <param name="key">数据的主键</param>
        public void Delete(TPrimaryKey key)
        {
            Delete(GetById(key));
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="entities">删除的数据</param>
        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            _writeDataSet.RemoveRange(entities);
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <param name="where">删除条件表达式</param>
        public void DeleteRange(Expression<Func<TEntity, bool>> where)
        {
            DeleteRange(GetByWhere(where));
        }

        /// <summary>
        /// 全体数据查询
        /// </summary>
        /// <returns>数据查询结果</returns>
        public IQueryable<TEntity> GetAll()
        {
            return _readDataSet.AsNoTracking();
        }

        /// <summary>
        /// 全体数据查询（不跟踪数据状态）
        /// </summary>
        /// <returns>数据查询结果</returns>
        public IQueryable<TEntity> GetAllNoTracking()
        {
            return _readDataSet.AsNoTracking();
        }

        /// <summary>
        /// 通过主键获取对象
        /// </summary>
        /// <param name="key">主键值</param>
        /// <returns>数据查询结果</returns>
        public TEntity GetById(TPrimaryKey key)
        {
            return _readDataSet.Find(key);
        }

        /// <summary>
        /// 取得分页数据
        /// </summary>
        /// <param name="query">原始队列</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns>数据查询结果</returns>
        public PagedData<TEntity> GetByPage(IQueryable<TEntity> query, int pageIndex, int pageSize)
        {
            var skip = pageSize * (pageIndex - 1);
            return new PagedData<TEntity>
            {
                Total = query.Count(), // 满足条件总条数
                Data = query.Skip(skip).Take(pageSize).ToList()
            };
        }

        /// <summary>
        /// 根据条件检索对象
        /// </summary>
        /// <param name="where">检索条件</param>
        /// <returns>数据查询结果</returns>
        public IQueryable<TEntity> GetByWhere(Expression<Func<TEntity, bool>> where)
        {
            return _readDataSet.Where(where);
        }

        /// <summary>
        /// 条件检索对象（不跟踪数据状态）
        /// </summary>
        /// <param name="where">检索条件</param>
        /// <returns>数据查询结果</returns>
        public IQueryable<TEntity> GetByWhereNoTracking(Expression<Func<TEntity, bool>> where)
        {
            return _readDataSet.AsNoTracking().Where(where);
        }

        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <param name="entity">对象数据</param>
        /// <returns>实体对象</returns>
        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var result = await _writeContext.AddAsync(entity);
            return result.Entity;
        }

        /// <summary>
        /// 插入一组数据
        /// </summary>
        /// <param name="entity">对象数据</param>
        /// <returns>实体对象</returns>
        public async Task InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            await _writeContext.AddRangeAsync(entities);
        }

        /// <summary>
        /// 数据排序（逆序）
        /// </summary>
        /// <param name="query">原始队列</param>
        /// <param name="order">排序表达式</param>
        /// <returns>数据查询结果</returns>
        public IQueryable<TEntity> OrderDescQuery(IQueryable<TEntity> query, Expression<Func<TEntity, object>> order)
        {
            return query.OrderByDescending(order);
        }

        /// <summary>
        /// 数据排序（正序）
        /// </summary>
        /// <param name="query">原始队列</param>
        /// <param name="order">排序表达式</param>
        /// <returns>数据查询结果</returns>
        public IQueryable<TEntity> OrderQuery(IQueryable<TEntity> query, Expression<Func<TEntity, object>> order)
        {
            return query.OrderBy(order);
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="entity">更新的对象</param>
        public void Update(TEntity entity)
        {
            // 关联实体,然后设置状态
            _writeContext.Attach(entity);
            _writeContext.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// 更新部分属性
        /// </summary>
        /// <param name="entity">更新的对象</param>
        /// <param name="properties">更新的属性表达式</param>
        public void UpdatePartial(TEntity entity, Expression<Func<TEntity, object>>[] properties)
        {
            // 关联实体,然后设置状态
            _writeContext.Attach(entity);
            foreach (var property in properties)
            {
                _writeContext.Entry(entity).Property(property).IsModified = true;
            }
        }

        /// <summary>
        /// 批量更新对象
        /// </summary>
        /// <param name="entities">批量更新的对象</param>
        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            _writeContext.UpdateRange(entities);
        }
    }
}
