namespace BioTonFMS.Infrastructure.Persistence.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// �������� ��������� Eager Loading ��� LINQ-�������.
    /// </summary>
    /// <typeparam name="TQueried">��� ������������� ��������.</typeparam>
    /// <typeparam name="TFetch">��� �������� �������������� ��������.</typeparam>
    public interface IFetchRequestContext<TQueried, TFetch> : IFetchRequest<TQueried>
    {
        /// <summary>
        /// �������� ��������� ������������� �������� ��� Eager Loading � ������� ����� ��������.
        /// </summary>
        /// <typeparam name="TRelated">��� �������� �������������� ��������.</typeparam>
        /// <param name="selector">��������� ��������� �������������� ��������.</param>
        /// <returns>����� ��������.</returns>
        IFetchRequestContext<TQueried, TRelated> ThenFetch<TRelated>(
            Expression<Func<TFetch, TRelated>> selector);
        /*
        /// <summary>
        /// �������� ������������ ������������� �������� ��� Eager Loading � ������� ����� ��������.
        /// </summary>
        /// <typeparam name="TRelated">��� �������� �������������� ��������.</typeparam>
        /// <param name="selector">��������� ��������� �������������� ��������.</param>
        /// <returns>����� ��������.</returns>
        IFetchRequestContext<TQueried, TRelated> ThenFetchMany<TRelated>(
            Expression<Func<TFetch, IEnumerable<TRelated>>> selector);*/
    }
}