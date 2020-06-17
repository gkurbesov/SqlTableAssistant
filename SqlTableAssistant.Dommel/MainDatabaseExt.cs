using Dommel;
using SqlTableAssistant.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace SqlTableAssistant.Dommel
{
    public static class MainDatabaseExt
    {
        public static T Get<T>(this IMainDatabase db, int index) where T : class
        {
            try
            {
                if (db.IsConnected() && db.GetNewConnection(true) is IDbConnection connection)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        var result = connection.Get<T>(index);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                db.CallError(ex.Message, ex.StackTrace);
            }
            return null;
        }

        public static bool TryGet<T>(this IMainDatabase db, int index, out T value) where T : class
        {
            value = null;
            try
            {
                if (db.IsConnected() && db.GetNewConnection(true) is IDbConnection connection)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        var result = connection.Get<T>(index);
                        value = result;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                db.CallError(ex.Message, ex.StackTrace);
            }
            return false;
        }

        public static IEnumerable<T> GetAll<T>(this IMainDatabase db) where T : class
        {
            try
            {
                if (db.IsConnected() && db.GetNewConnection(true) is IDbConnection connection)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        var list = connection.GetAll<T>().ToArray();
                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                db.CallError(ex.Message, ex.StackTrace);
            }
            return Enumerable.Empty<T>();
        }

        public static IEnumerable<T> Select<T>(this IMainDatabase db, Expression<Func<T, bool>> predicate) where T : class
        {
            try
            {
                if (db.IsConnected() && db.GetNewConnection(true) is IDbConnection connection)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        var list = connection.Select<T>(predicate);
                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                db.CallError(ex.Message, ex.StackTrace);
            }
            return Enumerable.Empty<T>();
        }

        public static bool TrySelect<T>(this IMainDatabase db, Expression<Func<T, bool>> predicate, out IEnumerable<T> collection) where T : class
        {
            try
            {
                if (db.IsConnected() && db.GetNewConnection(true) is IDbConnection connection)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        collection = connection.Select<T>(predicate);
                        return collection.Count() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                db.CallError(ex.Message, ex.StackTrace);
            }
            collection = Enumerable.Empty<T>();
            return false;
        }

        public static long InsertAsIndex<T>(this IMainDatabase db, T value) where T : class
        {
            try
            {
                if (db.IsConnected() && db.GetNewConnection(true) is IDbConnection connection)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        var insert_ind = connection.Insert(value);
                        if (insert_ind != null)
                            return (long)insert_ind;
                        else
                            return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                db.CallError(ex.Message, ex.StackTrace);
            }
            return -1;
        }

        public static T Insert<T>(this IMainDatabase db, T value) where T : class
        {
            try
            {
                if (db.IsConnected() && db.GetNewConnection(true) is IDbConnection connection)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        var insert_ind = connection.Insert(value);
                        if (insert_ind != null)
                        {
                            return db.Get<T>(Convert.ToInt32(insert_ind));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                db.CallError(ex.Message, ex.StackTrace);
            }
            return null;
        }
        public static bool TryInsert<T>(this IMainDatabase db, T data, out T value) where T : class
        {
            value = null;
            try
            {
                if (db.IsConnected() && db.GetNewConnection(true) is IDbConnection connection)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        var insert_ind = db.Insert(data);
                        if (insert_ind != null)
                        {
                            value = db.Get<T>(Convert.ToInt32(insert_ind));
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                db.CallError(ex.Message, ex.StackTrace);
            }
            return false;
        }
        public static bool Update<T>(this IMainDatabase db, T value) where T : class
        {
            try
            {
                if (db.IsConnected() && db.GetNewConnection(true) is IDbConnection connection)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        return connection.Update(value);
                    }
                }
            }
            catch (Exception ex)
            {
                db.CallError(ex.Message, ex.StackTrace);
            }
            return false;
        }
        public static bool Delete<T>(this IMainDatabase db, Expression<Func<T, bool>> predicate) where T : class
        {
            try
            {
                if (db.IsConnected() && db.GetNewConnection(true) is IDbConnection connection)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.DeleteMultiple(predicate);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                db.CallError(ex.Message, ex.StackTrace);
            }
            return false;
        }
    }
}
