using System;
using Android.App;
using Android.Widget;
using Android.OS;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using BaseLibrary;
using static Android.App.ActionBar;

namespace MobileLibrary
{
  [Activity(Label = "Library 1.6", MainLauncher = true, Icon = "@drawable/icon")]
  public class MainActivity : Activity
  {
    #region Properties
    Library MyLib { set; get; }
    WebAccess WA { set; get; } = new WebAccess();
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

    #region C-tor
    public MainActivity()
    {
    }
    #endregion

    #region Methods
    private void LoadData()
    {
      if(result.IsCompleted)
        MyLib = new Library(result.Result, true);
    }

    private void ShowData<T>(List<T> list)
    {
      //foreach (Enum group in Enum.GetValues(typeof(Common.BookGroup)))
      //{
      //  LinearLayout ly = new LinearLayout(this);
      //  ly.Id = Convert.ToInt32(group);
      //  ly.SetMinimumHeight(LayoutParams.WrapContent);
      //  ly.SetMinimumWidth(LayoutParams.WrapContent);
      //  this.ScrollView.AddView(ly);
      //}
      

      foreach (var item in MyLib.AllBooks)
        ShowBook(Convert.ToInt32(item.Groupe), item);
    }

    private void ShowBook(int id, Book item)
    {
      TextView row;
      Book b = item as Book;

      row = new TextView(this);
      row.Click += Row_Click;
      row.Id = Convert.ToInt32(b.ID);
      row.Text = string.Format("{0}, {1} {2} - {3}", b.AuthorLastName, b.AuthorFirstName, b.AuthorMiddleName, b.CzechName);

      LinearLayout v = (LinearLayout)ScrollableLayout.FindViewById(id);
      ScrollableLayout.AddView(row);
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
      Button.Click += delegate { btnClick(); };
      Button.SetWidth(50);

      Label.Text = MyCache.GetDate();
      MyLib = new Library(MyCache.GetBooksList());
      ShowData(MyLib.AllBooks);      
    }

    private void Row_Click(object sender, EventArgs e)
    {
      string id = (sender as TextView).Id.ToString();
      Book b = MyLib.AllBooks.Where(i => i.ID.Equals(id)).ToList().FirstOrDefault();

      string tooltip = string.Format("[{0}] {1}, {2} {3} - {4} [{5}]",
        b.ID,
        b.AuthorLastName,
        b.AuthorFirstName,
        b.AuthorMiddleName,
        b.CzechName,
        b.OriginalName);

      Toast.MakeText(this, tooltip, ToastLength.Long).Show();
    }

    private void btnClick()
    {
      if (CallWeb())
      {
        Label.Text = string.Empty;

        LoadData();
        
        string info = $"Knih: {MyLib.NumberOfBooks.ToString()} [n:{MyLib.NumberOfNewBooks}, a:{MyLib.NumberOfAnticBooks}]\r\n";
        info += $"Přečteno: {MyLib.NumberOfReaded} [{MyLib.NumberOfReadedPercentage}%]\r\n";
        info += $"Chci: {MyLib.NumberOfWantedBooks}\r\n";
        info += $"Nakladatelé: [{MyLib.BestPublishers.Substring(0, MyLib.BestPublishers.Length - 2)}]\r\n";
        Label.Text = info;

        ShowData(MyLib.AllBooks);
        
        MyCache.SetDate(info);
        MyCache.SetBooksList(MyLib.AllBooks);
        Toast.MakeText(this, "Stored to cache", ToastLength.Short).Show();
      }
      else
        Toast.MakeText(this, "Cannot connect to DB...", ToastLength.Short).Show();
    }
    #endregion
  }
}

