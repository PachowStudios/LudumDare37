﻿using System;
using JetBrains.Annotations;

namespace PachowStudios.Framework
{
  [AttributeUsage(AttributeTargets.Field)]
  public class TypeMappingAttribute : Attribute
  {
    [NotNull] public Type Type { get; }

    public TypeMappingAttribute([NotNull] Type type)
    {
      Type = type;
    }
  }
}