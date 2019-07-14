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
        private const string Active = "active";
        private const string ContainsExpression = "contains(";

        public static GameObject FindGameObjectByUPath(string upath)
        {
            var currentUpath = upath;
            
            string typeOfSearch;
            int? index;
            string name;
            string condition;
            bool fullName;
            currentUpath = ProcessFirstUPathPart(currentUpath, out typeOfSearch, out index, out condition, out name, out fullName);

            GameObject currentGameObject;
            if (fullName)
                currentGameObject = GameObject.Find(name);
            else
                currentGameObject = null;

            while (currentUpath.Length != 0 && currentGameObject != null) //we processed all upath or currentGameObject is null
            {
                currentUpath = ProcessFirstUPathPart(currentUpath, out typeOfSearch, out index, out condition, out name, out fullName);

                if (index == null)
                    currentGameObject = FindGameObjectByExpression(currentGameObject, typeOfSearch, name, condition, fullName);
                else
                    currentGameObject = FindGameObjectsByExpression(currentGameObject, typeOfSearch, name, condition, fullName)[(int) index];
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
            bool fullName;
            currentUpath = ProcessFirstUPathPart(currentUpath, out typeOfSearch, out index, out condition, out name, out fullName);
            var currentGameObject = GameObject.Find(name);

            while (currentUpath.Length != 0 && currentGameObject != null) //we processed all upath or currentGameObject is null
            {
                currentUpath = ProcessFirstUPathPart(currentUpath, out typeOfSearch, out index, out condition, out name, out fullName);

                if (currentUpath.Length == 0)
                    result = FindGameObjectsByExpression(currentGameObject, typeOfSearch, name, condition, fullName);
                else if (index == null)
                    currentGameObject = FindGameObjectByExpression(currentGameObject, typeOfSearch, name, condition, fullName);
                else
                    currentGameObject = FindGameObjectsByExpression(currentGameObject, typeOfSearch, name, condition, fullName)[(int) index];
            }

            return result;
        }

        private static string ProcessFirstUPathPart(string upath, out string typeOfSearch, out int? index, out string condition, out string name, out bool fullName)
        {
            var expression = new Regex(@"^(?<expression>/..[^/]*)").Match(upath).Groups["expression"].ToString();

            var resultedUPath = upath.Substring(expression.Length); //remove already extracted path

            typeOfSearch = new Regex(@"^(?<type>)//|/[a-z]*[:]{2}|/").Match(expression).Groups[0].ToString();
            expression = expression.Replace(typeOfSearch, string.Empty); //remove type of search from the expression
            index = null;
            condition = null;
            fullName = true;
            while (true)
            {
                var property = new Regex(@"^.*\[(?<property>.*[^\]])").Match(expression).Groups["property"].ToString();

                if (!string.IsNullOrEmpty(property))
                {
                    int parseResult;
                    if (int.TryParse(property, out parseResult))
                        index = parseResult;
                    else
                        condition = property;

                    expression = expression.Replace("[" + property + "]", string.Empty);
                }
                else
                    break;
            }

            if (expression.StartsWith(ContainsExpression) && expression.EndsWith(")"))
            {
                fullName = false;
                //Extract part of name
                expression = expression.Substring(ContainsExpression.Length, expression.Length - ContainsExpression.Length - 1);
            }
            name = expression;

            return resultedUPath;
        }

        private static List<GameObject> FindGameObjectsByExpression(GameObject root, string typeOfSearch, string name, string condition, bool fullName)
        {
            var result = new List<GameObject>();

            switch (typeOfSearch)
            {
                case "/" + ParentAxe + "::":
                    if (condition != null)
                    {
                        var examinedGameObject = FindParent(name, root, fullName);
                        if (IsFulfilledCondition(examinedGameObject, condition))
                            result.Add(examinedGameObject);
                    }
                    else
                    {
                        result.Add(FindParent(name, root, fullName));
                    }

                    break;

                case "/" + AncestorAxe + "::":
                    if (condition != null)
                    {
                        var ancestors = FindAncestors(name, root, fullName);
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
                        result = FindAncestors(name, root, fullName);
                    }

                    break;

                case "/" + ChildAxe + "::":
                case "/":
                    if (condition != null)
                    {
                        var children = FindChildren(name, root, fullName);
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
                        result = FindChildren(name, root, fullName);
                    }

                    break;

                case "/" + DescendantAxe + "::":
                case "//":
                    if (condition != null)
                    {
                        var descendants = FindDescendants(name, root, fullName);
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
                        result = FindDescendants(name, root, fullName);
                    }

                    break;
            }

            return result;
        }

        private static GameObject FindGameObjectByExpression(GameObject root, string typeOfSearch, string name, string condition, bool fullName)
        {
            GameObject result = null;
            switch (typeOfSearch)
            {
                case "/" + ParentAxe + "::":
                    if (condition != null)
                    {
                        var examinedGameObject = FindParent(name, root, fullName);
                        result = IsFulfilledCondition(examinedGameObject, condition) ? examinedGameObject : null;
                    }
                    else
                    {
                        result = FindParent(name, root, fullName);
                    }

                    break;

                case "/" + AncestorAxe + "::":

                    if (condition != null)
                    {
                        var ancestors = FindAncestors(name, root, fullName);
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
                        result = FindAncestor(name, root, fullName);
                    }

                    break;

                case "/" + ChildAxe + "::":
                case "/":
                    if (condition != null)
                    {
                        var children = FindChildren(name, root, fullName);
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
                        result = FindChild(name, root, fullName);
                    }

                    break;

                case "/" + DescendantAxe + "::":
                case "//":
                    if (condition != null)
                    {
                        var descendants = FindDescendants(name, root, fullName);
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
                        result = FindDescendant(name, root, fullName);
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

        private static GameObject FindParent(string name, GameObject child, bool fullName = true)
        {
            var parent = child.transform.parent;

            if (parent == null)
                return null;
            else if (fullName && parent.name != name)
                return null;
            else if (!fullName && !parent.name.Contains(name))
                return null;

            return parent.gameObject;
        }

        private static GameObject FindAncestor(string name, GameObject child, bool fullName = true)
        {
            var gameObject = FindParent(name, child, fullName);

            if (gameObject != null)
                return gameObject;

            var parent = child.transform.parent;

            return parent != null ? FindAncestor(name, parent.gameObject, fullName) : null;
        }

        private static GameObject FindChild(string name, GameObject parent, bool fullName = true)
        {
            Transform gameObjectTransform = null;
            if (fullName)
                gameObjectTransform = parent.transform.Find(name);
            else
            {
                var count = parent.transform.childCount;
                for (var i = 0; i < count; i++)
                {
                    var child = parent.transform.GetChild(i).gameObject;
                    if (child.name.Contains(name))
                    {
                        gameObjectTransform = child.transform;
                        break;
                    }
                }
            }

            return gameObjectTransform != null ? gameObjectTransform.gameObject : null;
        }

        private static GameObject FindDescendant(string name, GameObject parent, bool fullName = true)
        {
            var gameObject = FindChild(name, parent, fullName);

            if (gameObject != null)
                return gameObject;

            foreach (Transform child in parent.transform)
            {
                gameObject = FindDescendant(name, child.gameObject, fullName);
                if (gameObject != null)
                {
                    return gameObject;
                }
            }

            return null;
        }

        private static List<GameObject> FindAncestors(string name, GameObject child, bool fullName = true)
        {
            var ancestorsList = new List<GameObject>();

            var currentGameObject = child;

            while (true)
            {
                var parent = currentGameObject.transform.parent;

                if (parent == null)
                    break;

                if ((fullName && parent.name == name) || (!fullName && parent.name.Contains(name)))
                    ancestorsList.Add(parent.gameObject);

                currentGameObject = parent.gameObject;
            }

            return ancestorsList;
        }

        private static List<GameObject> FindChildren(string name, GameObject parent, bool fullName = true)
        {
            var childrenList = new List<GameObject>();

            var count = parent.transform.childCount;
            for (var i = 0; i < count; i++)
            {
                var child = parent.transform.GetChild(i).gameObject;
                if ((fullName && child.name == name) || (!fullName && child.name.Contains(name)))
                    childrenList.Add(child);
            }

            return childrenList;
        }

        private static List<GameObject> FindDescendants(string name, GameObject parent, bool fullName = true)
        {
            var descendantsList = new List<GameObject>();

            FindDescendantsRecursive(name, parent, descendantsList, fullName);

            return descendantsList;
        }

        private static void FindDescendantsRecursive(string name, GameObject parent, List<GameObject> list, bool fullName = true)
        {
            var count = parent.transform.childCount;
            for (var i = 0; i < count; i++)
            {
                var child = parent.transform.GetChild(i).gameObject;
                if ((fullName && child.name == name) || (!fullName && child.name.Contains(name)))
                    list.Add(child);

                FindDescendantsRecursive(name, child, list);
            }
        }

        private static bool IsFulfilledCondition(GameObject gameObject, string condition)
        {
            var currentCondition = condition;
            var typeOfSearch = new Regex(@"^(?<type>)[a-z]*[:]{2}").Match(currentCondition).Groups[0].ToString();
            string name = null;
            var fullName = true;
            if (string.IsNullOrEmpty(typeOfSearch))
                typeOfSearch = currentCondition;
            else
            {
                name = currentCondition.Replace(typeOfSearch, string.Empty); //remove type of search from the expression
                if (name.StartsWith(ContainsExpression) && name.EndsWith(")"))
                {
                    fullName = false;
                    //Extract partName from expression: contains( + partName + )
                    name = name.Substring(ContainsExpression.Length, name.Length - ContainsExpression.Length - 1);
                }
            }

            var result = false;
            switch (typeOfSearch)
            {
                case ParentAxe + "::":
                    result = FindParent(name, gameObject, fullName) != null;
                    break;
                case ChildAxe + "::":
                    result = FindChild(name, gameObject, fullName) != null;
                    break;
                case AncestorAxe + "::":
                    result = FindAncestor(name, gameObject, fullName) != null;
                    break;
                case DescendantAxe + "::":
                    result = FindDescendant(name, gameObject, fullName) != null;
                    break;
                case Active:
                    result = gameObject.activeInHierarchy;
                    break;
                case "!" + Active:
                    result = !gameObject.activeInHierarchy;
                    break;
            }

            return result;
        }
    }
}