using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Puppetry.PuppetDriver.Puppet;

namespace Puppetry.PuppetDriver
{
    internal static class ConnectionManager
    {
        internal static ConcurrentDictionary<string, string> MappingPuppetsToSessions;

        internal static List<IPuppetHandler> AvailiablePuppets;

        static ConnectionManager()
        {
            //Key is IPuppetHandler.Identificator, Value is sessioId
            MappingPuppetsToSessions = new ConcurrentDictionary<string, string>();

            AvailiablePuppets = new List<IPuppetHandler>();
        }

        internal static void RemoveOldPuppets()
        {
            var listToRemove = new List<IPuppetHandler>();

            lock (AvailiablePuppets)
            {
                var currentDateTime = DateTime.UtcNow;
                foreach (var puppet in AvailiablePuppets)
                {
                    if (!puppet.Available)
                    {
                        if (currentDateTime - puppet.LastPing >= TimeSpan.FromMinutes(10))
                            listToRemove.Add(puppet);
                    }
                }

                foreach (var puppetToDelete in listToRemove)
                {
                    AvailiablePuppets.Remove(puppetToDelete);
                }
            }
        }

        internal static void AddPuppet(IPuppetHandler puppet)
        {
            lock (AvailiablePuppets)
            {
                AvailiablePuppets.Add(puppet);
            }
        }

        internal static IPuppetHandler ReconnectPuppet(Socket socket, string session)
        {
            lock (AvailiablePuppets)
            {
                var puppet = AvailiablePuppets.First(x => x.Session == session);
                puppet.Socket = socket;
                puppet.Available = true;

                return puppet;
            }
        }

        internal static void DisablePuppet(IPuppetHandler puppet)
        {
            if (puppet == null) return;

            lock (AvailiablePuppets)
            {
                AvailiablePuppets.First(p => p.Session == puppet.Session).Available = false;
            }
        }

        internal static IPuppetHandler GetPuppetHandler(string sessionId)
        {
            var tryCount = 0;
            while (true)
            {
                try
                {
                    lock (AvailiablePuppets)
                    {
                        if (AvailiablePuppets.Count <= 0)
                            throw new Exception("Error: Threre is no available Puppet");
                    }

                    var editorIdentificator = MappingPuppetsToSessions.FirstOrDefault(x => x.Value == sessionId).Key;

                    lock (AvailiablePuppets)
                    {
                        var puppet = AvailiablePuppets.First(p => p.Session == editorIdentificator);
                        if (puppet.Available)
                            return puppet;
                        else
                            throw new Exception("Error: Mapped puppet is disabled");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Try number {tryCount} has error {e}");
                    if (tryCount > 10)
                        throw;

                    Thread.Sleep(500);
                    tryCount++;
                }
            }
        }

        internal static void ReleasePuppetHandler(string sessionId)
        {
            MappingPuppetsToSessions.Remove(MappingPuppetsToSessions.First(x => x.Value == sessionId).Key, out var session);
        }
        
        internal static void ReleaseAllPuppetHandlers()
        {
            foreach (var mappedEditorToSession in MappingPuppetsToSessions)
            {
                MappingPuppetsToSessions.Remove(mappedEditorToSession.Key, out var session);
            }
        }

        internal static string StartSession()
        {
            lock (AvailiablePuppets)
            {
                if (AvailiablePuppets.Count <= 0)
                    return "Error: Threre is no available Puppet";

                foreach (var availiablePuppet in AvailiablePuppets)
                {
                    if (!availiablePuppet.Available) continue;

                    if (!MappingPuppetsToSessions.ContainsKey(availiablePuppet.Session))
                    {
                        var sessionId = Guid.NewGuid().ToString();
                        var result = MappingPuppetsToSessions.TryAdd(availiablePuppet.Session, sessionId);

                        if (result) return sessionId;
                    }
                }

                if (MappingPuppetsToSessions.Count == AvailiablePuppets.Count)
                    return "Error: All puppets are busy at the moment";
            }

            return "Error: Some error was occured during creating session";
        }
    }
}
