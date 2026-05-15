// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// 
// Description: BlockUIContainer - a wrapper for embedded UIElements in text 
//    flow content block collections
//

using System.Windows.Markup; // ContentProperty

namespace System.Windows.Documents
{
    /// <summary>
    /// BlockUIContainer - a wrapper for embedded UIElements in text 
    /// flow content block collections
    /// </summary>
    [ContentProperty("Child")]
    public partial class BlockUIContainer : Block
    {
        //-------------------------------------------------------------------
        //
        // Constructors
        //
        //-------------------------------------------------------------------

        #region Constructors

        /// <summary>
        /// Initializes a new instance of BlockUIContainer element.
        /// </summary>
        /// <remarks>
        /// The purpose of this element is to be a wrapper for UIElements
        /// when they are embedded into text flow - as items of
        /// BlockCollections.
        /// </remarks>
        public BlockUIContainer()
            : base()
        {
        }

        /// <summary>
        /// Initializes an BlockUIContainer specifying its child UIElement
        /// </summary>
        /// <param name="uiElement">
        /// UIElement set as a child of this block item
        /// </param>
        public BlockUIContainer(UIElement uiElement)
            : base()
        {
            ArgumentNullException.ThrowIfNull(uiElement);
            this.Child = uiElement;
        }

        #endregion Constructors

        //-------------------------------------------------------------------
        //
        // Public Properties
        //
        //-------------------------------------------------------------------

        #region Properties

        /// <summary>
        /// The content spanned by this TextElement.
        /// </summary>
        public UIElement Child
        {
            get
            {
#if HAS_UNO
                return _child;
#else
                return this.ContentStart.GetAdjacentElement(LogicalDirection.Forward) as UIElement;
#endif
            }

            set
            {
#if HAS_UNO
                if (_child != null)
                {
                    ContainerTextElementField.ClearValue(_child);
                }

                _child = value;
                if (_child != null)
                {
                    ContainerTextElementField.SetValue(_child, this);
                }
#else
                TextContainer textContainer = this.TextContainer;

                textContainer.BeginChange();
                try
                {
                    TextPointer contentStart = this.ContentStart;

                    UIElement child = Child;
                    if (child != null)
                    {
                        textContainer.DeleteContentInternal(contentStart, this.ContentEnd);
                        ContainerTextElementField.ClearValue(child);
                    }

                    if (value != null)
                    {
                        ContainerTextElementField.SetValue(value, this);
                        contentStart.InsertUIElement(value);
                    }
                }
                finally
                {
                    textContainer.EndChange();
                }
#endif
            }
        }

        #endregion

#if HAS_UNO
        private UIElement? _child;
#endif
    }
}
