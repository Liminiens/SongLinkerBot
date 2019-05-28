using System;
using System.Collections.Generic;
using System.Linq;
using MihaZupan;
using Newtonsoft.Json;
using SongLinkerBot.Entities;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace SongLinkerBot.Services
{
	public static class BotHelper
	{
		public static ITelegramBotClient BotClient;
		private static List<int> adminIds = new List<int>();

		public static void Configure()
		{
			var proxy = new HttpToSocks5Proxy( 
				Vault.ProxyHost,
				Vault.ProxyPort,
				Vault.ProxyUserName,
				Vault.ProxyPass);

			BotClient = new TelegramBotClient(Vault.Token);
			BotClient.WebProxy = proxy;
			BotClient.OnMessage += Bot_OnMessage;
		}

		public static void Start()
		{
			var me = BotClient.GetMeAsync().GetAwaiter().GetResult();
			Console.WriteLine(
				$"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
			);

			GetAdminsList();

			BotClient.StartReceiving();
		}
		static void Bot_OnMessage(object sender, MessageEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Message.Text))
				return;

			Console.WriteLine($"Received message {e.Message.Text} in chat {e.Message.Chat.Id}:{e.Message.Chat.Username}.");
			if (e.Message.Text.ToLower().Equals("/refresh"))
			{
				GetAdminsList();
				BotClient.SendTextMessageAsync(e.Message.From.Id, "Admins list loaded").GetAwaiter().GetResult();
				Console.WriteLine($"Admins list loaded");
				return;
			}

			if (!adminIds.Contains(e.Message.From.Id))
			{
				BotClient.SendTextMessageAsync(e.Message.From.Id, "You are not admin in channel").GetAwaiter().GetResult();
				Console.WriteLine($"{e.Message.Chat.Id}:{e.Message.Chat.Username} is not admin");
				return;
			}

			try
			{
				var url = new Uri(e.Message.Text);
			}
			catch
			{
				BotClient.SendTextMessageAsync(e.Message.From.Id, $"Received message {e.Message.Text} not a music link!").GetAwaiter().GetResult();
				Console.WriteLine($"Received message {e.Message.Text} in chat {e.Message.Chat.Id} not a link!");
			}

			var songProvider = new SongLinkSongProvider();
			var songData = songProvider.GetSongMetaData(e.Message.Text);
			
			Console.WriteLine(JsonConvert.SerializeObject(songData));

			if (songData != null)
			{
				BotClient.SendPhotoAsync(Vault.ChannelId, songData.Link, $"{songData.Title}\n{songData.Artist}\n\n{songData.Link}").GetAwaiter().GetResult();
				BotClient.SendTextMessageAsync(e.Message.From.Id, $"Posted!").GetAwaiter().GetResult();
			}
		}

		private static void GetAdminsList()
		{
			var admins = BotClient.GetChatAdministratorsAsync(Vault.ChannelId).GetAwaiter().GetResult();
			adminIds = admins.Select(x => x.User.Id).ToList();
		}

	}
}