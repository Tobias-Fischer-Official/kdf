using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.Games
{
    public enum Game
    {
        Poker,
        MauMau
    }

    public interface GameBase
    {
        void RegisterForGame();
        void CancelGame();
        void TryCancelGame();
        void JoinGameChannel(Game game);
    }
}
