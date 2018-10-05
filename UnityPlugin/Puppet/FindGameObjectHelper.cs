using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Puppetry.Puppet
{
    public static class FindGameObjectHelper
    {
        private const string ParentAxe = "parent";
        private const string ChildAxe = "child";
        private const string DescendantAxe = "descendant";
        private const string AncestorAxe = "ancestor";

        public static GameObject FindGameObjectByUPath(string upath)
        {
            var currentUpath = upath;
            
            string typeOfSearch;
            int? index;
            string name;
            string condition;
            currentUpath = ProcessFirstUPathPart(currentUpath, out typeOfSearch, out index, out condition, out name);
            var currentGameObject = GameObject.Find(name);

            while (currentUpath.Length == 0 || currentGameObject == null) //we processed all upath or currentGameObject is null
            {
                currentUpath = ProcessFirstUPathPart(currentUpath, out typeOfSearch, out index, out condition, out name);

                if (index == null)
                    currentGameObject = FindGameObjectByExpression(currentGameObject, typeOfSearch, name, condition);
                else
                    currentGameObject = FindGameObjectsByExpression(currentGameObject, typeOfSearch, name)[(int) index];
            }

            return currentGameObject;
        }

        public static List<GameObject> FindGameObjectsByUPath(string upath)
        {
            var result = new List<GameObject>();
            var currentUpath = upath;
            
            string typeOfSearch;
            int? index;
            string name;
            string condition;
            currentUpath = ProcessFirstUPathPart(currentUpath, out typeOfSearch, out index, out condition, out name);
            var currentGameObject = GameObject.Find(name);

            while (true)
            {
                currentUpath = ProcessFirstUPathPart(currentUpath, out typeOfSearch, out index, out condition, out name);

                if (currentUpath.Length == 0)
                    result = FindGameObjectsByExpression(currentGameObject, typeOfSearch, name, condition);
                else if (index == null)
                    currentGameObject = FindGameObjectByExpression(currentGameObject, typeOfSearch, name, condition);
                else
                    currentGameObject = FindGameObjectsByExpression(currentGameObject, typeOfSearch, name)[(int) index];

                if (currentUpath.Length == 0 || currentGameObject == null
                ) //we processed all upath or currentGameObject is null
                    break;
            }

            return result;
        }

        public static List<GameObject> GetGameObjects(string gameObjectName, string root, string parentName = null)
        {
            GameObject parent;
            if (!string.IsNullOrEmpty(parentName))
                parent = FindDescendant(parentName, GameObject.Find(root));
            else
                parent = GameObject.Find(root);
            return FindDescendants(gameObjectName, parent);
        }

        public static GameObject FindGameObject(string nameOrPath, string parentName, string root)
        {
            if (string.IsNullOrEmpty(parentName))
            {
                return FindDescendant(nameOrPath, GameObject.Find(root));
            }

            var parentGO = FindDescendant(parentName, GameObject.Find(root));
            if (parentGO != null)
            {
                if (nameOrPath.Contains("/"))
                {
                    var transform = parentGO.transform.Find(nameOrPath);
                    return transform != null ? transform.gameObject : null;
                }
                else
                {
                    return FindDescendant(nameOrPath, parentGO);
                }
            }
            else
            {
                return null;
            }
        }

        private static string ProcessFirstUPathPart(string upath, out string typeOfSearch, out int? index,
            out string condition, out string name)
        {
            var expression = new Regex(@"^(?<expression>/..[^/]*)").Match(upath).Groups["expression"].ToString();

            var resultedUPath = upath.Substring(expression.Length); //remove already extracted path

            typeOfSearch = new Regex(@"^(?<type>)//|/[a-z]*[:]{2}|/").Match(expression).Groups[0].ToString();
            expression = expression.Replace(typeOfSearch, string.Empty); //remove type of search from the expression

            var property = new Regex(@"^.*\[(?<property>.*[^\]])").Match(expression).Groups["property"].ToString();
            index = null;
            condition = null;
            if (!string.IsNullOrEmpty(property))
            {
                int parseResult;
                if (int.TryParse(property, out parseResult))
                    index = parseResult;
                else
                    condition = property;

                expression = expression.Replace("[" + property + "]", string.Empty);
            }

            name = expression;

            return resultedUPath;
        }

        private static List<GameObject> FindGameObjectsByExpression(GameObject root, string typeOfSearch, string name,
            string condition = null)
        {
            var result = new List<GameObject>();

            switch (typeOfSearch)
            {
                case "/" + ParentAxe + "::":
                    if (condition != null)
                    {
                        var examinedGameObject = FindParent(name, root);
                        if (IsFulfilledCondition(examinedGameObject, condition))
                            result.Add(examinedGameObject);
                    }
                    else
                    {
                        result.Add(FindParent(name, root));
                    }

                    break;

                case "/" + AncestorAxe + "::":
                    if (condition != null)
                    {
                        var ancestors = FindAncestors(name, root);
                        foreach (var ancestor in ancestors)
                        {
                            if (IsFulfilledCondition(ancestor, condition))
                            {
                                result.Add(ancestor);
                            }
                        }
                    }
                    else
                    {
                        result = FindAncestors(name, root);
                    }

                    break;

                case "/" + ChildAxe + "::":
                case "/":
                    if (condition != null)
                    {
                        var children = FindChildren(name, root);
                        foreach (var child in children)
                        {
                            if (IsFulfilledCondition(child, condition))
                            {
                                result.Add(child);
                            }
                        }
                    }
                    else
                    {
                        result = FindChildren(name, root);
                    }

                    break;

                case "/" + DescendantAxe + "::":
                case "//":
                    if (condition != null)
                    {
                        var descendants = FindDescendants(name, root);
                        foreach (var descendant in descendants)
                        {
                            if (IsFulfilledCondition(descendant, condition))
                            {
                                result.Add(descendant);
                            }
                        }
                    }
                    else
                    {
                        result = FindDescendants(name, root);
                    }

                    break;
            }

            return result;
        }

        private static GameObject FindGameObjectByExpression(GameObject root, string typeOfSearch, string name,
            string condition)
        {
            GameObject result = null;
            switch (typeOfSearch)
            {
                case "/" + ParentAxe + "::":
                    if (condition != null)
                    {
                        var examinedGameObject = FindParent(name, root);
                        result = IsFulfilledCondition(examinedGameObject, condition) ? examinedGameObject : null;
                    }
                    else
                    {
                        result = FindParent(name, root);
                    }

                    break;

                case "/" + AncestorAxe + "::":

                    if (condition != null)
                    {
                        var ancestors = FindAncestors(name, root);
                        foreach (var ancestor in ancestors)
                        {
                            if (IsFulfilledCondition(ancestor, condition))
                            {
                                result = ancestor;
                                break;
                            }
                        }
                    }
                    else
                    {
                        result = FindAncestor(name, root);
                    }

                    break;

                case "/" + ChildAxe + "::":
                case "/":
                    if (condition != null)
                    {
                        var children = FindChildren(name, root);
                        foreach (var child in children)
                        {
                            if (IsFulfilledCondition(child, condition))
                            {
                                result = child;
                                break;
                            }
                        }
                    }
                    else
                    {
                        result = FindChild(name, root);
                    }

                    break;

                case "/" + DescendantAxe + "::":
                case "//":
                    if (condition != null)
                    {
                        var descendants = FindDescendants(name, root);
                        foreach (var descendant in descendants)
                        {
                            if (IsFulfilledCondition(descendant, condition))
                            {
                                result = descendant;
                                break;
                            }
                        }
                    }
                    else
                    {
                        result = FindDescendant(name, root);
                    }

                    break;

                case "/..":
                    result = GetParent(root);
                    break;
            }

            return result;
        }

        private static GameObject GetParent(GameObject child)
        {
            var parent = child.transform.parent;

            return parent != null ? parent.gameObject : null;
        }

        private static GameObject FindParent(string name, GameObject child)
        {
            var parent = child.transform.parent;

            if (parent == null || parent.name != name)
                return null;

            return parent.gameObject;
        }

        private static GameObject FindAncestor(string name, GameObject child)
        {
            var gameObject = FindParent(name, child);

            if (gameObject != null)
                return gameObject;

            var parent = child.transform.parent;

            return parent != null ? FindAncestor(name, parent.gameObject) : null;
        }

        private static GameObject FindChild(string name, GameObject parent)
        {
            var gameObject = parent.transform.Find(name);

            return gameObject != null ? gameObject.gameObject : null;
        }

        private static GameObject FindDescendant(string name, GameObject parent)
        {
            var gameObject = FindChild(name, parent);

            if (gameObject != null)
                return gameObject;

            foreach (Transform child in parent.transform)
            {
                gameObject = FindDescendant(name, child.gameObject);
                if (gameObject != null)
                {
                    return gameObject;
                }
            }

            return null;
        }

        private static List<GameObject> FindAncestors(string name, GameObject child)
        {
            var ancestorsList = new List<GameObject>();

            var currentGameObject = child;

            while (true)
            {
                var parent = currentGameObject.transform.parent;

                if (parent == null)
                    break;

                if (parent.name == name)
                    ancestorsList.Add(parent.gameObject);

                currentGameObject = parent.gameObject;
            }

            return ancestorsList;
        }

        private static List<GameObject> FindChildren(string gameObjectName, GameObject parent)
        {
            var childrenList = new List<GameObject>();

            var count = parent.transform.childCount;
            for (var i = 0; i < count; i++)
            {
                var child = parent.transform.GetChild(i).gameObject;
                if (child.name == gameObjectName)
                    childrenList.Add(child);
            }

            return childrenList;
        }

        private static List<GameObject> FindDescendants(string name, GameObject parent)
        {
            var descendantsList = new List<GameObject>();

            FindDescendantsRecursive(name, parent, descendantsList);

            return descendantsList;
        }

        private static void FindDescendantsRecursive(string name, GameObject parent, List<GameObject> list)
        {
            var count = parent.transform.childCount;
            for (var i = 0; i < count; i++)
            {
                var child = parent.transform.GetChild(i).gameObject;
                if (child.name == name)
                    list.Add(child);

                FindDescendantsRecursive(name, child, list);
            }
        }

        private static bool IsFulfilledCondition(GameObject gameObject, string condition)
        {
            var currentCondition = condition;
            var typeOfSearch = new Regex(@"^(?<type>)[a-z]*[:]{2}").Match(currentCondition).Groups[0].ToString();
            var name = currentCondition.Replace(typeOfSearch, string.Empty); //remove type of search from the expression

            var result = false;
            switch (typeOfSearch)
            {
                case ParentAxe + "::":
                    result = FindParent(name, gameObject) != null;
                    break;
                case ChildAxe + "::":
                    result = FindChild(name, gameObject) != null;
                    break;
                case AncestorAxe + "::":
                    result = FindAncestor(name, gameObject) != null;
                    break;
                case DescendantAxe + "::":
                    result = FindDescendant(name, gameObject) != null;
                    break;
            }

            return result;
        }
    }
}