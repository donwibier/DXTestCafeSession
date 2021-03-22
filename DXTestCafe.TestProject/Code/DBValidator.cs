using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXTestCafe.TestProject.Code
{
	public class DataValidator<TEFContext, TKey, TModel, TDBModel>
		where TEFContext : DbContext
		where TKey : IEquatable<TKey>
		where TModel : class
		where TDBModel : class
	{
		protected DataValidationResult<TKey> CreateDefaultResult(TKey id, DataValidationEventType eventType)
		{
			return new DataValidationResult<TKey> { EventType = eventType, ResultType = DataValidationResultType.Success, ID = id };
		}

		public virtual DataValidationResult<TKey> Deleting(TKey id,
														DataValidationResults<TKey> validationResults,
														params object[] args)
		{
			var result = CreateDefaultResult(id, DataValidationEventType.Deleting);
			validationResults.Add(result);
			return result;
		}
		public virtual DataValidationResult<TKey> Inserting(TModel model, DataValidationResults<TKey> validationResults)
		{
			var result = CreateDefaultResult(default, DataValidationEventType.Inserting);
			validationResults.Add(result);
			return result;
		}
		public virtual DataValidationResult<TKey> Updating(TKey id, TModel model, DataValidationResults<TKey> validationResults)
		{
			var result = CreateDefaultResult(id, DataValidationEventType.Updating);
			validationResults.Add(result);
			return result;
		}

		public virtual DataValidationResult<TKey> Deleted(TKey id, TDBModel dbModel, DataValidationResults<TKey> validationResults)
		{
			var result = CreateDefaultResult(id, DataValidationEventType.Deleted);
			validationResults.Add(result);
			return result;
		}
		public virtual DataValidationResult<TKey> Inserted(TKey id, TModel model, TDBModel dbModel, DataValidationResults<TKey> validationResults)
		{
			var result = CreateDefaultResult(id, DataValidationEventType.Inserted);
			validationResults.Add(result);
			return result;
		}
		public virtual DataValidationResult<TKey> Updated(TKey id, TModel model, TDBModel dbModel, DataValidationResults<TKey> validationResults)
		{
			var result = CreateDefaultResult(id, DataValidationEventType.Updated);
			validationResults.Add(result);
			return result;
		}
	}

	public enum DataValidationResultType
	{
		Success,
		Warning,
		Error
	}
	public enum DataValidationEventType
	{
		Inserting,
		Inserted,
		Updating,
		Updated,
		Deleting,
		Deleted,
		Custom
	}
	public class DataValidationResult<TKey>
		where TKey : IEquatable<TKey>
	{
		public DataValidationResult()
		{
		}
		public DataValidationResult(DataValidationResultType resultType, TKey id, string fieldName, string message, int code)
		{

			ResultType = resultType;
			ID = id;
			FieldName = fieldName;
			Message = message;
			Code = code;

		}
		public DataValidationEventType EventType { get; set; }
		public DataValidationResultType ResultType { get; set; }
		public string FieldName { get; set; }
		public string Message { get; set; }
		public int Code { get; set; }
		public TKey ID { get; set; }

	}

	public class DataValidationResults<TKey>
		where TKey : IEquatable<TKey>
	{
		private readonly List<DataValidationResult<TKey>> errors = new List<DataValidationResult<TKey>>();

		public IEnumerable<DataValidationResult<TKey>> Results { get => errors; }
		public void Add(DataValidationResult<TKey> error)
		{
			errors.Add(error);

		}
		public void Add(DataValidationResultType resultType, TKey id, string fieldName, string message, int code)
		{
			errors.Add(new DataValidationResult<TKey>(resultType, id, fieldName, message, code));
		}

		public string[] Messages(params DataValidationResultType[] resultsTypes)
		{
			string[] results = new string[] { };
			if (errors == null || errors.Count() == 0)
				return results;

			if (resultsTypes == null || resultsTypes.Length == 0)
				results = errors.Select(r => r.Message).ToArray();
			else
				results = errors.Where(r => resultsTypes.Contains(r.ResultType))
					.Select(r => r.Message)
					.ToArray();
			return results;
		}

		public bool Success { get => (errors.Count == 0) || (errors.Count == errors.FindAll(x => x.ResultType == DataValidationResultType.Success).Count); }
	}

	public class DataValidationException<TKey> : Exception
		where TKey : IEquatable<TKey>
	{
		public DataValidationException(DataValidationResults<TKey> validationResults)
			: base()
		{
			ValidationResults = validationResults;
		}

		public DataValidationResults<TKey> ValidationResults { get; protected set; }
		public override IDictionary Data
		{
			get
			{
				var results = ValidationResults.Results.ToDictionary(r => r.ID, r => r.Message);
				return results;
			}
		}
		public override string Message
		{
			get { return string.Join("\n", ValidationResults.Messages(DataValidationResultType.Error, DataValidationResultType.Warning)); }
		}
	}
}
