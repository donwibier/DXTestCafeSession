using AutoMapper;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DXTestCafe.TestProject.Code
{
	public interface IDataStore<TKey, TModel>
		where TKey : IEquatable<TKey>
		where TModel : class
	{
		Task<List<TModel>> SelectAsync(Expression<Func<TModel, bool>> where, int startIndex, int numItems, params IOrderByExpression<TModel>[] orderBy);
		List<TModel> Select(Expression<Func<TModel, bool>> where, int startIndex, int numItems, params IOrderByExpression<TModel>[] orderBy);

		//Task<List<TModel>> SelectAsync(Expression<Func<TModel, bool>> predicate, int startIndex, int numItems, params Expression<Func<TModel, IComparable>>[] orderBy);
		//List<TModel> Select(Expression<Func<TModel, bool>> predicate, int startIndex, int numItems, params Expression<Func<TModel, IComparable>>[] orderBy);

		//Task<List<TModel>> SelectAsync(Func<TModel, bool> predicate, int startIndex = 0, int numItems = 0, EFOrderBy<TKey, TModel>[] orderBy);
		//List<TModel> Select(Func<TModel, bool> predicate, int startIndex = 0, int numItems = 0, EFOrderBy<TKey, TModel>[] orderBy);

		IMapper Mapper { get; }

		TModel GetByKey(TKey key);

		Task<LoadResult> SelectWithOptionsAsync(DataSourceLoadOptionsBase loadOptions,
			CancellationToken cancellationToken = default);

		DataValidationResults<TKey> Create(params TModel[] items);

		Task<DataValidationResults<TKey>> CreateAsync(params TModel[] items);

		DataValidationResults<TKey> Update(params TModel[] items);

		Task<DataValidationResults<TKey>> UpdateAsync(params TModel[] items);

		DataValidationResults<TKey> Store(params TModel[] items);

		Task<DataValidationResults<TKey>> StoreAsync(params TModel[] items);

		DataValidationResults<TKey> Delete(params TKey[] ids);

		Task<DataValidationResults<TKey>> DeleteAsync(params TKey[] ids);

		IQueryable<T> Query<T>() where T : class, new();

		IQueryable<TModel> Query();

		TKey ModelKey(TModel model);
	}

	public interface IOrderByExpression<TEntity> where TEntity : class
	{
		IOrderedQueryable<TEntity> ApplyOrderBy(IQueryable<TEntity> query);
		IOrderedQueryable<TEntity> ApplyThenBy(IOrderedQueryable<TEntity> query);
	}
	public abstract class EFDataStore<TEFContext, TKey, TModel, TDBModel> : IDataStore<TKey, TModel>
		where TEFContext : DbContext
		where TKey : IEquatable<TKey>
		where TModel : class
		where TDBModel : class, new()
	{
		public EFDataStore(TEFContext context,
						IMapper mapper,
						DataValidator<TEFContext, TKey, TModel, TDBModel> validator)
		{
			Validator = validator;
			Mapper = mapper;
			DbContext = context;
		}

		public class OrderBy<TOrderBy> : IOrderByExpression<TModel>
		{
			private Expression<Func<TModel, TOrderBy>> expression;
			private bool descending;

			public OrderBy(Expression<Func<TModel, TOrderBy>> expression, bool descending = false)
			{
				this.expression = expression;
				this.descending = descending;
			}

			public IOrderedQueryable<TModel> ApplyOrderBy(IQueryable<TModel> query)
			{
				return (descending) ? query.OrderByDescending(expression) : query.OrderBy(expression);
			}

			public IOrderedQueryable<TModel> ApplyThenBy(IOrderedQueryable<TModel> query)
			{
				return (descending) ? query.ThenByDescending(expression) : query.ThenBy(expression);
			}
		}



		public abstract TKey ModelKey(TModel model);

		public abstract TKey DBModelKey(TDBModel model);

		public IMapper Mapper { get; }

		public TEFContext DbContext { get; }

		public DataValidator<TEFContext, TKey, TModel, TDBModel> Validator { get; }

		protected virtual TDBModel EFGetByKey(TKey key)
		{
			return DbContext.Find<TDBModel>(key);
		}

		public virtual TModel GetByKey(TKey key)
		{
			TDBModel result = EFGetByKey(key);
			if (result != null)
			{
				var r = Mapper.Map(result, typeof(TDBModel), typeof(TModel));
				return r as TModel;
			}

			return default;
		}

		protected virtual IQueryable<TDBModel> EFQuery()
		{
			var results = DbContext.Set<TDBModel>();
			return results;
		}

		public virtual IQueryable<T> Query<T>()
			where T : class, new()
		{
			return EFQuery().ProjectTo<T>(Mapper.ConfigurationProvider);
		}
		public virtual IQueryable<TModel> Query()
		{
			return EFQuery().ProjectTo<TModel>(Mapper.ConfigurationProvider);
		}

		public async virtual Task<LoadResult> SelectWithOptionsAsync(DataSourceLoadOptionsBase loadOptions,
			CancellationToken cancellationToken = default)
		{
			if (loadOptions == null)
				throw new ArgumentNullException(nameof(loadOptions));

			var loadResult = await DataSourceLoader.LoadAsync(Query(), loadOptions, cancellationToken);
			return loadResult;
		}

		protected virtual IQueryable<TModel> Query(Expression<Func<TModel, bool>> where,
			int startIndex, int numItems,
			params IOrderByExpression<TModel>[] orderBy)
		{
			if (where == null)
				throw new ArgumentNullException(nameof(where));

			var query = Query().Where(where);

			if (startIndex > 0)
				query = query.Skip(startIndex);
			if (numItems > 0)
				query = query.Take(numItems);

			if (orderBy == null)
				return query;

			IOrderedQueryable<TModel> result = null;
			foreach (var orderExpr in orderBy)
			{
				if (result == null)
					result = orderExpr.ApplyOrderBy(query);
				else
					result = orderExpr.ApplyThenBy(result);
			}

			return result ?? query;
		}

		public virtual List<TModel> Select(Expression<Func<TModel, bool>> where,
			int startIndex, int numItems,
			params IOrderByExpression<TModel>[] orderBy)
		{
			return Query(where, startIndex, numItems, orderBy).ToList();
		}


		public async virtual Task<List<TModel>> SelectAsync(
			Expression<Func<TModel, bool>> where,
			int startIndex,
			int numItems,
			params IOrderByExpression<TModel>[] orderBy)
		{
			return await Query(where, startIndex, numItems, orderBy).ToListAsync();
		}

		public virtual DataValidationResults<TKey> Create(params TModel[] items)
		{
			if (items == null)
				throw new ArgumentNullException(nameof(items));

			var result = TransactionalExec((s, t) =>
			{
				DataValidationResults<TKey> r = new DataValidationResults<TKey>();

				foreach (var item in items)
				{
					r = InternalCreate(s, t, item, r);
				}
				try
				{
					s.DbContext.SaveChanges();
					t.Commit();
				}
				catch (Exception e)
				{
					r.Add(new DataValidationResult<TKey>
					{
						ResultType = DataValidationResultType.Error,
						Message = e.InnerException != null ? e.InnerException.Message : e.Message
					});
				}
				return r;
			},
										false);
			return result;
		}


		public async virtual Task<DataValidationResults<TKey>> CreateAsync(params TModel[] items)
		{
			return await Task.FromResult(Create(items));
		}

		public virtual DataValidationResults<TKey> Update(params TModel[] items)
		{
			if (items == null)
				throw new ArgumentNullException(nameof(items));

			var result = TransactionalExec((s, t) =>
			{
				DataValidationResults<TKey> r = new DataValidationResults<TKey>();

				foreach (var item in items)
				{
					r = InternalUpdate(s, t, ModelKey(item), item, r);
				}
				try
				{
					s.DbContext.SaveChanges();
					t.Commit();
				}
				catch (Exception e)
				{
					r.Add(new DataValidationResult<TKey>
					{
						ResultType = DataValidationResultType.Error,
						Message = e.InnerException != null ? e.InnerException.Message : e.Message
					});
				}
				return r;
			},
										false);
			return result;
		}

		public async virtual Task<DataValidationResults<TKey>> UpdateAsync(params TModel[] items)
		{
			return await Task.FromResult(Update(items));
		}

		public virtual DataValidationResults<TKey> Store(params TModel[] items)
		{
			if (items == null)
				throw new ArgumentNullException(nameof(items));

			var result = TransactionalExec((s, t) =>
			{
				DataValidationResults<TKey> r = new DataValidationResults<TKey>();

				foreach (var item in items)
				{
					var key = ModelKey(item);
					r = (EmptyKeyValue.Equals(key))
						? InternalCreate(s, t, item, r, false)
						: InternalUpdate(s, t, key, item, r, false);
				}
				try
				{
					s.DbContext.SaveChanges();
					t.Commit();
				}
				catch (Exception e)
				{
					r.Add(new DataValidationResult<TKey>
					{
						ResultType = DataValidationResultType.Error,
						Message = e.InnerException != null ? e.InnerException.Message : e.Message
					});
				}
				return r;
			},
										false);
			return result;
		}

		public async virtual Task<DataValidationResults<TKey>> StoreAsync(params TModel[] items)
		{
			return await Task.FromResult(Store(items));
		}

		public virtual DataValidationResults<TKey> Delete(params TKey[] ids)
		{
			if (ids == null)
				throw new ArgumentNullException(nameof(ids));


			var result = TransactionalExec((s, t) =>
			{
				DataValidationResults<TKey> r = new DataValidationResults<TKey>();

				foreach (var id in ids)
				{
					r = InternalDelete(s, t, id, r);
				}
				try
				{
					s.DbContext.SaveChanges();
					t.Commit();
				}
				catch (Exception e)
				{
					r.Add(new DataValidationResult<TKey>
					{
						ResultType = DataValidationResultType.Error,
						Message = e.InnerException != null ? e.InnerException.Message : e.Message
					});
				}
				return r;
			},
										false);
			return result;
		}

		public async virtual Task<DataValidationResults<TKey>> DeleteAsync(params TKey[] ids)
		{
			return await Task.FromResult(Delete(ids));
		}

		protected TKey EmptyKeyValue => default;

		protected virtual T TransactionalExec<T>(Func<EFDataStore<TEFContext, TKey, TModel, TDBModel>,
			IDbContextTransaction, T> work,
			bool autoCommit = true)
		{
			T result = default;
			using (var dbTrans = DbContext.Database.BeginTransaction())
			{
				result = work(this, dbTrans);
				if (autoCommit && DbContext.ChangeTracker.HasChanges())
				{
					DbContext.SaveChanges();
					dbTrans.Commit();
				}
			}
			return result;
		}

		protected virtual void TransactionalExec<T>(Action<EFDataStore<TEFContext, TKey, TModel, TDBModel>, IDbContextTransaction> work,
													bool autoCommit = true)
		{
			using (var dbTrans = DbContext.Database.BeginTransaction())
			{
				work(this, dbTrans);
				if (autoCommit && DbContext.ChangeTracker.HasChanges())
				{
					DbContext.SaveChanges();
					dbTrans.Commit();
				}
			}
		}

		protected virtual DataValidationResults<TKey> InternalCreate(EFDataStore<TEFContext, TKey, TModel, TDBModel> store,
																	IDbContextTransaction trans,
																	TModel item,
																	DataValidationResults<TKey> results,
																	bool continueOnError = false)
		{
			var canInsert = Validator?.Inserting(item, results);
			results.Add(canInsert);
			if (canInsert.ResultType == DataValidationResultType.Error && !continueOnError)
				return results;

			var newItem = new TDBModel();
			Mapper.Map(item, newItem);
			DbContext.Set<TDBModel>().Add(newItem);
			DbContext.SaveChanges();

			var hasInserted = Validator?.Inserted(default, item, newItem, results);
			results.Add(hasInserted);
			return results;
		}

		protected virtual DataValidationResults<TKey> InternalUpdate(EFDataStore<TEFContext, TKey, TModel, TDBModel> store,
																	IDbContextTransaction trans,
																	TKey key,
																	TModel item,
																	DataValidationResults<TKey> results,
																	bool continueOnError = false)
		{
			var canUpdate = Validator?.Updating(key, item, results);
			results.Add(canUpdate);
			if (canUpdate.ResultType == DataValidationResultType.Error && !continueOnError)
				return results;

			var dbModel = EFGetByKey(key);
			if (dbModel == null)
			{
				results.Add(DataValidationResultType.Error,
							key,
							"KeyField",
							$"Unable to locate {typeof(TDBModel).Name}({key}) in datastore",
							0);
			}
			else
			{
				Mapper.Map(item, dbModel);
				DbContext.Entry(dbModel).State = EntityState.Modified;
				DbContext.SaveChanges();

				var hasUpdated = Validator?.Inserted(key, item, dbModel, results);
				results.Add(hasUpdated);
			}
			return results;
		}

		protected virtual DataValidationResults<TKey> InternalDelete(EFDataStore<TEFContext, TKey, TModel, TDBModel> store,
																	IDbContextTransaction trans,
																	TKey key,
																	DataValidationResults<TKey> results,
																	bool continueOnError = false)
		{
			var canDelete = Validator?.Deleting(key, results);
			results.Add(canDelete);
			if (canDelete.ResultType == DataValidationResultType.Error && !continueOnError)
				return results;

			var dbModel = EFGetByKey(key);
			if (dbModel == null)
			{
				results.Add(DataValidationResultType.Error,
							key,
							"KeyField",
							$"Unable to locate {typeof(TDBModel).Name}({key}) in datastore",
							0);
			}
			else
			{
				DbContext.Entry(dbModel).State = EntityState.Deleted;
				DbContext.SaveChanges();

				var hasDeleted = Validator?.Deleted(key, dbModel, results);
				results.Add(hasDeleted);
			}
			return results;
		}
	}
}
