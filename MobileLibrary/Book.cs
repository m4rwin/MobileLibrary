using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Xml;

namespace MobileLibrary
{
  public class Book
  {
    #region Properties
    public string ID { get; set; }
    public string OriginalName { get; set; }
    public string CzechName { get; set; }
    public string AuthorFirstName { get; set; }
    public string AuthorMiddleName { get; set; }
    public string AuthorLastName { get; set; }
    public string OriginalPublicationDate { get; private set; }
    public string MyPublicationDate { get; private set; }
    public string Publish3r { get; private set; }
    public string BookType { get; private set; }
    public List<string> Genre { get; private set; } = new List<string>();
    //public Common.BookGroup Groupe { get; private set; }
    public bool Readed { get; private set; }
    public string DateRead { get; private set; }
    public string Ratings { get; private set; }
    public string RatingsValue { get; private set; }
    public string GenreAll
    {
      private set { }
      get
      {
        return string.Join(",", Genre);
      }
    }
    #endregion

    public Book(XmlNode lNode)
    {
      ID = lNode.Attributes["id"].Value;

      foreach (XmlElement item in lNode.ChildNodes)
      {
        switch (item.LocalName)
        {
          case "name":
            ReadName(item); break;

          case "author":
            ReadAuthor(item); break;

          case "publication_date":
            ReadPublicationDate(item); break;

          case "publisher":
            Publish3r = item.InnerText; break;

          case "type":
            BookType = item.InnerText; break;

          case "genre":
            ReadGenre(item); break;

          //case "group":
          //  Groupe = ReadGroup(item.InnerText); break;

          case "readed":
            Readed = (item.InnerText.Equals("ano")) ? true : false; break;

          case "date_read":
            DateRead = item.InnerText; break;

          case "ratings":
            Ratings = item.InnerText; break;

          case "ratings_value":
            RatingsValue = item.InnerText; break;

          default:
            break;
        }
      }
    }

    public Book()
    {
    }

    //private Common.BookGroup ReadGroup(string name)
    //{
    //  Common.BookGroup g = Common.BookGroup.Beletrie;
    //  foreach (string item in Enum.GetNames(typeof(Common.BookGroup)))
    //    if (item.Equals(name))
    //      Enum.TryParse(item, out g);

    //  return g;
    //}

    private void ReadAuthor(XmlElement item)
    {
      foreach (XmlNode child in item.ChildNodes)
      {
        if (child.LocalName.Equals("first_name")) AuthorFirstName = child.InnerText;
        else if (child.LocalName.Equals("middle_name")) AuthorMiddleName = child.InnerText;
        else if (child.LocalName.Equals("last_name")) AuthorLastName = child.InnerText;
      }
    }

    private void ReadName(XmlElement item)
    {
      OriginalName = item.GetAttribute("original");
      CzechName = item.GetAttribute("czech");
    }

    private void ReadPublicationDate(XmlElement item)
    {
      OriginalPublicationDate = item.GetAttribute("original");
      MyPublicationDate = item.GetAttribute("my");
    }

    private void ReadGenre(XmlElement item)
    {
      var v = item.ChildNodes;
      foreach (XmlElement a in v)
        Genre.Add(a.InnerText);
    }
  }
}