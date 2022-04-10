/*
 https://stackoverflow.com/questions/38325824/log-method-entry-and-exit-in-net-using-nlog
 */

using System;
using System.Reflection;
using MethodDecorator.Fody.Interfaces;
using NLog;

[module: LogMethod] // Atribute should be "registered" by adding as module or assembly custom attribute
// Any attribute which provides OnEntry/OnExit/OnException with proper args
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
public class LogMethodAttribute : Attribute, IMethodDecorator
{
    private MethodBase _method;
    // instance, method and args can be captured here and stored in attribute instance fields
    // for future usage in OnEntry/OnExit/OnException
    public void Init(object instance, MethodBase method, object[] args)
    {
        _method = method;
    }
    public void OnEntry()
    {
        LogManager.GetCurrentClassLogger().Trace("Entering into {0}", _method.Name);            
    }

    public void OnExit()
    {
        LogManager.GetCurrentClassLogger().Trace("Exiting into {0}", _method.Name);        
    }

    public void OnException(Exception exception)
    {
        //LogManager.GetCurrentClassLogger().Trace(exception, "Exception {0}", _method.Name);
    }
}

