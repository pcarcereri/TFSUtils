﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildChangesetRetriever
{
    public static class Extensions
    {
        /// <summary>
        /// Returns the preceeding element in the enumerable
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        public static TSource GetPrecedingElement<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource> searchElement)
        {
            TSource currentElement = searchElement();

            TSource preceedingElement = default(TSource);
            foreach (var element in source)
            {
                if(element.Equals(currentElement))
                {
                    return preceedingElement;
                }

                preceedingElement = element;
            }

            return default(TSource);
        }

        /// <summary>
        /// Custom implementation of string.Join that inserts a placeholder in the case value is empty 
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="placeholder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String JoinWithPlaceholder(this string str, string separator, string placeholder, params String[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if(!value.Any())
            {
                return placeholder;
            }

            return string.Join(separator, value);
        }

    }
}
