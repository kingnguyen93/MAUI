using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text.Json.Serialization;

namespace RocketPDF.EntityFrameworkCore.Extensions
{
    public static class DbContextExtension
    {
        public static DbCommand LoadStoredProcedure(this DbContext context, string storedProcName)
        {
            var cmd = context.Database.GetDbConnection().CreateCommand();
            if (context.Database.CurrentTransaction != null)
                cmd.Transaction = context.Database.CurrentTransaction.GetDbTransaction();
            cmd.CommandText = storedProcName;
            cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }

        public static DbCommand WithSqlParams(this DbCommand cmd, params (string, object)[] nameValueParamPairs)
        {
            foreach (var pair in nameValueParamPairs)
            {
                var param = cmd.CreateParameter();
                param.ParameterName = pair.Item1;
                param.Value = pair.Item2 ?? DBNull.Value;
                cmd.Parameters.Add(param);
            }
            return cmd;
        }

        public static DbCommand WithSqlParams(this DbCommand cmd, params (string, object, DbType?)[] nameValueParamPairs)
        {
            foreach (var pair in nameValueParamPairs)
            {
                var param = cmd.CreateParameter();
                param.ParameterName = pair.Item1;
                param.Value = pair.Item2 ?? DBNull.Value;
                if (pair.Item3.HasValue)
                {
                    param.DbType = pair.Item3.Value;
                }
                cmd.Parameters.Add(param);
            }
            return cmd;
        }

        public static async Task<int> ExecuteStoredProcedureNonQueryAsync(this DbCommand command, bool closeConnection = false)
        {
            using (command)
            {
                if (command.Connection.State == ConnectionState.Closed)
                    await command.Connection.OpenAsync();
                try
                {
                    return await command.ExecuteNonQueryAsync();
                }
                finally
                {
                    if (closeConnection)
                        await command.Connection.CloseAsync();
                }
            }
        }

        public static async Task<T> ExecuteStoredProcedureScalarAsync<T>(this DbCommand command, bool closeConnection = false)
        {
            using (command)
            {
                if (command.Connection.State == ConnectionState.Closed)
                    await command.Connection.OpenAsync();
                try
                {
                    var result = await command.ExecuteScalarAsync();
                    return (T)Convert.ChangeType(result, typeof(T));
                }
                catch (InvalidCastException)
                {
                    return default;
                }
                finally
                {
                    if (closeConnection)
                        await command.Connection.CloseAsync();
                }
            }
        }

        public static async Task<T> ExecuteStoredProcedureAsync<T>(this DbCommand command, bool closeConnection = false) where T : class
        {
            using (command)
            {
                if (command.Connection.State == ConnectionState.Closed)
                    await command.Connection.OpenAsync();
                try
                {
                    using var reader = await command.ExecuteReaderAsync();
                    return reader.MapToList<T>()?.FirstOrDefault();
                }
                finally
                {
                    if (closeConnection)
                        await command.Connection.CloseAsync();
                }
            }
        }

        public static async Task<ICollection<T>> ExecuteStoredProcedureListAsync<T>(this DbCommand command, bool closeConnection = false) where T : class
        {
            using (command)
            {
                if (command.Connection.State == ConnectionState.Closed)
                    await command.Connection.OpenAsync();
                try
                {
                    using var reader = await command.ExecuteReaderAsync();
                    return reader.MapToList<T>();
                }
                finally
                {
                    if (closeConnection)
                        await command.Connection.CloseAsync();
                }
            }
        }

        private static ICollection<T> MapToList<T>(this DbDataReader dr)
        {
            var objList = new List<T>();
            var props = typeof(T).GetRuntimeProperties();

            var colMapping = dr.GetColumnSchema()
                .Where(x => props.Any(y => (y.GetCustomAttribute<ColumnAttribute>()?.Name ?? y.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? y.Name) == x.ColumnName))
                .ToDictionary(key => key.ColumnName);

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    T obj = Activator.CreateInstance<T>();
                    foreach (var prop in props)
                    {
                        var val = dr.GetValue(colMapping[prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? prop.Name].ColumnOrdinal.Value);
                        prop.SetValue(obj, val == DBNull.Value ? default : val);
                    }
                    objList.Add(obj);
                }
            }
            return objList;
        }
    }
}