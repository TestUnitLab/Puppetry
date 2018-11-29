using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

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

        internal static void AddPuppet(IPuppetHandler puppet)
        {
            lock (AvailiablePuppets)
            {
                AvailiablePuppets.Add(puppet);
            }
        }

        internal static void ReconnectPuppet(Socket socket, string session)
        {
            lock (AvailiablePuppets)
            {
                var puppet = AvailiablePuppets.First(x => x.Session == session);
                puppet.Socket = socket;
                puppet.IsAvailable = true;
            }
        }

        internal static void DisablePuppet(IPuppetHandler puppet)
        {
            if (puppet == null) return;

            lock (AvailiablePuppets)
            {
                AvailiablePuppets.First(p => p.Session == puppet.Session).IsAvailable = false;
            }
        }

        internal static IPuppetHandler GetPuppetHandler(string sessionId)
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
                    var puppet =  AvailiablePuppets.First(p => p.Session == editorIdentificator);
                    if (puppet.IsAvailable)
                        return puppet;
                    else
                        throw new Exception("Error: Mapped puppet is disabled");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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
                    if (!availiablePuppet.IsAvailable) continue;

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
