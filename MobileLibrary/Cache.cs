using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using BaseLibrary;

namespace MobileLibrary
{
  public class Cache
  {
    #region Private
    private static readonly Cache _instance = new Cache();
    #endregion
    
    #region Properties
    public ISharedPreferences LastUpdate { private set; get; }
    public ISharedPreferences BooksCache { private set; get; }
    #endregion

    #region C-tor
    private Cache()
    {
      LastUpdate = Application.Context.GetSharedPreferences("LastUpdate", FileCreationMode.WorldWriteable);
      BooksCache = Application.Context.GetSharedPreferences("BooksCache", FileCreationMode.WorldWriteable);
    }
    #endregion

    #region Method
    public static Cache GetInstance() { return _instance; }

    public void SetDate(string value)
    {
      var item = LastUpdate.Edit();
      item.PutString("date", value);
      item.Commit();
    }

    public string GetDate()
    {
      return LastUpdate.GetString("date", $"Books: -1, Last update: {DateTime.MinValue.ToString()}");
    }

    public void SetBooksList(List<Book> list)
    {
      List<string> ee = new List<string>();
      list.ForEach(i => ee.Add(GetStringBook(i)));

      var items = BooksCache.Edit();
      items.PutStringSet("data", ee);
      items.Commit();
    }

    public List<Book> GetBooksList()
    {
      List<Book> empty = new List<Book>();
      var ee = BooksCache.GetStringSet("data", new List<String>());

      foreach (var item in ee)
        empty.Add(GetBook(item));

      empty.Sort((x, y) => string.Compare(x.AuthorLastName, y.AuthorLastName));
      return empty;
    }

    private string GetStringBook(Book b)
    {
      return string.Format("{0};{1};{2};{3};{4};{5}", b.ID, b.AuthorLastName, b.AuthorFirstName, b.AuthorMiddleName, b.CzechName, b.OriginalName);
    }

    private Book GetBook(string value)
    {
      string[] result = value.Split(';');

      if (result.Length == 5)
        return new Book();

      return new Book(result[0], result[1], result[2], result[3], result[4], result[5]);
    }
    #endregion
  }
}