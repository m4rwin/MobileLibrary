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
using System.Linq;

namespace MobileLibrary
{
  [Activity(Label = "Library 1.5", MainLauncher = true, Icon = "@drawable/icon")]
  public class MainActivity : Activity
  {
    #region Properties
    List<Book> BooksList { set; get; } = new List<Book>();
    WebAccess WA { set; get; } = new WebAccess();
    XmlDocument XDocument { set; get; } = new XmlDocument();
    Task<string> result { set; get; } = null;
    Cache MyCache { set; get; } = Cache.GetInstance();
    #endregion

    #region Controls
    LinearLayout MainLayout { set; get; }
    LinearLayout ScrollableLayout { set; get; }
    Button Button { set; get; }
    TextView Label { set; get; }
    ScrollView ScrollView { set; get; }
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

    private void ShowData<T>(List<T> list)
    {
      TextView row;
      foreach (var item in list)
      {
        Book b = item as Book;

        row = new TextView(this);
        row.Click += Row_Click;
        row.Id = Convert.ToInt32(b.ID);
        row.Text = string.Format("{0}, {1} {2} - {3}", b.AuthorLastName, b.AuthorFirstName, b.AuthorMiddleName, b.CzechName);
        
        ScrollableLayout.AddView(row);
      }
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
      Label = FindViewById<TextView>(Resource.Id.MyLabel);
      MainLayout = FindViewById<LinearLayout>(Resource.Id.MainLayout);
      ScrollableLayout = FindViewById<LinearLayout>(Resource.Id.ScrollableLayout);
      ScrollView = FindViewById<ScrollView>(Resource.Id.ScrollView);
      Button.Click += delegate { btnClick(BooksList); };
      Button.SetWidth(50);

      
      //row.MovementMethod = new Android.Text.Method.ScrollingMovementMethod(); // because of scrolling   

      Label.Text = MyCache.GetDate();
      BooksList = MyCache.GetBooksList();
      ShowData(BooksList);      
    }

    private void Row_Click(object sender, EventArgs e)
    {
      string id = (sender as TextView).Id.ToString();
      Book b = BooksList.Where(i => i.ID.Equals(id)).ToList().FirstOrDefault();

      string tooltip = string.Format("[{0}] {1}, {2} {3} - {4} [{5}]",
        b.ID,
        b.AuthorLastName,
        b.AuthorFirstName,
        b.AuthorMiddleName,
        b.CzechName,
        b.OriginalName);

      Toast.MakeText(this, tooltip, ToastLength.Long).Show();
    }

    private void btnClick(List<Book> list)
    {
      if (CallWeb())
      {
        Label.Text = string.Empty;

        LoadData();

        string info = $"Books: {list.Count}, Last update: {DateTime.Now.ToString()}";
        Label.Text = info;

        ShowData(list);
        
        MyCache.SetDate(info);
        MyCache.SetBooksList(list);
        Toast.MakeText(this, "Stored to cache", ToastLength.Short).Show();
      }
      else
        Toast.MakeText(this, "Cannot connect to DB...", ToastLength.Short).Show();
    }
    #endregion
  }
}

