using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.Games.Poker
{
    //Die Nummer gibt den Bildnamen an (chars weggeparsed) wie das bild auf dem Knuddels server liegt
    public enum Card
    {
        //T = 10, J= Bube, Q = Dame, K=könig, A= ass
        //Karo
        d2 = 12,
        d3 = 13,
        d4 = 14,
        d5 = 15,
        d6 = 16,
        d7 = 17,
        d8 = 18,
        d9 = 19,
        dt = 110,
        dj = 111,
        dq = 112,
        dk = 113,
        da = 114,
        //Herz
        h2 = 22,
        h3 = 23,
        h4 = 24,
        h5 = 25,
        h6 = 26,
        h7 = 27,
        h8 = 28,
        h9 = 29,
        ht = 210,
        hj = 211,
        hq = 212,
        hk = 213,
        ha = 214,
        //Pik
        s2 = 32,
        s3 = 33,
        s4 = 34,
        s5 = 35,
        s6 = 36,
        s7 = 37,
        s8 = 38,
        s9 = 39,
        st = 310,
        sj = 311,
        sq = 312,
        sk = 313,
        sa = 314,
        //Kreuz
        c2 = 42,
        c3 = 43,
        c4 = 44,
        c5 = 45,
        c6 = 46,
        c7 = 47,
        c8 = 48,
        c9 = 49,
        ct = 410,
        cj = 411,
        cq = 412,
        ck = 413,
        ca = 414
    }

    public class PokerCard
    {
        public string[] alpha = {" ","?","?","?","?","?","?","","§"," ","!","\"","#","$","%","&","'","(",")","*","+",",","-",".","/",":",";","<","=",">","?","@","A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z","[","\\","]","^","_","`","a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z","{","|","}","~","¦","?" };

        public int Value { get; set; }
        public string FileName { get; set; }
        public Card CardName { get; set; }

        public PokerCard(string imgUrl)
        {
            FileName = imgUrl;
            CardName = (Card)int.Parse(string.Concat(imgUrl.Split(alpha, StringSplitOptions.RemoveEmptyEntries)));
        }        
    }
}
