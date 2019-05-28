using System;
using Microsoft.Extensions.Configuration;

namespace SongLinkerBot.Entities
{
	public static class Vault
	{
		public static string Token;
		public static string ProxyHost;
		public static int ProxyPort;
		public static string ProxyUserName;
		public static string ProxyPass;
		public static long ChannelId;

		public static void Load(IConfiguration config)
		{
			Token = config["Token"];
			ProxyHost = config["ProxyHost"];
			ProxyPort = int.Parse(config["ProxyPort"]);
			ProxyUserName = config["ProxyUserName"];
			ProxyPass = config["ProxyPass"];
			ChannelId = long.Parse(config["ChannelId"]);
		}
	}
}