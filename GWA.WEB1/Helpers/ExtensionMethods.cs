using GWA.Domaine.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace GWA.WEB1.Helpers
{
    public static class ExtensionMethods
    {
        public static IEnumerable<SelectListItem> ToSelectListItems(
              this IEnumerable<string> Noms)
        {
            return
                Noms.OrderBy(genre => Noms)
                      .Select(genre =>
                          new SelectListItem
                          {

                              Text = genre,
                              Value = genre
                          });
        }

        public static IEnumerable<SelectListItem> ToSelectListItems(
         this IEnumerable<Category> categories)
        {
            return
                categories.OrderBy(c => c.Name)
                      .Select(c =>
                          new SelectListItem
                          {
                              //     Selected = (prod.ProducteurId == selectedId),
                              Text = c.Name,
                              Value = c.Id.ToString()

                          });
        }


             public static IEnumerable<SelectListItem> ToSelectListItems(
         this IEnumerable<IdentityRole> categories)
        {
            return
                categories.OrderBy(c => c.Name)
                      .Select(c =>
                          new SelectListItem
                          {
                              //     Selected = (prod.ProducteurId == selectedId),
                              Text = c.Name,
                              Value = c.Id.ToString()

                          });
        }

        public static IQueryable<T> WhereLike<T>(this IQueryable<T> source, string propertyName, string pattern)
        {
            if (null == source) throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");


            var a = Expression.Parameter(typeof(T), "a");
            var prop = Expression.Property(a, propertyName);
            var body = Expression.Call(typeof(SqlMethods), "Like", null, prop, Expression.Constant(pattern));
            var fn = Expression.Lambda<Func<T, bool>>(body, a);


            return source.Where(fn);
        }

    }
}