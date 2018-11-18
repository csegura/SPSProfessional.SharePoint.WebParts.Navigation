using System;
using System.Diagnostics;
using System.Web.UI.WebControls;
using SPSProfessional.SharePoint.Framework.Hierarchy;

namespace SPSProfessional.SharePoint.WebParts.Navigation.Tools
{
    public delegate void TreeBound(TreeNode treeNode, SPSNodeBase hierarchyNode);

    internal static class HierarchyTools
    {
        public static void PopulateTreeView(TreeView tree, SPSHierarchyDataSource dataSource, TreeBound delegateBound)
        {
            if (tree != null && dataSource != null)
            {
                TreeNode treeNode = PopulateTreeNodes(dataSource, delegateBound);
                if (treeNode != null)
                {
                    tree.Nodes.Add(treeNode);
                    DecorateTree(tree);
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        private static TreeNode PopulateTreeNodes(SPSHierarchyDataSource dataSource, TreeBound delegateBound)
        {
            TreeNode root;

            using(SPSNodeCollection dataNodes = dataSource.GetAll())
            {
                SPSNodeBase dataRoot = dataNodes[0];
                root = new TreeNode(dataRoot.Name, dataRoot.UrlSegment, dataRoot.ImageUrl, dataRoot.NavigateUrl, "");

                if (delegateBound != null)
                {
                    delegateBound(root, dataRoot);
                }

                PopulateTreeNodesRecursive(root, dataRoot.Children, delegateBound);
            }
            return root;
        }

        private static void PopulateTreeNodesRecursive(TreeNode node,
                                                       SPSNodeCollection dataChildrens,
                                                       TreeBound delegateBound)
        {
            foreach (SPSNodeBase subDataNode in dataChildrens)
            {
                TreeNode newNode = new TreeNode(subDataNode.Name,
                                                subDataNode.UrlSegment,
                                                subDataNode.ImageUrl,
                                                subDataNode.NavigateUrl,
                                                "");

                node.ChildNodes.Add(newNode);

                Debug.WriteLine(string.Format("-> {0},{1},{2},{3}",
                                              subDataNode.Name,
                                              subDataNode.UrlSegment,
                                              subDataNode.ImageUrl,
                                              subDataNode.NavigateUrl));

                if (delegateBound != null)
                {
                    delegateBound(newNode, subDataNode);
                }

                PopulateTreeNodesRecursive(newNode, subDataNode.Children, delegateBound);
                subDataNode.Dispose();
            }
        }


        public static void DecorateTree(TreeView tree)
        {
            tree.ShowLines = true;
            tree.EnableClientScript = true;
            tree.EnableViewState = true;
            tree.NodeStyle.CssClass = "ms-navitem";
            tree.NodeStyle.HorizontalPadding = 2;
            tree.SelectedNodeStyle.CssClass = "ms-tvselected";
            tree.SkipLinkText = "";
            tree.NodeIndent = 12;
            tree.ExpandImageUrl = "/_layouts/images/tvplus.gif";
            tree.CollapseImageUrl = "/_layouts/images/tvminus.gif";
            tree.NoExpandImageUrl = "/_layouts/images/tvblank.gif";
        }
    }
}