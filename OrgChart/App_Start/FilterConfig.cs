// <copyright file="FilterConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace OrgChart
{
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// FilterConfig contains
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Registers filters
        /// </summary>
        /// <param name="filters">filters errors</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}