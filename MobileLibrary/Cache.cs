using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using GoogleGson;
using Java.Lang.Reflect;
using System.IO;

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
      list.ForEach( i => ee.Add(string.Format("{0}, {1} {2} - {3}", i.AuthorLastName, i.AuthorFirstName, i.AuthorMiddleName, i.CzechName)));

      var items = BooksCache.Edit();
      items.PutStringSet("data", ee);
      items.Commit();
    }

    public List<string> GetBooksList()
    {
      List<string> empty = new List<string>();
      var ee = BooksCache.GetStringSet("data", new List<String>());
      empty.AddRange(ee);
      return empty;
    }
    #endregion
  }
}