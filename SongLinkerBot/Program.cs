using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SongLinkerBot.Entities;
using SongLinkerBot.Services;

namespace SongLinkerBot
{
	class Program
	{
		static void Main(string[] args)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("Vault.json");
 
			IConfiguration config = new ConfigurationBuilder()
				.AddJsonFile("Vault.json", false, true)
				.Build();
			
			Vault.Load(config);
			
			BotHelper.Configure();
			BotHelper.Start();

			Console.ReadLine();
		}
	}
}