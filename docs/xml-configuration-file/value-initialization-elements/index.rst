=============================
Value Initialization Elements
=============================

Value initialization elements are elements that are used to specify values in different contexts. Some examples are using elements like **int32**, **constructedValue**, **injectedObject**, **object**, or **constructedValue** to specify constructor parameter value, or a value to returned in **autoMethod** or **autoProperty** elements.

Value initialization elements normally have attributes or child elements to specify the value, and might have some additional attributes, based on the type of the element and the context, where the element is used.

For example, if the value initialization element is used to specify injected property or constructor parameter value, the element should have a **name** attribute, to specify the constructor parameter or property name.

On the other hand, if the element is used to specify a returned value in **autoMethod** or **autoProperty** element, no **name** attribute should be present.

.. toctree::

    predefined-type-value-elements.rst
    object.rst
    injected-object.rst
    collection.rst
    constructed-value.rst
    setting-value.rst
    class-member.rst
