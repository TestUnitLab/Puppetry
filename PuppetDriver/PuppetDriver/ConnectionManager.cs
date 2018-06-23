using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using PuppetDriver.Editor;

namespace PuppetDriver
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

        internal static void RemoveEditor(IEditorHandler editor)
        {
            lock (AvailiableEditors)
            {
                AvailiableEditors.Remove(editor);
            }

            Interlocked.Decrement(ref AvailableEditorsCount);
        }

        internal static IEditorHandler GetEditorHandler(string sessionId)
        {
            if (AvailableEditorsCount <= 0)
                throw new Exception("Error: Available Editors is 0");

            string editorIdentificator;

            editorIdentificator = MappingEditorsToSessions.FirstOrDefault(x => x.Value == sessionId).Key;

            lock (AvailiableEditors)
            {
                return AvailiableEditors.First(x => x.Identificator == editorIdentificator);
            }
        }

        internal static void ReleaseEditorHandler(string sessionId)
        {
            MappingEditorsToSessions.Remove(MappingEditorsToSessions.First(x => x.Value == sessionId).Key, out var session);
        }

        internal static string StartSession()
        {
            if (AvailableEditorsCount <= 0)
                return "Error: Available Editors is 0";

            lock (AvailiableEditors)
            {
                foreach (var editor in AvailiableEditors)
                {
                    if (!MappingEditorsToSessions.ContainsKey(editor.Identificator))
                    {
                        var sessionId = Guid.NewGuid().ToString();
                        var result = MappingEditorsToSessions.TryAdd(editor.Identificator, sessionId);

                        if (result) return sessionId;
                    }
                }
            }

            return "Error: Some error was occured during creating session";
        }
    }
}
