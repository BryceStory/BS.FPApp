using System;
using System.Collections.Generic;

namespace FiiiPay.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public static class ListExtension
    {
        /// <summary>
        /// Move up the sepcified data list.
        /// </summary>
        /// <param name="list">The data list</param>
        /// <param name="index">The item index</param>
        /// <param name="step">The move up step</param>
        /// <typeparam name="T">The item Type</typeparam>
        /// <returns></returns>
        public static bool MoveUp<T>(this IList<T> list, int index, int step = 1)
        {
            return list.Move(index, index - step);
        }

        /// <summary>
        /// Move down the sepcified data list.
        /// </summary>
        /// <param name="list">The data list</param>
        /// <param name="index">The item index</param>
        /// <param name="step">The move down step</param>
        /// <typeparam name="T">The item Type</typeparam>
        /// <returns></returns>
        /// <returns></returns>
        public static bool MoveDown<T>(this IList<T> list, int index, int step = 1)
        {
            return list.Move(index, index + step);
        }

        /// <summary>
        /// Move top the sepcified data list.
        /// </summary>
        /// <param name="list">The data list</param>
        /// <param name="index">The item index</param>
        /// <typeparam name="T">The item Type</typeparam>
        /// <returns></returns>
        public static bool MoveTop<T>(this IList<T> list, int index)
        {
            return list.Move(index, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="match"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool MoveTop<T>(this IList<T> list, Predicate<T> match)
        {
            return list.Move(match, 0);
        }

        /// <summary>
        /// Move bottom the sepcified data list.
        /// </summary>
        /// <param name="list">The data list</param>
        /// <param name="index">The item index</param>
        /// <typeparam name="T">The item Type</typeparam>
        /// <returns></returns>
        public static bool MoveBottom<T>(this IList<T> list, int index)
        {
            return list.Move(index, list.Count - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="match"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool MoveBottom<T>(this IList<T> list, Predicate<T> match)
        {
            return list.Move(match, list.Count - 1);
        }

        /// <summary>
        ///  Move the sepcified data list.
        /// </summary>
        /// <param name="list">The data list</param>
        /// <param name="oldIndex">The item old index</param>
        /// <param name="newIndex">The item new index</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            if (oldIndex >= list.Count || oldIndex < 0)
                throw new IndexOutOfRangeException("the old index out of list range.");

            if (newIndex >= list.Count || newIndex < 0)
                throw new IndexOutOfRangeException("the new index out of list range.");

            if (oldIndex == newIndex)
                return true;


            try
            {
                T item = list[oldIndex];
                list.RemoveAt(oldIndex);
                list.Insert(newIndex, item);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Move the sepcified data list.
        /// <seealso cref="Move{T}(System.Collections.Generic.IList{T},int,int)"/>
        /// </summary>
        /// <param name="list"></param>
        /// <param name="match"></param>
        /// <param name="newIndex"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static bool Move<T>(this IList<T> list, Predicate<T> match, int newIndex)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            if (newIndex >= list.Count || newIndex < 0)
                throw new IndexOutOfRangeException("the new index out of list range.");

            for (int index = 0; index < list.Count; index++)
            {
                if (match(list[index]))
                {
                    T item = list[index];
                    list.RemoveAt(index);
                    list.Insert(newIndex, item);
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Exchange the specified data list.
        /// </summary>
        /// <param name="list">The dat list</param>
        /// <param name="firIndex">The first item</param>
        /// <param name="secIndex">The second item</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Exchange<T>(this IList<T> list, int firIndex, int secIndex)
        {
            if (firIndex >= list.Count || firIndex < 0)
                throw new IndexOutOfRangeException("the old index out of list range.");

            if (secIndex >= list.Count || secIndex < 0)
                throw new IndexOutOfRangeException("the new index out of list range.");

            if (firIndex == secIndex)
                return true;


            try
            {
                T item = list[firIndex];
                list[firIndex] = list[secIndex];
                list[secIndex] = item;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// Remove the specified item from list data.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool Remove<T>(this IList<T> list, Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            foreach (var item in list)
            {
                if (predicate(item))
                    return list.Remove(item);
            }

            return false;
        }
    }
}