﻿using DotBook.Model.Entities;
using DotBook.Processing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using static DotBook.Utils.Common;

namespace DotBook.Model.Members
{
    public class PropertyInfo : IMember
    {
        public string Name { get; }
        public string FullName { get => $"{Parent.FullName}.{Name}"; }

        public INameable NodeValue => this;

        private SortedSet<Modifier> _modifiers = new SortedSet<Modifier>();
        public IReadOnlyCollection<Modifier> Modifiers => _modifiers;

        public string Type { get; }
        public AccessorInfo Getter { get; }
        public AccessorInfo Setter { get; }
        public bool HasGetter => Getter != null;
        public bool HasSetter => Setter != null;

        public string Documentation { get; }

        public IMemberContainer Parent { get; }

        public INode<INameable> ParentNode => Parent;

        public IEnumerable<INode<INameable>> ChildrenNodes => null;

        public PropertyInfo(PropertyDeclarationSyntax decl, IMemberContainer parent)
        {
            Name = decl.Identifier.Text;
            if (decl.HasLeadingTrivia)
                Documentation = GetDocumentation(decl.GetLeadingTrivia());
            Parent = parent;
            _modifiers = decl.Modifiers
                .ParseModifiers()
                .WithDefaultVisibility(Parent is InterfaceInfo ?
                    Modifier.Public : Modifier.Private);

            Type = decl.Type.ToString();

            var accessors = decl.AccessorList?.Accessors
                .Select(a => new AccessorInfo(a, this));

            if (accessors != null)
            {
                Setter = accessors.FirstOrDefault(a => a.IsSetter);
                Getter = accessors.FirstOrDefault(a => a.IsGetter);
            }
            else if (decl.ExpressionBody != null)
                Getter = new AccessorInfo(this);
        }

        public int CompareTo(object obj) =>
            FullName.CompareTo((obj as PropertyInfo)?.FullName);
    }
}
