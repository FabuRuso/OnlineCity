using Model;
using OCUnion;
using OCUnion.Transfer.Types;
using ServerOnlineCity.Model;
using ServerOnlineCity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Transfer;

namespace ServerOnlineCity.ChatService
{
    internal sealed class DeleteUserCmd : IChatCmd
    {
        public string CmdID => "deleteuser";

        public Grants GrantsForRun => Grants.SuperAdmin | Grants.Moderator | Grants.DiscordBot;

        public string Help => ChatManager.prefix + "deleteuser {UserLogin}: The player will be totally removed from server.";

        private readonly ChatManager _chatManager;

        public DeleteUserCmd(ChatManager chatManager)
        {
            _chatManager = chatManager;
        }

        public ModelStatus Execute(ref PlayerServer player, Chat chat, List<string> argsM, ServiceContext context)
        {
            var ownLogin = player.Public.Login;

            if (argsM.Count < 1)
            {
                return _chatManager.PostCommandPrivatPostActivChat(ChatCmdResult.PlayerNameEmpty, ownLogin, chat, "Player name is empty");
            }

            var deletePlayer = Repository.GetPlayerByLogin(argsM[0]);
            if (deletePlayer == null)
            {
                return _chatManager.PostCommandPrivatPostActivChat(ChatCmdResult.UserNotFound, ownLogin, chat, "User " + argsM[0] + " not found");
            }
            if ((deletePlayer.Public.Grants & Grants.SuperAdmin) == Grants.SuperAdmin)
            {
                return _chatManager.PostCommandPrivatPostActivChat(ChatCmdResult.UserNotFound, ownLogin, chat, $"Can't ban {argsM[0]}");
            }

            deletePlayer.AbandonSettlement();
            deletePlayer.Delete();

            return new ModelStatus() { Status = 0 };
        }
    }
}