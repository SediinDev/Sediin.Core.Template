using System.Reflection;

namespace Sediin.Core.Mvc.Helpers.PagingHelpers
{
    public interface PagedResultViewModel<TItem, TFiltri>
    {
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public TFiltri? Filtri { get; set; }
        public IEnumerable<TItem> Result { get; set; }
    }

    public static class PagingHelper
    {
        public static T GetModelWithPaging<T, TItem>(int? page, IEnumerable<TItem>? query, object? filtri, int pageSize)
            where T : new()
        {
            int currentPage = page.GetValueOrDefault(1);
            int totalRecords = query?.Count() ?? 0;

            int skip = (currentPage - 1) * pageSize;

            List<TItem> pagedData = query?.Skip(skip).Take(pageSize).ToList() ?? new List<TItem>();

            T resultModel = new T();

            void SetProperty(string propName, object? value)
            {
                try
                {
                    typeof(T).GetProperty(propName, BindingFlags.Public | BindingFlags.Instance)
                             ?.SetValue(resultModel, value);
                }
                catch
                {
                    // Silently catch or add logging
                }
            }

            SetProperty("TotalRecords", totalRecords);
            SetProperty("Result", pagedData);
            SetProperty("CurrentPage", currentPage);
            SetProperty("PageSize", pageSize);
            SetProperty("Filtri", filtri);

            return resultModel;
        }

        public static T GetModelWithPaging<T, TItem>(int? page, IEnumerable<TItem>? query, object? filtri, int totalRecords, int pageSize)
            where T : new()
        {
            int currentPage = page.GetValueOrDefault(1);

            T resultModel = new T();

            void SetProperty(string propName, object? value)
            {
                try
                {
                    typeof(T).GetProperty(propName, BindingFlags.Public | BindingFlags.Instance)
                             ?.SetValue(resultModel, value);
                }
                catch
                {
                    // Silently catch or add logging
                }
            }

            SetProperty("TotalRecords", totalRecords);
            SetProperty("Result", query);
            SetProperty("CurrentPage", currentPage);
            SetProperty("PageSize", pageSize);
            SetProperty("Filtri", filtri);

            return resultModel;
        }
    }

}
