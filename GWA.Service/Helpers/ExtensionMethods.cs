using GWA.Domaine.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace GWA.Service.Helpers
{
    public static class ExtensionMethods
    {
       

        //public static IQueryable<T> WhereLike<T>(this IQueryable<T> source, string propertyName, string pattern)
        //{
        //    if (null == source) throw new ArgumentNullException("source");
        //    if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");


        //    var a = Expression.Parameter(typeof(T), "a");
        //    var prop = Expression.Property(a, propertyName);
        //    var body = Expression.Call(typeof(SqlMethods), "Like", null, prop, Expression.Constant(pattern));
        //    var fn = Expression.Lambda<Func<T, bool>>(body, a);


        //    return source.Where(fn);
        //}

        public static IQueryable<T> Like<T>(this IQueryable<T> source, string propertyName, string keyword)
        {
            var type = typeof(T);
            var property = type.GetProperty(propertyName);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var constant = Expression.Constant("%" + keyword + "%");

            var like = typeof(SqlMethods).GetMethod("Like",
                       new Type[] { typeof(string), typeof(string) });
            MethodCallExpression methodExp =
                  Expression.Call(null, like, propertyAccess, constant);
            Expression<Func<T, bool>> lambda =
                  Expression.Lambda<Func<T, bool>>(methodExp, parameter);
            return source.Where(lambda);
        }



    }
}