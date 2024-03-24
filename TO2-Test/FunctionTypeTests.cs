﻿using System;
using System.Collections.Generic;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;
using Xunit;

namespace KontrolSystem.TO2.Test;

public class FunctionTypeTests {
    [Fact]
    public void TestMakeAction() {
        var context = new Context(KontrolRegistry.CreateCore());
        var moduleContext = context.CreateModuleContext("Test");

        Assert.Equal(typeof(Func<object>),
            new FunctionType(false, [], BuiltinType.Unit).GeneratedType(moduleContext));
        Assert.Equal(typeof(Func<long, object>),
            new FunctionType(false, [BuiltinType.Int], BuiltinType.Unit).GeneratedType(
                moduleContext));
        Assert.Equal(typeof(Func<long, string, object>),
            new FunctionType(false, [BuiltinType.Int, BuiltinType.String], BuiltinType.Unit)
                .GeneratedType(moduleContext));
        Assert.Equal(typeof(Func<long, string, bool, object>),
            new FunctionType(false, [BuiltinType.Int, BuiltinType.String, BuiltinType.Bool],
                BuiltinType.Unit).GeneratedType(moduleContext));
        Assert.Equal(typeof(Func<long, string, bool, double, object>),
            new FunctionType(false,
                [BuiltinType.Int, BuiltinType.String, BuiltinType.Bool, BuiltinType.Float],
                BuiltinType.Unit).GeneratedType(moduleContext));
    }

    [Fact]
    public void TestMakeFunc() {
        var context = new Context(KontrolRegistry.CreateCore());
        var moduleContext = context.CreateModuleContext("Test");

        Assert.Equal(typeof(Func<long>),
            new FunctionType(false, [], BuiltinType.Int).GeneratedType(moduleContext));
        Assert.Equal(typeof(Func<long, string>),
            new FunctionType(false, [BuiltinType.Int], BuiltinType.String).GeneratedType(
                moduleContext));
        Assert.Equal(typeof(Func<long, string, bool>),
            new FunctionType(false, [BuiltinType.Int, BuiltinType.String], BuiltinType.Bool)
                .GeneratedType(moduleContext));
        Assert.Equal(typeof(Func<long, string, bool, double>),
            new FunctionType(false, [BuiltinType.Int, BuiltinType.String, BuiltinType.Bool],
                BuiltinType.Float).GeneratedType(moduleContext));
        Assert.Equal(typeof(Func<long, string, bool, double, long>),
            new FunctionType(false,
                new List<TO2Type> { BuiltinType.Int, BuiltinType.String, BuiltinType.Bool, BuiltinType.Float },
                BuiltinType.Int).GeneratedType(moduleContext));
    }
}
