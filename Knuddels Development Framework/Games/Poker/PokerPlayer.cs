using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.Games.Poker
{
    public enum PlayerSeat
    {
        Seat1 = 1,
        Seat2 = 2,
        Seat3 = 3,
        Seat4 = 4,
        Seat5 = 5,
        Seat6 = 6,
        Seat7 = 7,
        Seat8 = 8,
        Seat9 = 9,
        Unknown = 0
    }

    public class PokerPlayer
    {
        public int ComponentId { get; set; }
        public System.Drawing.Point Position { get; set; }
        public PlayerSeat TablePosition { get; set; }
        public PlayerSeat PositionFromMe { get; set; }
        public string Username { get; set; }
        public double Stack { get; set; }
        public double Bet { get; set; }
        public bool Folded { get; set; }
        public int Tickets { get; set; }

        public int Wins { get; set; }
        public int Folds { get; set; }
        public int Splits { get; set; }
        public int PotShare { get; set; }
        
    }
}
