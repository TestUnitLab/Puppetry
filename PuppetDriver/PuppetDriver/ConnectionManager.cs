using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

using Puppetry.PuppetDriver.Editor;

namespace Puppetry.PuppetDriver
{
    internal static class ConnectionManager
    {
        internal static int AvailableEditorsCount;

        internal static ConcurrentDictionary<string, string> MappingEditorsToSessions;

        internal static List<IEditorHandler> AvailiableEditors;

        static ConnectionManager()
        {
            AvailableEditorsCount = 0;

            //Key is IEditorHandler.Identificator, Value is sessioId
            MappingEditorsToSessions = new ConcurrentDictionary<string, string>();

            AvailiableEditors = new List<IEditorHandler>();
        }

        internal static void AddEditor(IEditorHandler editor)
        {
            lock (AvailiableEditors)
            {
                AvailiableEditors.Add(editor);
            }

            Interlocked.Increment(ref AvailableEditorsCount);
        }

        internal static void ReconnectEditor(Socket socket, string session)
        {
            lock (AvailiableEditors)
            {
                AvailiableEditors.First(x => x.Session == session).Socket = socket;
            }
        }

        internal static void RemoveEditor(IEditorHandler editor)
        {
            if (editor == null) return;

            lock (AvailiableEditors)
            {
                AvailiableEditors.Remove(editor);
            }

            Interlocked.Decrement(ref AvailableEditorsCount);
        }

        internal static IEditorHandler GetEditorHandler(string sessionId)
        {
            for (var i = 0; i < 60; i++)
            {
                try
                {
                    if (AvailableEditorsCount <= 0)
                        throw new Exception("Error: Available Editors is 0");

                    var editorIdentificator = MappingEditorsToSessions.FirstOrDefault(x => x.Value == sessionId).Key;

                    lock (AvailiableEditors)
                    {
                        return AvailiableEditors.First(x => x.Session == editorIdentificator);
                    }
                }
                catch (Exception)
                {
                    Thread.Sleep(500);
                }
            }

            throw new Exception("No Available Editors was received in 30 sec");
        }

        internal static void ReleaseEditorHandler(string sessionId)
        {
            MappingEditorsToSessions.Remove(MappingEditorsToSessions.First(x => x.Value == sessionId).Key, out var session);
        }
        
        internal static void ReleaseAllEditorHandlers()
        {
            foreach (var mappedEditorToSession in MappingEditorsToSessions)
            {
                MappingEditorsToSessions.Remove(mappedEditorToSession.Key, out var session);
            }
        }

        internal static string StartSession()
        {
            if (AvailableEditorsCount <= 0)
                return "Error: Available Editors is 0";

            lock (AvailiableEditors)
            {
                foreach (var editor in AvailiableEditors)
                {
                    if (!MappingEditorsToSessions.ContainsKey(editor.Session))
                    {
                        var sessionId = Guid.NewGuid().ToString();
                        var result = MappingEditorsToSessions.TryAdd(editor.Session, sessionId);

                        if (result) return sessionId;
                    }
                }

                if (MappingEditorsToSessions.Count == AvailableEditorsCount)
                    return "Error: All editors are busy at the moment";
            }

            return "Error: Some error was occured during creating session";
        }
    }
}
