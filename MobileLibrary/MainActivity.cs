using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Xml;
using System.Net;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileLibrary
{
  [Activity(Label = "Library 1.4", MainLauncher = true, Icon = "@drawable/icon")]
  public class MainActivity : Activity
  {
    #region Properties
    List<Book> BooksList { set; get; } = new List<Book>();
    WebAccess WA { set; get; } = new WebAccess();
    XmlDocument XDocument { set; get; } = new XmlDocument();
    Task<string> result { set; get; } = null;
    Button Button { set; get; }
    TextView View { set; get; }
    TextView Label { set; get; }
    Cache MyCache { set; get; } = Cache.GetInstance();
    #endregion

    #region Methods
    private void LoadData()
    {
      BooksList.Clear();
      if (result.IsCompleted)
        XDocument.LoadXml(result.Result);
      XmlNode lRoot = XDocument.DocumentElement;

      foreach (XmlNode lNode in lRoot.ChildNodes)
        if (lNode.Name.Equals("book"))
          BooksList.Add(new Book(lNode));
    }

    private bool CallWeb()
    {
      try
      {
        result = WA.DownloadPageAsync();

        if (result.Status == TaskStatus.Faulted)
          return false;

        result.Wait();
      }
      catch (WebException ex)
      {
        return false;
      }
      return true;
    }
    #endregion

    #region Events
    protected override void OnCreate(Bundle bundle)
    {
      base.OnCreate(bundle);

      // Set our view from the "main" layout resource
      SetContentView(Resource.Layout.Main);

      // Get our button from the layout resource,
      // and attach an event to it
      Button = FindViewById<Button>(Resource.Id.MyButton);
      View = FindViewById<TextView>(Resource.Id.MyView);
      Label = FindViewById<TextView>(Resource.Id.MyLabel);
      Button.Click += delegate { btnClick(BooksList, View); };
      Button.SetWidth(50);

      View.MovementMethod = new Android.Text.Method.ScrollingMovementMethod(); // because of scrolling

      View.Text = string.Empty;
      Label.Text = MyCache.GetDate();
      List<string> books = MyCache.GetBooksList();
      books.Sort();
      books.ForEach(i => View.Text += i + "\r\n");
    }

    private void btnClick(List<Book> list, TextView view)
    {
      if (CallWeb())
      {
        Label.Text = string.Empty;
        view.Text = string.Empty;

        LoadData();

        string info = $"Books: {list.Count}, Last update: {DateTime.Now.ToString()}";
        Label.Text = info;
        list.Sort((x, y) => string.Compare(x.AuthorLastName, y.AuthorLastName));
        list.ForEach(i => view.Text += i.AuthorLastName + ", " + i.AuthorFirstName + " " + i.AuthorMiddleName + " - " + i.CzechName + "\r\n");

        MyCache.SetDate(info);
        MyCache.SetBooksList(list);
        Android.Widget.Toast.MakeText(this, "Stored to cache", ToastLength.Short).Show();
      }
      else
        Android.Widget.Toast.MakeText(this, "Cannot connect to DB...", ToastLength.Short).Show();
    }
    #endregion
  }
}

