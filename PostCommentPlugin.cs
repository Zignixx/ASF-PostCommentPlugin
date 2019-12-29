using System;
using System.Collections.Generic;
using System.Composition;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArchiSteamFarm.Json;
using ArchiSteamFarm.Plugins;
using Newtonsoft.Json.Linq;
using SteamKit2;

namespace ArchiSteamFarm.Cobra.PostCommentPlugin {
	[Export(typeof(IPlugin))]
	internal sealed class PostCommentPlugin : IBotCommand {
		private static readonly Random Random = new Random();
		internal static int RandomNext(int min, int max) {
			lock (Random) {
				return Random.Next(min,max);
			}
		}
		public string Name => nameof(PostCommentPlugin);
		public Version Version => typeof(PostCommentPlugin).Assembly.GetName().Version;
		public async Task<string> OnBotCommand(Bot bot, ulong steamID, string message, string[] args) {
			switch (args[0].ToUpperInvariant()) {
				case "POSTCOMMENT" when bot.HasPermission(steamID, BotConfig.EPermission.Master):
					if (args.Length < 2) {
						return "Missing arguments!\n\nUse:\n!postcomment TARGETSTEAMID Some Text here";
					}
					if (bot.AccountFlags.HasFlag(EAccountFlags.LimitedUser) || bot.AccountFlags.HasFlag(EAccountFlags.LimitedUserForce)) {
						return bot.Commands.FormatBotResponse(ArchiSteamFarm.Localization.Strings.ErrorAccessDenied);
					}
					string comment = Utilities.GetArgsAsText(message, 2);
					string comment_request = "/comment/Profile/post/" + args[1] + "/-1/";
					//1 field for the sessionid
					Dictionary<string, string> comment_data = new Dictionary<string, string>(4) {
						{"comment", comment },
						{"count", "0" },
						{"feature2", "-1" }
					};
					await bot.ArchiWebHandler.UrlPostToHtmlDocumentWithSession(ArchiWebHandler.SteamCommunityURL, comment_request, comment_data).ConfigureAwait(false);
					return bot.Commands.FormatBotResponse(ArchiSteamFarm.Localization.Strings.Done);
				default:
					return null;
			}
		}
		public void OnLoaded() {
			ASF.ArchiLogger.LogGenericInfo("PostCommentPlugin by Cobra");
		}
	}
}
