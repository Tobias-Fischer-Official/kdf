using System;
using KDF.Networks.Core;
using System.Reflection;
using KDF.Networks.GameServer;

namespace KDF_Console_Sample
{
    class Program
    {
        static KnuddelsClient c;
        static void Main(string[] args)
        {
            c = new KnuddelsClient();
            //c.ClientType = ClientType.iPhone;
            c.OnChatComponentCommandReceived += new EventHandler<KDF.ClientEventArgs.ChatComponentReceivedEventArgs>(c_OnChatComponentCommandReceived);
            c.OnCardServerConnectionEstablished += new EventHandler<KDF.ClientEventArgs.GameServerConnectionEventArgs>(c_OnCardServerConnection);
            c.OnDataReceived += new EventHandler<KDF.ClientEventArgs.DataReceivedEventArgs>(c_OnDataReceived);
            c.OnLoginFailed += new EventHandler<KDF.ClientEventArgs.LoginFailedEventArgs>(c_OnLoginFailed);
            c.OnGlobalException += new EventHandler<KDF.ClientEventArgs.GlobalExceptionEventArgs>(c_OnGlobalException);
            c.OnConnectionStateChanged += new EventHandler<KDF.ClientEventArgs.ConnectionStateChangedEventArgs>(c_OnConnectionStateChanged);
            c.Connect(ChatSystem.de);
        }

        static bool first = true;
        static void c_OnConnectionStateChanged(object sender, KDF.ClientEventArgs.ConnectionStateChangedEventArgs e)
        {
            if (e.Connected == true && e.LoggedIn == false && first)
            {
                c.Login("kdf1", "123", "Poker $2");
                first = false;
            }
        }

        static void c_OnGlobalException(object sender, KDF.ClientEventArgs.GlobalExceptionEventArgs e)
        {
            //Console.WriteLine(e.Exception);
        }

        static void c_OnLoginFailed(object sender, KDF.ClientEventArgs.LoginFailedEventArgs e)
        {
            Console.WriteLine(e.Reason);
        }
        static void c_OnDataReceived(object sender, KDF.ClientEventArgs.DataReceivedEventArgs e)
        {
            //Console.WriteLine(e.Data);
        }
        static void c_OnChatComponentCommandReceived(object sender, KDF.ClientEventArgs.ChatComponentReceivedEventArgs e)
        {
            //Console.WriteLine("[MODULE] " + e.Module.Name);
        }

        #region CardServer
        static void c_OnCardServerConnection(object sernder, KDF.ClientEventArgs.GameServerConnectionEventArgs e)
        {
            e.Client.OnConnectionStateChanged += new EventHandler<KDF.ClientEventArgs.ConnectionStateChangedEventArgs>(csClient_OnConnectionStateChanged);
            e.Client.OnModuleReceived += new EventHandler<KDF.ClientEventArgs.ModuleReceivedEventArgs>(csClient_OnModuleReceived);
            
            e.Client.Connect();
        }

        static void csClient_OnConnectionStateChanged(object sender, KDF.ClientEventArgs.ConnectionStateChangedEventArgs e)
        {
            GSClient client = (GSClient)sender;
            if (e.Connected && !e.LoggedIn)
                client.Login();
            else if (e.Connected && e.LoggedIn)
                client.JoinRoom();
        }

        static void csClient_OnModuleReceived(object sender, KDF.ClientEventArgs.ModuleReceivedEventArgs e)
        {
            Console.WriteLine("[CardServer] " + e.Module.Name);
        }
        #endregion

    }
}
