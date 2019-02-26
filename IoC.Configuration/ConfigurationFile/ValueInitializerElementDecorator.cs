// This software is part of the IoC.Configuration library
// Copyright © 2018 IoC.Configuration Contributors
// http://oroptimizer.com
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;
using OROptimizer.DynamicCode;

namespace IoC.Configuration.ConfigurationFile
{
    public abstract class ValueInitializerElementDecorator : IValueInitializerElementDecorator
    {
        #region Member Variables

        private bool _addCodeGenerateValueCSharpWasCalled;

        #endregion

        #region  Constructors

        protected ValueInitializerElementDecorator([NotNull] IValueInitializerElement decoratedValueInitializerElement)
        {
            DecoratedValueInitializerElement = decoratedValueInitializerElement;
        }

        #endregion

        #region IValueInitializerElementDecorator Interface Implementation

        public void AddChild(IConfigurationFileElement child)
        {
            InterceptValueInitialzerException(() => DecoratedValueInitializerElement.AddChild(child));
        }

        public void BeforeChildInitialize(IConfigurationFileElement child)
        {
            InterceptValueInitialzerException(() => DecoratedValueInitializerElement.BeforeChildInitialize(child));
        }

        public IReadOnlyList<IConfigurationFileElement> Children => DecoratedValueInitializerElement.Children;
        public IConfiguration Configuration => DecoratedValueInitializerElement.Configuration;

        [NotNull]
        public IValueInitializerElement DecoratedValueInitializerElement { get; }

        public string ElementName => DecoratedValueInitializerElement.ElementName;

        string IConfigurationFileElement.GenerateElementError(string message, IConfigurationFileElement parentElement)
        {
            return ErrorHelperAmbientContext.Context.GenerateElementError(DecoratedValueInitializerElement, message, parentElement);
        }

        public string GenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            if (!_addCodeGenerateValueCSharpWasCalled)
            {
                _addCodeGenerateValueCSharpWasCalled = true;
                AddCodeOnGenerateValueCSharp(dynamicAssemblyBuilder);
            }

            return DoGenerateValueCSharp(dynamicAssemblyBuilder);
        }

        public string GetAttributeValue(string attributeName)
        {
            return DecoratedValueInitializerElement.GetAttributeValue(attributeName);
        }

        IPluginSetup IConfigurationFileElement.GetParentPluginSetupElement()
        {
#pragma warning disable CS0612, CS0618
            return DecoratedValueInitializerElement.GetParentPluginSetupElement();
#pragma warning restore CS0612, CS0618
        }

        public IPluginSetup GetPluginSetupElement()
        {
            return DecoratedValueInitializerElement.GetPluginSetupElement();
        }

        public bool HasAttribute(string attributeName)
        {
            return DecoratedValueInitializerElement.HasAttribute(attributeName);
        }

        public IPluginElement OwningPluginElement => DecoratedValueInitializerElement.OwningPluginElement;

        public IConfigurationFileElement Parent => DecoratedValueInitializerElement.Parent;

        public ITypeInfo ValueTypeInfo => DecoratedValueInitializerElement.ValueTypeInfo;

        #endregion

        #region Current Type Interface

        protected virtual void AddCodeOnGenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
        }

        protected virtual string DoGenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            return InterceptValueInitialzerExceptionWithReturnedValue(
                () => DecoratedValueInitializerElement.GenerateValueCSharp(dynamicAssemblyBuilder));
        }

        public virtual bool Enabled => DecoratedValueInitializerElement.Enabled;

        public virtual object GenerateValue()
        {
            return InterceptValueInitialzerExceptionWithReturnedValue(DecoratedValueInitializerElement.GenerateValue);
        }

        public virtual void Initialize()
        {
            InterceptValueInitialzerException(DecoratedValueInitializerElement.Initialize);
        }

        public virtual bool IsResolvedFromDiContainer => DecoratedValueInitializerElement.IsResolvedFromDiContainer;

        public virtual void ValidateAfterChildrenAdded()
        {
            InterceptValueInitialzerException(DecoratedValueInitializerElement.ValidateAfterChildrenAdded);
        }

        public virtual void ValidateOnTreeConstructed()
        {
            InterceptValueInitialzerException(DecoratedValueInitializerElement.ValidateOnTreeConstructed);
        }

        public virtual string XmlElementToString()
        {
            return DecoratedValueInitializerElement.XmlElementToString();
        }

        #endregion

        #region Member Functions

        private void InterceptValueInitialzerException([NotNull] Action action)
        {
            try
            {
                action();
            }
            catch (ConfigurationParseException e) when (e.ConfigurationFileElement == DecoratedValueInitializerElement)
            {
                throw new ConfigurationParseException(this, e.OriginalMessage, e.ParentConfigurationFileElement);
            }
            catch (ConfigurationParseException e) when (e.ParentConfigurationFileElement == DecoratedValueInitializerElement)
            {
                throw new ConfigurationParseException(e.ConfigurationFileElement, e.OriginalMessage, this);
            }
            catch (ConfigurationParseException e) when (e.ConfigurationFileElement == this || e.ParentConfigurationFileElement == this)
            {
                throw;
            }
            catch (Exception e)
            {
                LogHelper.Context.Log.Error(e);
                throw new ConfigurationParseException(this, e.Message);
            }
        }

        private T InterceptValueInitialzerExceptionWithReturnedValue<T>([NotNull] Func<T> funcToIntercept)
        {
            var value = default(T);

            InterceptValueInitialzerException(() => { value = funcToIntercept(); });

            return value;
        }

        #endregion
    }
}