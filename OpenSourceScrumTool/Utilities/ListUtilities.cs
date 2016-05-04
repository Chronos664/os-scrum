using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenSourceScrumTool.Models;

namespace OpenSourceScrumTool.Utilities
{
    public static class ListUtilities
    {
        public static IEnumerable<T> ToDTO<T>(this IEnumerable<IModelContent> collection)
        {
            List<T> DTOList = new List<T>();
            foreach (IModelContent item in collection)
            {
                DTOList.Add((T)item.ToDTO());
            }
            return DTOList;
        }

        public static IEnumerable<T> GetDetails<T>(this IEnumerable<IModelContent> collection)
        {
            List<T> DetailsDTOList = new List<T>();
            foreach (IModelContent item in collection)
            {
                DetailsDTOList.Add((T)item.GetDetails());
            }
            return DetailsDTOList;
        }
    }
}