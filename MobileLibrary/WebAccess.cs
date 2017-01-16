using System.Threading.Tasks;
using System.Net.Http;

namespace MobileLibrary
{
  public class WebAccess
  {
    public async Task<string> Read()
    {
      return await Task.Run( () => DownloadPageAsync());
    }

    public async Task<string> DownloadPageAsync()
    {
      string page = "http://hromek.aspone.cz/library/storage.xml";
      string result = string.Empty;

      try
      {
        using (HttpClient client = new HttpClient())
        {
          using (HttpResponseMessage response = /*await*/ client.GetAsync(page).Result)
          {
            using (HttpContent content = response.Content)
            {
              result = await content.ReadAsStringAsync();
            }
          }
        }
      }
      catch (System.Net.WebException ex)
      {
        throw new System.ArgumentException("err");
      }

      return result;
    }
  }
}