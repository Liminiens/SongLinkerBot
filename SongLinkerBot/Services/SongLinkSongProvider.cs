using System;
using System.Net;
using System.Net.Http;
using System.Web;
using HtmlAgilityPack;
using SongLinkerBot.Entities;

namespace SongLinkerBot.Services
{
	public class SongLinkSongProvider
	{
		public SongMetaData GetSongMetaData(string link)
		{
			var res = new SongMetaData();
			
			var newUrl = $"https://song.link/{link}";
			
			using (HttpClient client = new HttpClient())
			{
				try	
				{
					var response = client.GetAsync(newUrl).GetAwaiter().GetResult();
					response.EnsureSuccessStatusCode();
					var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
					
					var doc = new HtmlDocument();
					doc.LoadHtml(responseBody);

					res.ImageUrl = doc.DocumentNode
						.SelectSingleNode("//img")?
						.Attributes["src"]?.Value;

					// if (res.ImageUrl == null)
					// 	return null;
					//
					if (res.ImageUrl == null)
					{
						var style = HttpUtility.HtmlDecode(doc.DocumentNode
							.SelectSingleNode("//div[@class='ytp-cued-thumbnail-overlay-image']")?
							.Attributes["style"]?.Value);
						var start = "background-image: url(\"";
						var end = "\");";
						style.TrimStart(start.ToCharArray());
						style.TrimEnd(end.ToCharArray());
						res.ImageUrl = style;
					}
					
					res.Title = HttpUtility.HtmlDecode(doc.DocumentNode
						.SelectSingleNode("//div[@class='css-dduo7c']").InnerText);
					res.Artist = HttpUtility.HtmlDecode(doc.DocumentNode
						.SelectSingleNode("//div[@class='css-fd8fdn']").InnerText);

					res.Link = response.RequestMessage.RequestUri.ToString();
				}  
				catch(HttpRequestException e)
				{
					Console.WriteLine("\nException Caught!");	
					Console.WriteLine("Message :{0} ",e.Message);
					return null;
				}
			}

			return res;
		}
	}
}